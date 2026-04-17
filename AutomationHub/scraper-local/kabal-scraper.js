const { Builder, By, until } = require('selenium-webdriver');
const edge = require('selenium-webdriver/edge');
const express = require('express');
const axios = require('axios');
const path = require('path');

const app = express();
app.use(express.json());

// Serve the UI file
app.get('/', (req, res) => {
    res.sendFile(path.join(__dirname, '..', 'workflows', 'sync-ui.html'));
});

const CONFIG = {
    // Login entry point — triggers SSO and lands back on the APEX app after auth
    loginUrl: process.env.KABAL_LOGIN_URL || 'https://account01.kabal.com/w/web/r/wels/kabal-account/',
    tmutilsUrl: process.env.TMUTILS_URL || 'http://localhost:9001',
    // Each operator lives in a separate APEX application (different App ID).
    // session=0 tells APEX to create a fresh session from active SSO cookies.
    // The page item parameters (P328_VALUE_TYPE, PATH) filter page 328 to the
    // loadout/cargo view — without them the page renders empty or wrong.
    // Format: f?p=AppID:PageID:Session::Language::ItemNames:ItemValues
    operatorAppUrls: {
        'Equinor':        'https://app01.kabal.com/mws/f?p=16786:328:0::NO::P328_VALUE_TYPE,P328_ACTION,P3523_FILTER,PATH:loadout,,,cargo.operations.loadout',
        'Vår Energi':     'https://app01.kabal.com/mws/f?p=1364:328:0::NO::P328_VALUE_TYPE,P328_ACTION,P3523_FILTER,PATH:loadout,,,cargo.operations.loadout',
        'ConocoPhillips': 'https://app01.kabal.com/mws/f?p=1267:328:0::NO::P328_VALUE_TYPE,P328_ACTION,P3523_FILTER,PATH:loadout,,,cargo.operations.loadout',
        'AkerBP':         'https://app01.kabal.com/mws/f?p=1276:328:0::NO::P328_VALUE_TYPE,P328_ACTION,P3523_FILTER,PATH:loadout,,,cargo.operations.loadout'
    },
    // Display name used when clicking the operator selector inside the APEX app
    operatorMapping: {
        'AkerBP': 'AkerBP',
        'ConocoPhillips': 'ConocoPhillips - Operator (NO)',
        'Equinor': 'Equinor Norge - Operator (NO)',
        'Vår Energi': 'Vaar Energi - Operator (NO)'
    }
};

// ---------------------------------------------------------------------------
// Build an Edge WebDriver (headless by default, visible if headless=false)
// ---------------------------------------------------------------------------
async function buildDriver(headless = true) {
    const options = new edge.Options();
    if (headless) {
        options.addArguments('--headless');  
        options.addArguments('--disable-gpu');
    }
    options.addArguments('--no-sandbox');
    options.addArguments('--disable-dev-shm-usage');
    return new Builder().forBrowser('MicrosoftEdge').setEdgeOptions(options).build();
}

