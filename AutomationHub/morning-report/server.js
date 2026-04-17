// Morning Report Server — standalone replacement for the n8n morning-report workflow.
// Exposes 3 endpoints (same paths the dashboard already calls):
//   POST /webhook/morning-report       → process incoming Excel files
//   GET  /webhook/morning-report-data   → return all-rigs-data.json
//   GET  /webhook/morning-report-export → return master Excel as base64
//   GET  /healthz                       → health check

const http = require('http');
const fs   = require('fs');
const path = require('path');

const xlsx = require('xlsx');

// ── Paths (OneDrive) ─────────────────────────────────────────────────────────
const BASE_DIR       = path.join(process.env.USERPROFILE || '', 'OneDrive - Halliburton', 'Documents', 'MorningReports');
const inputDir       = path.join(BASE_DIR, 'incoming');
const processedDir   = path.join(BASE_DIR, 'processed');
const skippedDir     = path.join(BASE_DIR, 'skipped');
const masterXlsxFile = path.join(BASE_DIR, 'morning-report-master.xlsx');
const allRigsFile    = path.join(BASE_DIR, 'all-rigs-data.json');

const PORT = 3001;

// ── Helpers ──────────────────────────────────────────────────────────────────
function getCol(r, i) { return String((r || [])[i] || '').trim(); }

function extractReportDate(filename) {
  var m = filename.match(/(\d{2})\.(\d{2})\.(\d{4})/);
  if (m) return new Date(+m[3], +m[2] - 1, +m[1]);
  var m2 = filename.match(/(\d{4})-(\d{2})-(\d{2})/);
  if (m2) return new Date(+m2[1], +m2[2] - 1, +m2[3]);
  return new Date(0);
}

function extractRigName(filename) {
  var base = path.basename(filename, path.extname(filename));
  base = base.replace(/^\d{4}-\d{2}-\d{2}_\d{2}-\d{2}_/, '');
  base = base.replace(/\s+\d{1,2}[.\s]\d{1,2}[.\s]\d{4}$/, '');
  var idx = base.toLowerCase().indexOf(' morning report');
  if (idx >= 0) return base.substring(0, idx).trim();
  return base.trim() || 'Unknown';
}