// ---------------------------------------------------------------------------
// Extract all rows from an Oracle APEX Interactive Report on the current page,
// walking through every pagination page until there are no more.
// Returns an array of { colHeader: value, ... } objects.
// ---------------------------------------------------------------------------
async function scrapeApexIR(driver) {
    const allRows = [];

    while (true) {
        // Wait for the IR table (APEX renders it as .a-IRR-table or a table inside
        // a div with id containing "apxir" / "report" - try all common selectors)
        await driver.sleep(1500);

        let headers = [];
        let tableFound = false;

        // Try standard APEX IR selector first, then fall back to any table
        const tableSelectors = [
            'table.a-IRR-table',
            'table[summary]',
            'table.uReportStandard',
            'table tbody',
        ];

        for (const sel of tableSelectors) {
            try {
                const tbl = await driver.findElement(By.css(sel));
                if (tbl) {
                    // Collect headers from <th> elements
                    const thEls = await driver.findElements(By.css(`${sel.split(' ')[0]} thead th, ${sel.split(' ')[0]} th`));
                    headers = await Promise.all(thEls.map(th => th.getText()));
                    headers = headers.map(h => h.trim()).filter(Boolean);

                    // Collect data rows
                    const rows = await driver.findElements(By.css(`${sel.split(' ')[0]} tbody tr`));
                    for (const row of rows) {
                        try {
                            const cells = await row.findElements(By.css('td'));
                            if (cells.length === 0) continue;
                            const values = await Promise.all(cells.map(c => c.getText()));
                            const obj = {};
                            values.forEach((v, i) => {
                                obj[headers[i] || `col_${i}`] = v.trim();
                            });
                            allRows.push(obj);
                        } catch (e) { /* skip malformed row */ }
                    }
                    tableFound = true;
                    break;
                }
            } catch (_) { /* selector not found, try next */ }
        }

        if (!tableFound) {
            console.log('[WARN] No table found on this page — check selector or page structure');
            break;
        }

        // Try to click the APEX IR "Next Page" pagination button
        let nextClicked = false;
        try {
            // APEX pagination: button/link with aria-label "Next" or title "Next"
            // and also the classic .pagination .next or .uPaginationNext
            const nextSelectors = [
                'button[title="Next"]',
                'a[title="Next"]',
                '.a-IRR-pagination-item--next:not(.is-disabled)',
                '.uButtonPaginationNext:not(:disabled)',
                'span.next:not(.disabled)',
            ];
            for (const ns of nextSelectors) {
                try {
                    const nextBtn = await driver.findElement(By.css(ns));
                    const isDisabled = await nextBtn.getAttribute('disabled');
                    const classList = await nextBtn.getAttribute('class') || '';
                    if (!isDisabled && !classList.includes('is-disabled') && !classList.includes('disabled')) {
                        await nextBtn.click();
                        nextClicked = true;
                        console.log('[INFO] Navigating to next page of results...');
                        break;
                    }
                } catch (_) { /* not found */ }
            }
        } catch (_) { /* pagination unavailable */ }

        if (!nextClicked) break; // no more pages
    }

    return allRows;
}

// ---------------------------------------------------------------------------
// Parse rows from the APEX IR into modem objects
// ---------------------------------------------------------------------------
function parseModems(rows) {
    const modems = [];

    for (const row of rows) {
        // Normalise keys to lowercase for resilience against header casing
        const lc = {};
        for (const [k, v] of Object.entries(row)) lc[k.toLowerCase()] = v;

        // Extract modem number — look in every field for M-XXXXXXX or plain digits
        let modemNumber = '';
        let packageName = '';
        let kabalId = '';
        let shippingDate = '';
        let supplier = '';
        let destination = '';

        for (const [key, val] of Object.entries(lc)) {
            if (!val) continue;

            // Kabal ID  e.g. VAAR179300, FBS1234, ...
            if (!kabalId && /^[A-Z]{2,6}\d{4,10}$/i.test(val.trim())) kabalId = val.trim();

            // Package / description containing modem number
            if (!packageName && (val.includes('M-') || val.toLowerCase().includes('modem'))) {
                packageName = val.trim();
                const m = val.match(/M-(\d+)/i);
                if (m) modemNumber = m[1];
            }

            // Shipping / departure date — various formats
            if (!shippingDate && /\d{2}[\-\/\.]\d{2}[\-\/\.]\d{2,4}/.test(val)) {
                shippingDate = val.trim();
            }
            if (!shippingDate && /\d{4}-\d{2}-\d{2}/.test(val)) shippingDate = val.trim();

            // Supplier — look for Halliburton / HLB keywords
            if (!supplier && /HLB|Halliburton|Baker|Schlumberger|SLB/i.test(val)) {
                supplier = val.trim();
            }

            // Destination / rig
            if (!destination && key.includes('dest')) destination = val.trim();
        }

        // Fall back: look for modem number in kabalId field too
        if (!modemNumber) {
            const m = (packageName + ' ' + kabalId).match(/\b(\d{6,8})\b/);
            if (m) modemNumber = m[1];
        }

        if (modemNumber || kabalId) {
            modems.push({ modemNumber, shippingDate, kabalId, packageName, supplier, destination, raw: row });
        }
    }

    return modems;
}

// ---------------------------------------------------------------------------
// Normalise various date formats to YYYY-MM-DD
// ---------------------------------------------------------------------------
function normaliseDate(dateStr) {
    if (!dateStr) return null;

    // Already ISO
    if (/^\d{4}-\d{2}-\d{2}$/.test(dateStr)) return dateStr;

    // DD-MM-YY or DD/MM/YY
    const m1 = dateStr.match(/^(\d{2})[\-\/\.](\d{2})[\-\/\.](\d{2})$/);
    if (m1) return `20${m1[3]}-${m1[2]}-${m1[1]}`;

    // DD-MM-YYYY
    const m2 = dateStr.match(/^(\d{2})[\-\/\.](\d{2})[\-\/\.](\d{4})$/);
    if (m2) return `${m2[3]}-${m2[2]}-${m2[1]}`;

    // MMM DD-MM-YY  e.g. "Jan 15-03-26"
    const m3 = dateStr.match(/(\d{2})-(\d{2})-(\d{2})/);
    if (m3) return `20${m3[3]}-${m3[2]}-${m3[1]}`;

    return dateStr;
}

// ---------------------------------------------------------------------------
// Main scrape function
// ---------------------------------------------------------------------------
async function scrapeKabal(operator, rigName, username, password, headless = true, dryRun = false) {
    console.log('[INFO] Starting Kabal scraper');
    console.log(`[INFO] Operator: ${operator}, Rig: ${rigName || 'all'}`);

    const driver = await buildDriver(headless);

    try {
        // ── Step 1: Login ──────────────────────────────────────────────────
        console.log('[INFO] Navigating to login page...');
        await driver.get(CONFIG.loginUrl);
        await driver.sleep(2000);

        console.log('[INFO] Entering credentials...');
        const usernameField = await driver.wait(
            until.elementLocated(By.css('input[type="text"], input[type="email"]')), 15000
        );
        await usernameField.clear();
        await usernameField.sendKeys(username);

        const passwordField = await driver.findElement(By.css('input[type="password"]'));
        await passwordField.clear();
        await passwordField.sendKeys(password);

        const submitBtn = await driver.findElement(By.css('button[type="submit"]'));
        await submitBtn.click();
        await driver.sleep(3000);

        // ── Step 2: Operator selection (if present) ────────────────────────
        const operatorName = CONFIG.operatorMapping[operator] || operator;
        try {
            console.log(`[INFO] Selecting operator: ${operatorName}`);
            const operatorLink = await driver.wait(
                until.elementLocated(By.xpath(`//*[contains(text(), "${operatorName}")]`)), 8000
            );
            await operatorLink.click();
            await driver.sleep(3000);
        } catch (e) {
            console.log('[INFO] Operator selector not found — may already be in app context');
        }

        // ── Step 3: Navigate to operator-specific APEX app page ───────────────
        // Each operator has their own App ID; session=0 creates a fresh session from SSO cookies.
        const targetPageUrl = CONFIG.operatorAppUrls[operator]
            || 'https://app01.kabal.com/mws/f?p=16786:328:0::NO::P328_VALUE_TYPE,P328_ACTION,P3523_FILTER,PATH:loadout,,,cargo.operations.loadout';  // fallback to Equinor
        console.log(`[INFO] Navigating to APEX app for ${operator}: ${targetPageUrl}`);
        await driver.get(targetPageUrl);
        await driver.sleep(3000);

        // Verify we're not on the login page again
        const currentUrl = await driver.getCurrentUrl();
        if (currentUrl.includes('login') || currentUrl.includes('kabal-account:login')) {
            throw new Error('Redirected back to login — credentials may be incorrect or SSO session expired');
        }
        console.log(`[INFO] Landed on: ${currentUrl}`);

        // ── Step 4: Scrape all rows from the APEX IR ───────────────────────
        console.log('[INFO] Scraping APEX Interactive Report...');
        const rows = await scrapeApexIR(driver);
        console.log(`[INFO] Total raw rows collected: ${rows.length}`);

        if (rows.length > 0) {
            console.log('[DEBUG] Sample row:', JSON.stringify(rows[0]));
        }

        const modems = parseModems(rows);
        console.log(`[SUCCESS] Parsed ${modems.length} modem records`);

        // ── Step 5: Push to TMUtils (skipped in dryRun mode) ────────────────
        const results = [];
        if (!dryRun) {
            for (const modem of modems) {
                try {
                    const isoDate = normaliseDate(modem.shippingDate);
                    const response = await axios.post(
                        `${CONFIG.tmutilsUrl}/modem-shift.html`,
                        {
                            modemNumber: modem.modemNumber,
                            deliveryDate: isoDate,
                            kabalId: modem.kabalId,
                            packageName: modem.packageName,
                            supplier: modem.supplier
                        },
                        { timeout: 10000 }
                    );
                    results.push({ modemNumber: modem.modemNumber, status: 'success' });
                } catch (e) {
                    results.push({ modemNumber: modem.modemNumber, status: 'error', error: e.message });
                }
            }
        }

        await driver.quit();

        return {
            success: true,
            scraped: modems.length,
            dryRun,
            modems,  // always returned so callers can inspect the data
            updated: results.filter(r => r.status === 'success').length,
            failed: results.filter(r => r.status === 'error').length,
            results,
            rawRows: rows
        };

    } catch (error) {
        console.error('[ERROR]', error.message);
        try { await driver.quit(); } catch (_) {}
        throw error;
    }
}