function mkDateStr(d) { return d.toISOString().split('T')[0]; }
function mkDisplayDate(d) { return d.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' }); }

function parseExpiryDate(raw, today) {
  if (!raw) return { str: '', days: '', status: 'OK' };
  raw = String(raw).trim();
  var exp;
  var n = parseFloat(raw);
  if (!isNaN(n) && n > 40000 && n < 100000 && raw.indexOf('/') === -1 && raw.indexOf('.') === -1) {
    exp = new Date(Math.round((n - 25569) * 86400000));
    raw = ('0'+exp.getUTCDate()).slice(-2)+'/'+('0'+(exp.getUTCMonth()+1)).slice(-2)+'/'+exp.getUTCFullYear();
  }
  var parts = raw.split('/');
  if (parts.length === 3) exp = new Date(+parts[2], +parts[1]-1, +parts[0]);
  if (!exp || isNaN(exp.getTime())) {
    parts = raw.split('.');
    if (parts.length === 3) {
      var yr = parts[2].length === 2 ? 2000 + +parts[2] : +parts[2];
      exp = new Date(yr, +parts[1]-1, +parts[0]);
    }
  }
  if (!exp || isNaN(exp.getTime())) return { str: raw, days: '', status: '-' };
  exp.setHours(0,0,0,0);
  var days = Math.floor((exp - today) / 86400000);
  return { str: raw, days: days, status: days < 0 ? 'EXPIRED' : days <= 30 ? 'EXPIRING SOON' : 'OK' };
}

function parseExcelFile(filePath, today) {
  var wb  = xlsx.readFile(filePath);
  var ws  = wb.Sheets[wb.SheetNames[0]];
  var raw = xlsx.utils.sheet_to_json(ws, { header: 1, defval: '' });

  var wsRef = xlsx.utils.decode_range(ws['!ref'] || 'A1');
  var bCol = (wsRef.s.c >= 1) ? 0 : 1;
  function bc(n) { return bCol + n; }

  var kitsRow = -1, certsRow = -1, certsEndRow = -1, insiteRow = -1;
  var ddRow = -1, mwdRow = -1;
  var opsLast24Row = -1, opsNext24Row = -1;

  for (var i = 0; i < raw.length; i++) {
    var vb = getCol(raw[i], bc(0)).toLowerCase();
    var vc = getCol(raw[i], bc(1)).toLowerCase();
    if (vb === 'kits')                        kitsRow      = i;
    if (vb.indexOf('equipment cert') === 0)   certsRow     = i;
    if (vb.indexOf('dd personnel')  >= 0)     ddRow        = i;
    if (vb.indexOf('mwd personnel') >= 0)     mwdRow       = i;
    var rowText = raw[i].map(function(c){ return String(c||'').toLowerCase(); }).join(' ');
    if (opsLast24Row === -1 && rowText.indexOf('last 24') >= 0) opsLast24Row = i;
    if (opsNext24Row === -1 && rowText.indexOf('next 24') >= 0) opsNext24Row = i;
    if (insiteRow === -1 && rowText.indexOf('insite machines') >= 0) insiteRow = i;
    if (certsRow >= 0 && certsEndRow === -1 && i > certsRow + 2) {
      var trigger = vb + ' ' + vc;
      if (trigger.indexOf('date issued') >= 0 || trigger.indexOf('revision no') >= 0 ||
          vb.indexOf('issued') >= 0 || vb === 'date' ||
          rowText.indexOf('insite machines') >= 0) {
        certsEndRow = i;
      }
    }
  }
  if (certsEndRow === -1) certsEndRow = raw.length;

  function extractOpsText(labelRowIdx) {
    if (labelRowIdx < 0) return '';
    // Activity text lives on the SAME row as the label, in a column to the right
    var row = raw[labelRowIdx];
    for (var ci = bc(0) + 1; ci < row.length; ci++) {
      var v = String(row[ci] || '').trim();
      if (v && v.length > 5) return v;
    }
    return '';
  }

  var operations = {
    last24: extractOpsText(opsLast24Row),
    next24: extractOpsText(opsNext24Row)
  };

  function parsePersonnel(rowIdx) {
    if (rowIdx < 0 || rowIdx + 1 >= raw.length) return { day:'', flex:'', night:'', crewChangeName:'', crewIn:'', crewOut:'' };
    var hdr = raw[rowIdx];
    var dat = raw[rowIdx + 1];
    var dayCol=-1, flexCol=-1, nightCol=-1, crewChangeCol=-1, inCol=-1, outCol=-1;
    for (var c = bc(0); c < hdr.length; c++) {
      var h = String(hdr[c] || '').trim().toLowerCase();
      if (h === 'day')                                         dayCol        = c;
      if (h === 'flex')                                        flexCol       = c;
      if (h === 'night' || h === 'nights')                     nightCol      = c;
      if (h.indexOf('crew change') >= 0 && h.indexOf('name') >= 0) crewChangeCol = c;
      if (h === 'in')                                          inCol         = c;
      if (h === 'out')                                         outCol        = c;
    }
    return {
      day:            dayCol        >= 0 ? getCol(dat, dayCol)        : '',
      flex:           flexCol       >= 0 ? getCol(dat, flexCol)       : '',
      night:          nightCol      >= 0 ? getCol(dat, nightCol)      : '',
      crewChangeName: crewChangeCol >= 0 ? getCol(dat, crewChangeCol) : '',
      crewIn:         inCol         >= 0 ? getCol(dat, inCol)         : '',
      crewOut:        outCol        >= 0 ? getCol(dat, outCol)        : ''
    };
  }

  var ddPersonnel  = parsePersonnel(ddRow);
  var mwdPersonnel = parsePersonnel(mwdRow);

  var kits = [];
  if (kitsRow >= 0 && certsRow > kitsRow) {
    for (var ki = kitsRow + 2; ki < certsRow; ki++) {
      var sn = getCol(raw[ki], bc(0));
      if (!sn || sn.toLowerCase() === 's/n' || sn.toLowerCase() === 'serial') continue;
      kits.push({ sn: sn, kitName: getCol(raw[ki], bc(2)), location: getCol(raw[ki], bc(5)) });
    }
  }

  var certs = [];
  if (certsRow >= 0) {
    for (var ci2 = certsRow + 2; ci2 < certsEndRow; ci2++) {
      var equipment = getCol(raw[ci2], bc(0));
      if (!equipment) continue;
      var eqLow = equipment.toLowerCase();
      if (eqLow === 'equipment' || eqLow.indexOf('date issued') >= 0 || eqLow.indexOf('revision') >= 0) continue;
      var expiry = parseExpiryDate(getCol(raw[ci2], bc(2)), today);
      certs.push({
        equipment:     equipment,
        expiryDate:    expiry.str,
        description:   getCol(raw[ci2], bc(5)),
        location:      getCol(raw[ci2], bc(14)),
        daysRemaining: expiry.days,
        status:        expiry.status
      });
    }
  }

  var insiteMachines = [];
  if (insiteRow >= 0) {
    var insiteHdr = raw[insiteRow + 1] || [];
    var colCN=-1, colType=-1, colIV=-1, colDesig=-1, colAN=-1, colLoc=-1, colLic=-1, colMac=-1;
    for (var c = bc(0); c < insiteHdr.length; c++) {
      var h = String(insiteHdr[c] || '').trim().toLowerCase();
      if (h === 'computer name')             colCN    = c;
      if (h === 'type')                      colType  = c;
      if (h.indexOf('insite version') >= 0)  colIV    = c;
      if (h === 'designation')               colDesig = c;
      if (h === 'asset name')                colAN    = c;
      if (h === 'location')                  colLoc   = c;
      if (h.indexOf('license') >= 0)         colLic   = c;
      if (h.indexOf('mac') >= 0)             colMac   = c;
    }
    for (var ii = insiteRow + 2; ii < raw.length; ii++) {
      var assetName = colAN >= 0 ? getCol(raw[ii], colAN) : '';
      var compName  = colCN >= 0 ? getCol(raw[ii], colCN) : '';
      if (!assetName && !compName) continue;
      var rowTxt2 = raw[ii].map(function(c){ return String(c||'').toLowerCase(); }).join(' ');
      if (rowTxt2.indexOf('revision') >= 0 || rowTxt2.indexOf('date issued') >= 0) break;
      insiteMachines.push({
        assetName:     assetName,
        computerName:  compName,
        type:          colType  >= 0 ? getCol(raw[ii], colType)  : '',
        insiteVersion: colIV    >= 0 ? getCol(raw[ii], colIV)    : '',
        designation:   colDesig >= 0 ? getCol(raw[ii], colDesig) : '',
        location:      colLoc   >= 0 ? getCol(raw[ii], colLoc)   : '',
        license:       colLic   >= 0 ? getCol(raw[ii], colLic)   : '',
        mac:           colMac   >= 0 ? getCol(raw[ii], colMac)   : ''
      });
    }
  }

  var rigName  = getCol(raw[2] || [], bc(0)) || 'Unknown';
  var wellName = getCol(raw[6] || [], bc(12)) || '';

  return { rigName: rigName, wellName: wellName, operations: operations,
           ddPersonnel: ddPersonnel, mwdPersonnel: mwdPersonnel, kits: kits, certs: certs,
           insiteMachines: insiteMachines };
}

// ── Main processing function ─────────────────────────────────────────────────
function processReports() {
  [processedDir, skippedDir].forEach(function(d) {
    if (!fs.existsSync(d)) fs.mkdirSync(d, { recursive: true });
  });

  if (!fs.existsSync(inputDir)) {
    return { success: false, message: 'Incoming folder not found: ' + inputDir };
  }

  var today = new Date();
  today.setHours(0, 0, 0, 0);

  var allFiles = fs.readdirSync(inputDir)
    .filter(function(f) { return /\.(xlsx|xls)$/i.test(f) && f.toLowerCase().indexOf('morning report') !== -1; })
    .map(function(f) { return { name: f, rigName: extractRigName(f), reportDate: extractReportDate(f) }; });

  // Move non-morning-report Excel files to skipped/
  fs.readdirSync(inputDir).forEach(function(f) {
    if (!/\.(xlsx|xls)$/i.test(f)) return;
    if (f.toLowerCase().indexOf('morning report') === -1) {
      try { fs.renameSync(path.join(inputDir, f), path.join(skippedDir, f)); } catch(e) {}
    }
  });

  if (allFiles.length === 0) {
    return { success: false, message: 'No morning reports found in incoming folder.' };
  }

  var rigGroups = {};
  allFiles.forEach(function(f) {
    var r = f.rigName || 'Unknown';
    if (!rigGroups[r]) rigGroups[r] = [];
    rigGroups[r].push(f);
  });

  var allRigsData = {};
  var processedCount = 0;
  var rigList = [];

  Object.keys(rigGroups).forEach(function(rigName) {
    var files = rigGroups[rigName].sort(function(a, b) { return b.reportDate - a.reportDate; });
    var todayFiles = files.filter(function(f) { return f.reportDate <= today; });
    var todayFile    = todayFiles[0] || null;
    var previousFile = todayFiles[1] || null;

    for (var di = 2; di < todayFiles.length; di++) {
      try { fs.unlinkSync(path.join(inputDir, todayFiles[di].name)); } catch(e) {}
    }

    if (!todayFile) return;

    var todayData = null;
    try { todayData = parseExcelFile(path.join(inputDir, todayFile.name), today); } catch(e) { todayData = null; }
    if (!todayData) return;

    var prevData = null;
    if (previousFile) {
      try { prevData = parseExcelFile(path.join(inputDir, previousFile.name), today); } catch(e) { prevData = null; }
    }

    function resolveCrewChange(curP, prevP) {
      var statedChange = !!curP.crewChangeName;
      if (statedChange) {
        return { statedChange: true,
                 currentDay: curP.day, currentFlex: curP.flex, currentNight: curP.night,
                 crewChangeName: curP.crewChangeName, incoming: curP.crewIn || '',
                 changed: true };
      }
      if (prevP) {
        var dayChanged   = curP.day   && prevP.day   && curP.day   !== prevP.day;
        var nightChanged = curP.night && prevP.night && curP.night !== prevP.night;
        if (dayChanged || nightChanged) {
          return { statedChange: false,
                   currentDay: curP.day, currentFlex: curP.flex, currentNight: curP.night,
                   prevDay: prevP.day, prevFlex: prevP.flex, prevNight: prevP.night,
                   dayChanged: dayChanged, nightChanged: nightChanged,
                   changed: true };
        }
      }
      return { statedChange: false,
               currentDay: curP.day, currentFlex: curP.flex, currentNight: curP.night,
               changed: false };
    }

    var ddCrewChange  = resolveCrewChange(todayData.ddPersonnel,  prevData ? prevData.ddPersonnel  : null);
    var mwdCrewChange = resolveCrewChange(todayData.mwdPersonnel, prevData ? prevData.mwdPersonnel : null);

    var kits = todayData.kits.map(function(k) { return { sn:k.sn, kitName:k.kitName, location:k.location, missing:false }; });
    if (prevData && prevData.kits) {
      var currentSNs = {};
      todayData.kits.forEach(function(k) { currentSNs[k.sn] = true; });
      prevData.kits.forEach(function(prev) {
        if (!currentSNs[prev.sn])
          kits.push({ sn:prev.sn, kitName:prev.kitName, location:prev.location, missing:true, note:'(BL?)' });
      });
    }

    var certsList = todayData.certs.map(function(c) {
      var prevExpiry = '';
      if (prevData && prevData.certs) {
        for (var pi = 0; pi < prevData.certs.length; pi++) {
          var p = prevData.certs[pi];
          if (p.equipment === c.equipment && p.expiryDate && p.expiryDate !== c.expiryDate) { prevExpiry = p.expiryDate; break; }
        }
      }
      return { equipment:c.equipment, expiryDate:c.expiryDate, description:c.description,
               location:c.location, daysRemaining:c.daysRemaining, status:c.status,
               prevExpiry:prevExpiry, missing:false };
    });
    if (prevData && prevData.certs) {
      var currentEquip = {};
      todayData.certs.forEach(function(c) { currentEquip[c.equipment] = true; });
      prevData.certs.forEach(function(prev) {
        if (!currentEquip[prev.equipment])
          certsList.push({ equipment:prev.equipment, expiryDate:prev.expiryDate, description:prev.description,
            location:prev.location, daysRemaining:prev.daysRemaining, status:prev.status,
            prevExpiry:'', missing:true, note:'(BL?)' });
      });
    }

    var summary = {
      kitsTotal:    todayData.kits.length,
      kitsMissing:  kits.filter(function(k) { return k.missing; }).length,
      certsTotal:   todayData.certs.length,
      expired:      certsList.filter(function(c) { return !c.missing && c.status === 'EXPIRED'; }).length,
      expiringSoon: certsList.filter(function(c) { return !c.missing && c.status === 'EXPIRING SOON'; }).length,
      certsMissing: certsList.filter(function(c) { return c.missing; }).length
    };

    var rptDate = todayFile.reportDate;
    allRigsData[rigName] = {
      reportDate:     mkDateStr(rptDate),
      displayDate:    mkDisplayDate(rptDate),
      prevReportDate: previousFile ? mkDateStr(previousFile.reportDate) : null,
      processedAt:    new Date().toISOString(),
      sourceFile:     todayFile.name,
      prevSourceFile: previousFile ? previousFile.name : null,
      rig:            todayData.rigName,
      well:           todayData.wellName,
      operations:     todayData.operations,
      ddCrewChange:   ddCrewChange,
      mwdCrewChange:  mwdCrewChange,
      kits:           kits,
      certs:          certsList,
      insiteMachines: todayData.insiteMachines || [],
      summary:        summary
    };
    rigList.push(rigName);
    processedCount++;
  });

  fs.writeFileSync(allRigsFile, JSON.stringify(allRigsData, null, 2));

  // Update master Excel — one tab per rig
  var masterWb;
  try { masterWb = fs.existsSync(masterXlsxFile) ? xlsx.readFile(masterXlsxFile) : xlsx.utils.book_new(); }
  catch(e) { masterWb = xlsx.utils.book_new(); }

  rigList.forEach(function(rigName) {
    var d = allRigsData[rigName];
    if (!d) return;
    var sheetName = rigName.substring(0, 31);
    var dd  = d.ddCrewChange  || {};
    var mwd = d.mwdCrewChange || {};
    var sheetData = [
      ['MORNING REPORT', '', '', '', '', '', ''],
      ['Date', d.displayDate, '', 'Rig', rigName, 'Well', d.well],
      [''],
      ['OPERATIONS'],
      ['Last 24 hours', (d.operations && d.operations.last24) || ''],
      ['Next 24 hours', (d.operations && d.operations.next24) || ''],
      [''],
      ['PERSONNEL', 'Day', 'Flex', 'Night', 'Crew Change'],
      ['DD',  dd.currentDay||'', dd.currentFlex||'', dd.currentNight||'',
        dd.statedChange ? dd.crewChangeName+' <=> '+(dd.incoming||'?') : (dd.changed ? '(prev: '+dd.prevDay+')' : '')],
      ['MWD', mwd.currentDay||'', mwd.currentFlex||'', mwd.currentNight||'',
        mwd.statedChange ? mwd.crewChangeName+' <=> '+(mwd.incoming||'?') : (mwd.changed ? '(prev: '+mwd.prevDay+')' : '')],
      [''],
      ['KITS ON BOARD', '', '', ''],
      ['S/N', 'Kit Name', 'Location', 'Flag']
    ];
    d.kits.forEach(function(k) { sheetData.push([k.sn, k.kitName, k.location, k.missing ? 'MISSING (BL?)' : '']); });
    sheetData.push(['']);
    sheetData.push(['EQUIPMENT CERTIFICATIONS', '', '', '', '', '']);
    sheetData.push(['Equipment', 'Expiry Date', 'Description', 'Location', 'Days Remaining', 'Status']);
    d.certs.forEach(function(c) {
      var statusOut = c.missing ? 'MISSING (BL?)' : (c.status + (c.prevExpiry ? ' [was: '+c.prevExpiry+']' : ''));
      sheetData.push([c.equipment, c.expiryDate, c.description, c.location, c.daysRemaining, statusOut]);
    });
    var wsNew = xlsx.utils.aoa_to_sheet(sheetData);
    if (masterWb.SheetNames.indexOf(sheetName) >= 0) {
      delete masterWb.Sheets[sheetName];
      masterWb.SheetNames.splice(masterWb.SheetNames.indexOf(sheetName), 1);
    }
    xlsx.utils.book_append_sheet(masterWb, wsNew, sheetName);
  });

  if (rigList.length > 0) xlsx.writeFile(masterWb, masterXlsxFile);

  return {
    success: true,
    message: 'Processed ' + processedCount + ' rig(s): ' + rigList.join(', '),
    rigs: rigList,
    processedCount: processedCount
  };
}

// ── HTTP Server ──────────────────────────────────────────────────────────────
function sendJSON(res, status, obj) {
  var body = JSON.stringify(obj);
  res.writeHead(status, {
    'Content-Type': 'application/json',
    'Access-Control-Allow-Origin': '*',
    'Access-Control-Allow-Methods': 'GET, POST, OPTIONS',
    'Access-Control-Allow-Headers': 'Content-Type'
  });
  res.end(body);
}

http.createServer(function(req, res) {
  var urlPath = req.url.split('?')[0];

  // CORS preflight
  if (req.method === 'OPTIONS') {
    res.writeHead(204, {
      'Access-Control-Allow-Origin': '*',
      'Access-Control-Allow-Methods': 'GET, POST, OPTIONS',
      'Access-Control-Allow-Headers': 'Content-Type'
    });
    return res.end();
  }

  // Health check
  if (urlPath === '/healthz') {
    return sendJSON(res, 200, { status: 'ok' });
  }

  // POST /webhook/morning-report — run processing
  if (req.method === 'POST' && urlPath === '/webhook/morning-report') {
    try {
      var result = processReports();
      return sendJSON(res, 200, result);
    } catch(e) {
      console.error('Processing error:', e);
      return sendJSON(res, 500, { success: false, message: e.message });
    }
  }

  // GET /webhook/morning-report-data — return JSON data
  if (req.method === 'GET' && urlPath === '/webhook/morning-report-data') {
    if (!fs.existsSync(allRigsFile)) {
      return sendJSON(res, 200, { allRigs: {}, rigList: [] });
    }
    try {
      var allRigs = JSON.parse(fs.readFileSync(allRigsFile, 'utf8'));
      var rigList = Object.keys(allRigs);
      return sendJSON(res, 200, { allRigs: allRigs, rigList: rigList, defaultRig: rigList[0] || '' });
    } catch(e) {
      return sendJSON(res, 500, { error: 'Could not read data file: ' + e.message });
    }
  }

  // GET /webhook/morning-report-export — return master Excel as base64
  if (req.method === 'GET' && urlPath === '/webhook/morning-report-export') {
    if (!fs.existsSync(masterXlsxFile)) {
      return sendJSON(res, 404, { error: 'Master Excel not found. Run report first.' });
    }
    try {
      var buf = fs.readFileSync(masterXlsxFile);
      var b64 = buf.toString('base64');
      return sendJSON(res, 200, {
        filename: 'morning-report-master.xlsx',
        mimeType: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
        base64: b64
      });
    } catch(e) {
      return sendJSON(res, 500, { error: 'Could not read master file: ' + e.message });
    }
  }

  sendJSON(res, 404, { error: 'Not found' });
}).listen(PORT, '127.0.0.1', function() {
  console.log('Morning Report server running at http://localhost:' + PORT);
  console.log('Data folder: ' + BASE_DIR);
});