app.post('/scrape', async (req, res) => {
    const { operator, rigName, username, password, headless = true, dryRun = false } = req.body;

    if (!operator || !username || !password) {
        return res.status(400).json({ error: 'Missing required fields: operator, username, password' });
    }

    console.log(`[INFO] Received scrape request for ${operator} (dryRun=${dryRun})`);

    try {
        const result = await scrapeKabal(operator, rigName, username, password, headless, dryRun);
        res.json(result);
    } catch (error) {
        console.error('[ERROR] Scraping failed:', error);
        res.status(500).json({ error: error.message });
    }
});

app.get('/health', (req, res) => {
    res.json({ status: 'ok' });
});

// Debug endpoint: login, navigate to page 328, return raw HTML + rows without
// posting anything to TMUtils. Useful for mapping column names on first run.
// POST { operator, username, password, headless: false }
app.post('/debug-page', async (req, res) => {
    const { operator, username, password, headless = false } = req.body;

    if (!username || !password) {
        return res.status(400).json({ error: 'Missing username / password' });
    }

    const driver = await buildDriver(headless);
    try {
        const operatorName = (CONFIG.operatorMapping[operator] || operator || 'Equinor');

        console.log('[DEBUG] Login...');
        await driver.get(CONFIG.loginUrl);
        await driver.sleep(2000);

        const usernameField = await driver.wait(
            until.elementLocated(By.css('input[type="text"], input[type="email"]')), 15000
        );
        await usernameField.sendKeys(username);
        await driver.findElement(By.css('input[type="password"]')).sendKeys(password);
        await driver.findElement(By.css('button[type="submit"]')).click();
        await driver.sleep(3000);

        try {
            const opLink = await driver.wait(
                until.elementLocated(By.xpath(`//*[contains(text(), "${operatorName}")]`)), 6000
            );
            await opLink.click();
            await driver.sleep(3000);
        } catch (_) {}

        console.log('[DEBUG] Navigating to app page...');
        const targetPageUrl = CONFIG.operatorAppUrls[operator] || 'https://app01.kabal.com/mws/f?p=16786:328:0::NO::P328_VALUE_TYPE,P328_ACTION,P3523_FILTER,PATH:loadout,,,cargo.operations.loadout';
        await driver.get(targetPageUrl);
        await driver.sleep(3000);

        const currentUrl = await driver.getCurrentUrl();
        const pageSource = await driver.getPageSource();

        // Grab just the table portion to avoid sending MB of HTML
        const rows = await scrapeApexIR(driver);
        await driver.quit();

        res.json({
            currentUrl,
            rowCount: rows.length,
            sampleRows: rows.slice(0, 5),
            columns: rows.length > 0 ? Object.keys(rows[0]) : [],
            // First 4000 chars of page source for inspector
            pageSourcePreview: pageSource.substring(0, 4000)
        });
    } catch (e) {
        try { await driver.quit(); } catch (_) {}
        res.status(500).json({ error: e.message });
    }
});

const PORT = 3000;
app.listen(PORT, () => {
    console.log(`[INFO] Kabal scraper API running on port ${PORT}`);
    console.log(`[INFO] Open http://localhost:${PORT} to access the UI`);
});
