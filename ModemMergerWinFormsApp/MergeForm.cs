using ModemWebUtility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;

namespace ModemMergerWinFormsApp
{
    public partial class MergeForm : Form
    {
        private string targetModem; //used for treeveiwing naming
        private string targetModemNumber; //used for loading modem
        const string mwdName = "MWD";
        const string ddName = "DD";
        const string gpName = "GP";
        const string looseName = "Loose";

        private TreeNode targetNode = new TreeNode();
        private TreeNode targetMwdNode = new TreeNode(mwdName);
        private TreeNode targetDdNode = new TreeNode(ddName);
        private TreeNode targetGpNode = new TreeNode(gpName);
        private TreeNode targetLooseNode = new TreeNode(looseName);

        private FocusedTreeview lastFocused;
        private bool _headerChecked = true;
        private bool _loadoutLocked = false;
        private bool _etaLocked = false;

        // HTTP API server for the Modem Creator web UI
        private CreatorApiServer _apiServer;

        // Kabal sync preview: stores date info per Customer cell
        private class KabalDateInfo
        {
            public string KabalDate;
            public bool Matches;
        }




        public MergeForm()
        {
            InitializeComponent();
            UpdateTargetNodes(treeView2, "default");
            btnAdd.Enabled = false;
            btnGetModem.Enabled = false;
            LoadKabalCredentials();
            dgvKabal.CellPainting          += dgvKabal_CellPainting;
            dgvKabal.ColumnHeaderMouseClick  += dgvKabal_ColumnHeaderMouseClick;
            dgvKabal.CurrentCellDirtyStateChanged += dgvKabal_CurrentCellDirtyStateChanged;
            RefreshModemAutoComplete();

            // Start HTTP API server for the Modem Creator web UI (localhost:9002)
            try
            {
                _apiServer = new CreatorApiServer(() => _loadedModems);
                _apiServer.Start();
            }
            catch { /* non-critical — web UI will show connection error */ }
        }

        private void RefreshModemAutoComplete()
        {
            var src = new AutoCompleteStringCollection();
            src.AddRange(ModemHistory.GetHistory().ToArray());
            txtModemNo.AutoCompleteCustomSource = src;
            txtTargetModem.AutoCompleteCustomSource = src;
        }

        private void UpdateTargetNodes(TreeView treeview, string targetModemNo)
        {
            targetModem = targetModemNo;

            targetNode.Name = targetModem;
            targetNode.Text = targetModem;

            targetMwdNode.Name = mwdName;
            targetDdNode.Name = ddName;
            targetGpNode.Name = gpName;
            targetLooseNode.Name = looseName;

            targetNode.Nodes.Add(targetMwdNode);
            targetNode.Nodes.Add(targetDdNode);
            targetNode.Nodes.Add(targetGpNode);
            targetNode.Nodes.Add(targetLooseNode);

            treeview.Nodes.Add(targetNode);
        }


        private void btnGetModem_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            string modemNo;
            modemNo = txtModemNo.Text;

            string modemViewUrl = HDocUtility.UrlModemView;
            string bhaEditViewUrl = HDocUtility.BhaEditUrlwMwdId;
            string ddEditViewUrl = HDocUtility.UrlDdBhaEdit;
            string gpEditViewUrl = HDocUtility.UrlGpBhaEdit;
            string looseEditUrl = HDocUtility.UrlLooseItemEdit;

            ModemConnection mc = new ModemConnection(modemViewUrl + modemNo);
            ModemParameters mp = new ModemParameters(mc.GetHtmlAsHdoc(), modemNo);

            TreeNode mainNode = new TreeNode(modemNo);


            TreeNode mwdNode = new TreeNode(mwdName);
            mwdNode.Name = mwdName;
            TreeNode ddNode = new TreeNode(ddName);
            ddNode.Name = ddName;
            TreeNode gpNode = new TreeNode(gpName);
            gpNode.Name = gpName;
            TreeNode looseNode = new TreeNode(looseName);
            looseNode.Name = looseName;


            foreach (string item in mp.MwdId)
            {
                ModemConnection mcTemp = new ModemConnection(bhaEditViewUrl + item);
                MwdBhaParameters mbp = new MwdBhaParameters(mcTemp.GetHtmlAsHdoc());

                TreeNode tn = new TreeNode(ReplaceQuotes(mbp.MwdBhaPost.P_BHA_DESC));
                ModemMwdPostObjects tagObj = new ModemMwdPostObjects();

                tagObj.MwdCompPostDict = mbp.BhaCompPost;
                tagObj.MwdSoftPostDict = mbp.BhaSoftPost;
                tagObj.MwdBhaPost = mbp.MwdBhaPost;
                tn.Tag = tagObj;

                mwdNode.Nodes.Add(tn);
                if (!mainNode.Nodes.Contains(mwdNode))
                {
                    mainNode.Nodes.Add(mwdNode);
                }

            }



            foreach (string item in mp.DdId)
            {
                ModemConnection mcTemp = new ModemConnection(ddEditViewUrl + item);
                DdBhaParameters mbp = new DdBhaParameters(mcTemp.GetHtmlAsHdoc());

                TreeNode tn = new TreeNode(ReplaceQuotes(mbp.DdBhaPosts.P_MOTOR_DESC));
                ModemDdPostObjects tagObj = new ModemDdPostObjects();

                tagObj.DdCompPostDict = mbp.DdBhaCompPost;
                tagObj.DdBhaPast = mbp.DdBhaPosts;
                tn.Tag = tagObj;

                ddNode.Nodes.Add(tn);
                if (!mainNode.Nodes.Contains(ddNode))
                {
                    mainNode.Nodes.Add(ddNode);
                }

            }



            foreach (string item in mp.GpId)
            {

                ModemConnection mcTemp = new ModemConnection(gpEditViewUrl + item);
                GpBhaParameters mbp = new GpBhaParameters(mcTemp.GetHtmlAsHdoc());

                //foreach (var p in mbp.GpBhaPosts.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                //{
                //    if (p.PropertyType == typeof(Tuple<string, string>))
                //    {
                //        Tuple<string, string> temp = (Tuple<string, string>)p.GetValue(mbp.GpBhaPosts, null);
                //        if (p.Name == "P_L_HOLESEC")
                //        {
                //            MessageBox.Show("Original MBP: " + p.Name + " => " + temp.Item1 + " => " + temp.Item2);
                //        }


                //    }

                //}

                TreeNode tn = new TreeNode(ReplaceQuotes(mbp.GpBhaPosts.P_GP_DESC.Item2));
                ModemGpPostObjects tagObj = new ModemGpPostObjects
                {
                    GpCompPostDict = mbp.GpBhaCompPost,
                    GpBhaPost = mbp.GpBhaPosts
                };
                tn.Tag = tagObj;

                //foreach (var p in tagObj.GpBhaPost.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                //{
                //    if (p.PropertyType == typeof(Tuple<string, string>))
                //    {
                //        Tuple<string, string> temp = (Tuple<string, string>)p.GetValue(tagObj.GpBhaPost, null);
                //        if (p.Name == "P_L_HOLESEC")
                //        {
                //            MessageBox.Show("When getting from modem: "+ p.Name + " => " + temp.Item1 + " => " + temp.Item2);
                //        }


                //    }

                //}

                gpNode.Nodes.Add(tn);
                if (!mainNode.Nodes.Contains(gpNode))
                {
                    mainNode.Nodes.Add(gpNode);
                }

            }


            foreach (string item in mp.LooseId)
            {
                ModemConnection mcTemp = new ModemConnection(looseEditUrl + item);
                LooseBhaParameters mbp = new LooseBhaParameters(mcTemp.GetHtmlAsHdoc());


                TreeNode tn = new TreeNode(ReplaceQuotes(mbp.LooseBhaPosts[0].P_DESCRIPTION + " " + mbp.LooseBhaPosts[0].P_COMMENTS));
                ModemLoosePostObjects tagObj = new ModemLoosePostObjects();


                tagObj.LoosePostDict = mbp.LooseBhaPosts;

                tn.Tag = tagObj;

                looseNode.Nodes.Add(tn);


                if (!mainNode.Nodes.Contains(looseNode))
                {
                    mainNode.Nodes.Add(looseNode);
                }


            }

            treeView1.Nodes.Add(mainNode);
            ModemHistory.Add(modemNo);
            RefreshModemAutoComplete();
            Cursor.Current = Cursors.Default;
        }

        private void btnAddSelected_Click(object sender, EventArgs e)
        {
            if (lastFocused != FocusedTreeview.treeview1)
            {
                return;
            }

            TreeNode node = treeView1.SelectedNode;

            if (node.TreeView != treeView1)
            {
                return;
            }

            if (IsSelectableNode(node))
            {
                TreeNode newNode = new TreeNode();
                newNode.Name = node.Name;
                newNode.Text = node.Text;
                newNode.Tag = node.Tag;

                // Apply Main rename to node text for MWD and GP
                if (node.Parent != null && (node.Parent.Name == mwdName || node.Parent.Name == gpName))
                {
                    newNode.Text = RenameMainInText(newNode.Text);
                }

                int index = 0;

                switch (node.Parent != null ? node.Parent.Name : "")
                {
                    case mwdName:
                        index = targetNode.Nodes.IndexOf(targetMwdNode);
                        break;

                    case ddName:
                        index = targetNode.Nodes.IndexOf(targetDdNode);
                        break;

                    case gpName:
                        index = targetNode.Nodes.IndexOf(targetGpNode);

                        //ModemGpPostObjects t1 = node.Tag as ModemGpPostObjects;
                        //foreach (var p in t1.GpBhaPost.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                        //{
                        //    if (p.PropertyType == typeof(Tuple<string, string>))
                        //    {
                        //        Tuple<string, string> temp = (Tuple<string, string>)p.GetValue(t1.GpBhaPost, null);
                        //        if (p.Name == "P_L_HOLESEC")
                        //        {
                        //            MessageBox.Show("Before Adding from t1 to t2: " + p.Name + " => " + temp.Item1 + " => " + temp.Item2);
                        //        }


                        //    }

                        //}

                        //ModemGpPostObjects t2 = newNode.Tag as ModemGpPostObjects;
                        //foreach (var p in t2.GpBhaPost.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                        //{
                        //    if (p.PropertyType == typeof(Tuple<string, string>))
                        //    {
                        //        Tuple<string, string> temp = (Tuple<string, string>)p.GetValue(t2.GpBhaPost, null);
                        //        if (p.Name == "P_L_HOLESEC")
                        //        {
                        //            MessageBox.Show("When Adding from t1 to t2: " + p.Name + " => " + temp.Item1 + " => " + temp.Item2);
                        //        }


                        //    }

                        //}

                        break;

                    case looseName:
                        index = targetNode.Nodes.IndexOf(targetLooseNode);
                        break;

                    default:
                        break;
                }


                targetNode.Nodes[index].Nodes.Add(newNode);

                lastFocused = FocusedTreeview.none;
                treeView2.Update();
                treeView2.ExpandAll();
            }


        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Level == 0 || e.Node.Level == 1)
            {
                return;
            }
            else
            {
                btnAddSelected_Click(null, null);
                lastFocused = FocusedTreeview.treeview1;

            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Hide();
            Close();
            Dispose();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {

            targetModemNumber = txtTargetModem.Text;


            string modemViewUrl = HDocUtility.UrlModemView;
            ModemConnection mc = new ModemConnection(modemViewUrl + targetModemNumber);


            foreach (TreeNode node in treeView2.Nodes)
            {
                //MessageBox.Show(node.Name);

                foreach (TreeNode itemNode in node.Nodes)
                {
                    //MessageBox.Show(itemNode.Name);

                    foreach (TreeNode subNode in itemNode.Nodes)
                    {
                        //MessageBox.Show(subNode.Name);

                        switch (itemNode.Name)
                        {
                            case mwdName:
                                ModemMwdPostObjects mpo = new ModemMwdPostObjects();
                                mpo = subNode.Tag as ModemMwdPostObjects;

                                // Apply Main rename to MWD items
                                if (chkRenameMain.Checked && mpo != null)
                                {
                                    // Rename BHA-level fields
                                    mpo.MwdBhaPost.P_BHA_DESC = RenameMainInText(mpo.MwdBhaPost.P_BHA_DESC);
                                    mpo.MwdBhaPost.P_MWDDWD_ADD_INFO = RenameMainInText(mpo.MwdBhaPost.P_MWDDWD_ADD_INFO);
                                    
                                    // Rename component-level fields
                                    foreach (var compPost in mpo.MwdCompPostDict.Values)
                                    {
                                        compPost.P_DESCRIPTION = RenameMainInText(compPost.P_DESCRIPTION);
                                        compPost.P_COMMENTS = RenameMainInText(compPost.P_COMMENTS);
                                    }
                                }

                                ModemParameters mpm = new ModemParameters(mc.GetHtmlAsHdoc(), targetModemNumber);
                                ModemMwdInsert mim = new ModemMwdInsert(mpm, mpo, false);
                                break;

                            case ddName:
                                ModemDdPostObjects mdo = new ModemDdPostObjects();
                                mdo = subNode.Tag as ModemDdPostObjects;

                                ModemParameters mpd = new ModemParameters(mc.GetHtmlAsHdoc(), targetModemNumber);
                                ModemDdInsert mid = new ModemDdInsert(mpd, mdo, false);
                                break;

                            case gpName:
                                ModemGpPostObjects mdg = subNode.Tag as ModemGpPostObjects;

                                // Apply Main rename to GP items
                                if (chkRenameMain.Checked && mdg != null)
                                {
                                    // Rename BHA-level fields (Tuples - need to recreate with renamed Item2)
                                    mdg.GpBhaPost.P_GP_DESC = new Tuple<string, string>(
                                        mdg.GpBhaPost.P_GP_DESC.Item1,
                                        RenameMainInText(mdg.GpBhaPost.P_GP_DESC.Item2));
                                    mdg.GpBhaPost.P_GP_COMMENT = new Tuple<string, string>(
                                        mdg.GpBhaPost.P_GP_COMMENT.Item1,
                                        RenameMainInText(mdg.GpBhaPost.P_GP_COMMENT.Item2));
                                    
                                    // Rename component-level fields
                                    foreach (var compPost in mdg.GpCompPostDict.Values)
                                    {
                                        compPost.P_DESCRIPTION = RenameMainInText(compPost.P_DESCRIPTION);
                                        compPost.P_COMMENTS = RenameMainInText(compPost.P_COMMENTS);
                                    }
                                }

                                ModemParameters mpg = new ModemParameters(mc.GetHtmlAsHdoc(), targetModemNumber);
                                ModemGpInsert mig = new ModemGpInsert(mpg, mdg, false);
                                break;

                            case looseName:
                                ModemLoosePostObjects mdl = new ModemLoosePostObjects();
                                mdl = subNode.Tag as ModemLoosePostObjects;

                                ModemParameters mpl = new ModemParameters(mc.GetHtmlAsHdoc(), targetModemNumber);
                                ModemLooseInsert mil = new ModemLooseInsert(mpl, mdl, false);
                                break;

                            default:
                                break;
                        }


                    }
                }

            }


            CopyTreeNode(treeView1, treeView2, txtTargetModem.Text);
            TreeNode copiednode = treeView1.Nodes.Cast<TreeNode>().Where(n => n.Text == txtTargetModem.Text).FirstOrDefault();
            ColorNode(copiednode.Nodes, Color.Green);
            ModemHistory.Add(txtTargetModem.Text);
            RefreshModemAutoComplete();
            txtTargetModem.Text = "";
            treeView2.Nodes.Clear();



        }

        private void CopyTreeNode(TreeView treeview1CopyTo, TreeView treeview2CopyFrom, string nodeName)
        {
            //this copy assumes there is always one node in destination treeview
            
            TreeNode newTn = new TreeNode(nodeName);
            TreeNode sourceNode = treeview2CopyFrom.TopNode;
            CopyTreeNodeChilds(newTn, sourceNode);
            treeview1CopyTo.Nodes.Add(newTn);

        }

        private void CopyTreeNodeChilds(TreeNode parent, TreeNode willCopied)
        {
            TreeNode newTn;
            foreach (TreeNode tn in willCopied.Nodes)
            {
                newTn = new TreeNode(tn.Text);
                newTn.Name = tn.Text;
                newTn.Tag = tn.Tag;
                CopyTreeNodeChilds(newTn, tn);
                parent.Nodes.Add(newTn);
            }
        }

        private void ColorNode(TreeNodeCollection nodes, System.Drawing.Color Color)
        {
            
            foreach (TreeNode child in nodes)
            {
                child.ForeColor = Color;
                child.Parent.ForeColor = Color;
                if (child.Nodes != null && child.Nodes.Count > 0)
                    ColorNode(child.Nodes, Color);
            }
        }

        private void btnRemoveSelected_Click(object sender, EventArgs e)
        {
            if (lastFocused != FocusedTreeview.treeview2)
            {
                return;
            }

            TreeNode node = treeView2.SelectedNode;


            if (IsSelectableNode(node))
            {
                treeView2.Nodes.Remove(node);
                treeView2.Update();
                lastFocused = FocusedTreeview.none;
            }

        }

        //prevents selection of rootnode and secondnodes
        private bool IsSelectableNode(TreeNode node)
        {

            bool returnValue = false;

            if (node.Parent == null)
            {
                returnValue = false;
            }
            else if (node.Parent.Parent == null)
            {
                returnValue = false;
            }
            else
            {
                returnValue = true;
            }

            return returnValue;
        }

        private void treeView2_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Level == 0 || e.Node.Level == 1)
            {
                return;
            }
            else
            {
                btnRemoveSelected_Click(null, null);
                lastFocused = FocusedTreeview.treeview2;

            }
        }

        private void treeView1_Enter(object sender, EventArgs e)
        {
            lastFocused = FocusedTreeview.treeview1;
        }

        private void treeView2_Enter(object sender, EventArgs e)
        {
            lastFocused = FocusedTreeview.treeview2;
        }

        private void treeView1_Leave(object sender, EventArgs e)
        {
            if (!btnAddSelected.Focused)
            {
                lastFocused = FocusedTreeview.none;
            }
        }

        private void treeView2_Leave(object sender, EventArgs e)
        {
            if (!btnRemoveSelected.Focused)
            {
                lastFocused = FocusedTreeview.none;
            }
        }

        private string ReplaceQuotes(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return "No Desc";
            }
            else
            {
                return text.Replace("&quot;", "\"");
            }

        }

        private void txtTargetModem_TextChanged(object sender, EventArgs e)
        {
            if (txtTargetModem.Text.Length != 7)
            {
                btnAdd.Enabled = false;
            }
            else
            {
                btnAdd.Enabled = true;
            }
        }

        private void txtModemNo_TextChanged(object sender, EventArgs e)
        {
            if (txtModemNo.Text.Length != 7)
            {
                btnGetModem.Enabled = false;
            }
            else
            {
                btnGetModem.Enabled = true;
            }
        }

        private void txtModemNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtTargetModem_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void chkRenameMain_CheckedChanged(object sender, EventArgs e)
        {
            txtRenameMainTo.Enabled = chkRenameMain.Checked;
        }

        private void btnResetForm_Click(object sender, EventArgs e)
        {
            // Clear all textboxes
            txtModemNo.Text = "";
            txtTargetModem.Text = "";
            txtRenameMainTo.Text = "";
            
            // Uncheck checkbox
            chkRenameMain.Checked = false;
            
            // Clear both tree views
            treeView1.Nodes.Clear();
            treeView2.Nodes.Clear();
            
            // Re-initialize target nodes
            targetNode = new TreeNode();
            targetMwdNode = new TreeNode(mwdName);
            targetDdNode = new TreeNode(ddName);
            targetGpNode = new TreeNode(gpName);
            targetLooseNode = new TreeNode(looseName);
            
            // Rebuild target tree structure
            UpdateTargetNodes(treeView2, "default");
            
            // Reset buttons
            btnAdd.Enabled = false;
            btnGetModem.Enabled = false;
            
            // Reset focused state
            lastFocused = FocusedTreeview.none;
        }

        private string RenameMainInText(string text)
        {
            if (!chkRenameMain.Checked || string.IsNullOrWhiteSpace(txtRenameMainTo.Text))
                return text;

            if (string.IsNullOrEmpty(text))
                return text;

            // Case-insensitive replacement of "Main" with custom text
            return System.Text.RegularExpressions.Regex.Replace(
                text, 
                "Main", 
                txtRenameMainTo.Text.Trim(), 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );
        }

        private void btnFileManager_Click(object sender, EventArgs e)
        {
            AttachmentForm attachmentForm = new AttachmentForm(txtModemNo.Text.Trim());
            attachmentForm.ShowDialog();
            RefreshModemAutoComplete();
        }

        // ═══════════════════════════════════════════════════════════════════
        //  MODEM SHIFTER TAB — business logic
        // ═══════════════════════════════════════════════════════════════════

        private const string ModemViewUrlBase = "http://norwayappsprd.corp.halliburton.com/pls/log_web/mobssus_vieword$order_mc.QueryViewByKey?P_SSORD_ID=";

        // Rig → customer mapping & rig lists per customer
        private static readonly Dictionary<string, string[]> _rigsByCustomer = new Dictionary<string, string[]>
        {
            { "AkerBP",         new[] { "SC8", "Integrator", "Invincible", "Deepsea Nordkap", "Stavanger" } },
            { "ConocoPhillips", new[] { "West Linus", "Elara" } },
            { "Equinor",        new[] { "Grane", "Prospector", "Promoter", "Enabler", "Heidrun", "Njord A", "Snorre A", "Snorre B" } },
            { "Vår Energi",     new[] { "Ringhorne", "Pioneer", "Prospector" } }
        };

        // Abbreviation map for raw P_SHIPTO_1 text returned by HTTP fetch
        private static readonly Dictionary<string, string> _shipToAbbr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "KSU",          "KSU" },
            { "Kristiansund", "KSU" },
            { "Dusavik",      "DUS" },
            { "ASCO",         "ASC" },
            { "Sandnessjøen", "SSJ" },
            { "Sandnesjøen",  "SSJ" },
            { "Florø",        "FLO" },
            { "Floro",        "FLO" },
            { "Hammerfest",   "HMF" },
        };

        private string ConfigPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "kabal.config");

        private List<GantModem> _loadedModems = new List<GantModem>();

        // ── Credential persistence ───────────────────────────────────────

        private void LoadKabalCredentials()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    var lines = File.ReadAllLines(ConfigPath);
                    if (lines.Length >= 1) txtKabalUser.Text = lines[0];
                    if (lines.Length >= 2)
                    {
                        var encBytes = Convert.FromBase64String(lines[1]);
                        var plainBytes = System.Security.Cryptography.ProtectedData.Unprotect(
                            encBytes, null, System.Security.Cryptography.DataProtectionScope.CurrentUser);
                        txtKabalPass.Text = Encoding.UTF8.GetString(plainBytes);
                    }
                }
            }
            catch { /* first run or corrupted config — ignore */ }
        }

        private void SaveKabalCredentials()
        {
            try
            {
                var plainBytes = Encoding.UTF8.GetBytes(txtKabalPass.Text);
                var encBytes = System.Security.Cryptography.ProtectedData.Protect(
                    plainBytes, null, System.Security.Cryptography.DataProtectionScope.CurrentUser);
                File.WriteAllLines(ConfigPath, new[] { txtKabalUser.Text, Convert.ToBase64String(encBytes) });
            }
            catch { /* non-critical */ }
        }

        private void MergeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveKabalCredentials();
            try { _apiServer?.Dispose(); } catch { }
        }

        // ── UI event handlers ────────────────────────────────────────────

        private void cmbKabalCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbKabalRig.Items.Clear();
            cmbKabalRig.Items.Add("select rig");
            var customer = cmbKabalCustomer.Text;
            if (_rigsByCustomer.ContainsKey(customer))
            {
                foreach (var rig in _rigsByCustomer[customer])
                    cmbKabalRig.Items.Add(rig);
            }
            cmbKabalRig.SelectedIndex = 0;
        }

        private void txtKabalShiftDays_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '-')
                e.Handled = true;
        }

        private void dgvKabal_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // ── Data cells: custom paint Customer column with Kabal date overlay ──
            if (e.RowIndex >= 0)
            {
                if (dgvKabal.Columns.Contains("Customer") &&
                    e.ColumnIndex == dgvKabal.Columns["Customer"].Index)
                {
                    var cell = dgvKabal.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    var kabalInfo = cell.Tag as KabalDateInfo;
                    if (kabalInfo != null)
                    {
                        e.PaintBackground(e.ClipBounds, true);

                        var gantDate = cell.Value?.ToString() ?? "";
                        var kabalDate = kabalInfo.KabalDate;
                        var kabalColor = kabalInfo.Matches ? Color.Green : Color.DarkOrange;

                        // Draw Gant date in normal color
                        var gantDisplay = gantDate + "  ";
                        var gantSize = TextRenderer.MeasureText(e.Graphics, gantDisplay, e.CellStyle.Font);
                        TextRenderer.DrawText(e.Graphics, gantDisplay, e.CellStyle.Font,
                            new Rectangle(e.CellBounds.Left + 2, e.CellBounds.Top, gantSize.Width, e.CellBounds.Height),
                            e.CellStyle.ForeColor,
                            TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

                        // Draw Kabal date in green (match) or orange (mismatch)
                        using (var kabalFont = new Font(e.CellStyle.Font, FontStyle.Bold))
                        {
                            TextRenderer.DrawText(e.Graphics, kabalDate, kabalFont,
                                new Rectangle(e.CellBounds.Left + 2 + gantSize.Width, e.CellBounds.Top,
                                    e.CellBounds.Width - gantSize.Width - 4, e.CellBounds.Height),
                                kabalColor,
                                TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
                        }

                        e.Handled = true;
                    }
                }
                return; // data cells handled (or default paint)
            }

            // ── Header row (e.RowIndex == -1) ──

            // Select column — draw checkbox
            if (dgvKabal.Columns.Contains("Select") &&
                e.ColumnIndex == dgvKabal.Columns["Select"].Index)
            {
                e.PaintBackground(e.ClipBounds, false);
                var state = _headerChecked ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal;
                var sz = CheckBoxRenderer.GetGlyphSize(e.Graphics, state);
                var pt = new System.Drawing.Point(
                    e.CellBounds.Left + (e.CellBounds.Width  - sz.Width)  / 2,
                    e.CellBounds.Top  + (e.CellBounds.Height - sz.Height) / 2);
                CheckBoxRenderer.DrawCheckBox(e.Graphics, pt, state);
                e.Handled = true;
                return;
            }

            // Loadout / Customer columns — draw padlock icon in header
            bool isLoadout  = dgvKabal.Columns.Contains("Loadout")  && e.ColumnIndex == dgvKabal.Columns["Loadout"].Index;
            bool isCustomer = dgvKabal.Columns.Contains("Customer") && e.ColumnIndex == dgvKabal.Columns["Customer"].Index;
            if (isLoadout || isCustomer)
            {
                bool locked = isLoadout ? _loadoutLocked : _etaLocked;
                e.PaintBackground(e.ClipBounds, false);

                string text = e.Value?.ToString() ?? "";
                string icon = locked ? "\U0001F512 " : "\U0001F513 ";
                var display = icon + text;

                TextRenderer.DrawText(e.Graphics, display, e.CellStyle.Font,
                    e.CellBounds, e.CellStyle.ForeColor,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                e.Handled = true;
            }
        }

        private void dgvKabal_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Select column — toggle all checkboxes
            if (dgvKabal.Columns.Contains("Select") &&
                e.ColumnIndex == dgvKabal.Columns["Select"].Index)
            {
                _headerChecked = !_headerChecked;
                foreach (DataGridViewRow row in dgvKabal.Rows)
                {
                    if (row.Cells["Select"].ReadOnly) continue;
                    row.Cells["Select"].Value = _headerChecked;
                }
                dgvKabal.RefreshEdit();
                dgvKabal.InvalidateColumn(e.ColumnIndex);
                return;
            }

            // Loadout / Customer columns: click within the padlock icon zone (leftmost 22px)
            // toggles the lock; clicking anywhere else lets the grid sort normally.
            bool isLoadoutCol  = dgvKabal.Columns.Contains("Loadout")  && e.ColumnIndex == dgvKabal.Columns["Loadout"].Index;
            bool isCustomerCol = dgvKabal.Columns.Contains("Customer") && e.ColumnIndex == dgvKabal.Columns["Customer"].Index;
            if (isLoadoutCol || isCustomerCol)
            {
                if (e.X <= 22)  // within the padlock icon — toggle lock, suppress sort
                {
                    if (isLoadoutCol)
                    {
                        _loadoutLocked = !_loadoutLocked;
                        dgvKabal.InvalidateColumn(e.ColumnIndex);
                        lblKabalStatus.Text = _loadoutLocked ? "Loadout Date locked" : "Loadout Date unlocked";
                    }
                    else
                    {
                        _etaLocked = !_etaLocked;
                        dgvKabal.InvalidateColumn(e.ColumnIndex);
                        lblKabalStatus.Text = _etaLocked ? "Deliver To Customer locked" : "Deliver To Customer unlocked";
                    }
                    // Suppress the default sort by cancelling SortOrder on the column
                    dgvKabal.Columns[e.ColumnIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dgvKabal.Columns[e.ColumnIndex].SortMode = DataGridViewColumnSortMode.Automatic;
                }
                // else: fall through — grid handles sort naturally
            }
        }

        private void dgvKabal_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvKabal.CurrentCell is DataGridViewCheckBoxCell && !dgvKabal.CurrentCell.ReadOnly)
                dgvKabal.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private List<int> GetCheckedIds()
        {
            var ids = new List<int>();
            if (!dgvKabal.Columns.Contains("Select")) return ids;
            foreach (DataGridViewRow row in dgvKabal.Rows)
            {
                if (true.Equals(row.Cells["Select"].Value))
                {
                    int id;
                    if (int.TryParse(row.Cells["MobId"].Value?.ToString(), out id))
                        ids.Add(id);
                }
            }
            return ids;
        }

        private void dgvKabal_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            // MobId link column
            if (dgvKabal.Columns.Contains("MobId") &&
                e.ColumnIndex == dgvKabal.Columns["MobId"].Index)
            {
                var modemId = dgvKabal.Rows[e.RowIndex].Cells["MobId"].Value?.ToString();
                if (!string.IsNullOrEmpty(modemId))
                    Process.Start(ModemViewUrlBase + modemId);
            }
        }

        // ── Copy to Clipboard (HTML with links) ─────────────────────────

        private void btnKabalCopyClipboard_Click(object sender, EventArgs e)
        {
            if (dgvKabal.Rows.Count == 0)
            {
                MessageBox.Show("No modems to copy.", "Copy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Use highlighted (selected) rows; if all selected, include header
            var selected = dgvKabal.SelectedRows;
            bool allSelected = selected.Count >= dgvKabal.Rows.Count;
            var rows = new List<DataGridViewRow>();
            if (allSelected)
            {
                foreach (DataGridViewRow row in dgvKabal.Rows)
                    rows.Add(row);
            }
            else
            {
                foreach (DataGridViewRow row in selected)
                    rows.Add(row);
                rows.Sort((a, b) => a.Index.CompareTo(b.Index));
            }

            // Build HTML table with linked MobIDs, font size 10
            string[] headers = { "Mob ID", "Ship To", "Loadout Date", "Deliver To Customer", "Well Number", "Well Section", "BHA" };
            var html = new StringBuilder();
            html.Append("<table style=\"font-size:10pt;border-collapse:collapse;\">");
            if (allSelected)
            {
                html.Append("<thead><tr>");
                foreach (var h in headers)
                    html.Append("<th style=\"font-size:10pt;\">").Append(HtmlEnc(h)).Append("</th>");
                html.Append("</tr></thead>");
            }
            html.Append("<tbody>");

            foreach (DataGridViewRow row in rows)
            {
                var gm = row.Tag as GantModem;
                if (gm == null) continue;

                html.Append("<tr>");

                // Column 1: linked MobId
                var modemUrl = ModemViewUrlBase + gm.Id;
                html.Append("<td style=\"font-size:10pt;\"><a href=\"").Append(HtmlEnc(modemUrl)).Append("\">");
                html.Append(HtmlEnc(gm.Id.ToString())).Append("</a></td>");

                // Remaining columns
                html.Append("<td style=\"font-size:10pt;\">").Append(HtmlEnc(row.Cells["ShipTo"].Value?.ToString() ?? "")).Append("</td>");
                html.Append("<td style=\"font-size:10pt;\">").Append(HtmlEnc(row.Cells["Loadout"].Value?.ToString() ?? "")).Append("</td>");
                html.Append("<td style=\"font-size:10pt;\">").Append(HtmlEnc(row.Cells["Customer"].Value?.ToString() ?? "")).Append("</td>");
                html.Append("<td style=\"font-size:10pt;\">").Append(HtmlEnc(row.Cells["Well"].Value?.ToString() ?? "")).Append("</td>");
                html.Append("<td style=\"font-size:10pt;\">").Append(HtmlEnc(row.Cells["Section"].Value?.ToString() ?? "")).Append("</td>");
                html.Append("<td style=\"font-size:10pt;\">").Append(HtmlEnc(row.Cells["Bha"].Value?.ToString() ?? "")).Append("</td>");

                html.Append("</tr>");
            }
            html.Append("</tbody></table>");

            // Build CF_HTML clipboard format
            var htmlFragment = html.ToString();
            var cfHtml = BuildCfHtml(htmlFragment);

            var dataObj = new DataObject();
            dataObj.SetData(DataFormats.Html, cfHtml);
            // Plain text fallback (tab-delimited)
            var plain = new StringBuilder();
            if (allSelected)
                plain.AppendLine(string.Join("\t", headers));
            foreach (DataGridViewRow row in rows)
            {
                var gm = row.Tag as GantModem;
                if (gm == null) continue;
                plain.AppendLine(string.Join("\t",
                    gm.Id.ToString(),
                    row.Cells["ShipTo"].Value?.ToString() ?? "",
                    row.Cells["Loadout"].Value?.ToString() ?? "",
                    row.Cells["Customer"].Value?.ToString() ?? "",
                    row.Cells["Well"].Value?.ToString() ?? "",
                    row.Cells["Section"].Value?.ToString() ?? "",
                    row.Cells["Bha"].Value?.ToString() ?? ""));
            }
            dataObj.SetData(DataFormats.UnicodeText, plain.ToString());
            Clipboard.SetDataObject(dataObj, true);

            lblKabalStatus.Text = $"Copied {rows.Count} modem(s) to clipboard.";
        }

        private static string HtmlEnc(string s) => System.Net.WebUtility.HtmlEncode(s ?? "");

        /// <summary>
        /// Wraps an HTML fragment in the CF_HTML clipboard header format.
        /// </summary>
        private static string BuildCfHtml(string htmlFragment)
        {
            // CF_HTML requires UTF-8 byte offsets in the header
            const string markerBegin = "<!--StartFragment-->";
            const string markerEnd   = "<!--EndFragment-->";
            string pre  = "<html><body>" + markerBegin;
            string post = markerEnd + "</body></html>";

            // Header template with placeholder lengths (10-digit zero-padded)
            string header =
                "Version:0.9\r\n" +
                "StartHTML:{0:D10}\r\n" +
                "EndHTML:{1:D10}\r\n" +
                "StartFragment:{2:D10}\r\n" +
                "EndFragment:{3:D10}\r\n";

            int headerLen = string.Format(header, 0, 0, 0, 0).Length;
            int startHtml = headerLen;
            int startFragment = headerLen + Encoding.UTF8.GetByteCount(pre);
            int endFragment = startFragment + Encoding.UTF8.GetByteCount(htmlFragment);
            int endHtml = endFragment + Encoding.UTF8.GetByteCount(post);

            return string.Format(header, startHtml, endHtml, startFragment, endFragment)
                   + pre + htmlFragment + post;
        }

        // ── Date business rules ──────────────────────────────────────────

        /// <summary>
        /// Snap forward to the next Mon/Wed/Fri on or after the given date.
        /// </summary>
        private static DateTime SnapToBoatDayForward(DateTime date)
        {
            while (date.DayOfWeek != DayOfWeek.Monday &&
                   date.DayOfWeek != DayOfWeek.Wednesday &&
                   date.DayOfWeek != DayOfWeek.Friday)
                date = date.AddDays(1);
            return date;
        }

        private static DateTime AdvanceToNextBoatDay(DateTime date)
        {
            return SnapToBoatDayForward(date.AddDays(1));
        }

        /// <summary>
        /// Find the earliest boat-day delivery where LoadoutDate >= today+7 days,
        /// starting from the later of startDate and today.
        /// </summary>
        private static void CalcNextDelivery(DateTime startDate, string rigName,
            out DateTime deliver, out DateTime loadout)
        {
            var minLoadout = DateTime.Today.AddDays(7);
            var from = startDate > DateTime.Today ? startDate : DateTime.Today;
            var d = SnapToBoatDayForward(from);

            for (int i = 0; i < 104; i++)
            {
                var lo = CalcLoadoutDate(d, rigName);
                if (lo >= minLoadout)
                {
                    deliver = d;
                    loadout = lo;
                    return;
                }
                d = AdvanceToNextBoatDay(d);
            }

            loadout = CalcLoadoutDate(d, rigName);
            deliver = d;
        }

        /// <summary>
        /// Calculate H_LOADOUT_DATE from H_DATE_ETA based on rig-specific rules.
        /// ETA is already snapped to Mon/Wed/Fri at this point.
        /// rigName may be the full Gant name (e.g. "COSL Promoter", "Transocean Enabler").
        /// </summary>
        private static DateTime CalcLoadoutDate(DateTime eta, string rigName)
        {
            var dow = eta.DayOfWeek;
            var rig = rigName ?? "";

            // Njord A, Heidrun: Mon→-3, Wed/Fri→-2
            if (Contains(rig, "Njord") || Contains(rig, "Heidrun"))
                return dow == DayOfWeek.Monday ? eta.AddDays(-3) : eta.AddDays(-2);

            // Snorre A/B, Promoter: Mon→-3, Wed/Fri→-1
            if (Contains(rig, "Snorre") || Contains(rig, "Promoter"))
                return dow == DayOfWeek.Monday ? eta.AddDays(-3) : eta.AddDays(-1);

            // Enabler, Prospector: always -7
            if (Contains(rig, "Enabler") || Contains(rig, "Prospector"))
                return eta.AddDays(-7);

            // Default: Mon→-3, Wed/Fri→-2
            return dow == DayOfWeek.Monday ? eta.AddDays(-3) : eta.AddDays(-2);
        }

        private static bool Contains(string source, string value)
        {
            return source.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string FormatDateDMY_HM(DateTime d)
        {
            return d.ToString("dd.MM.yyyy") + " 10:00";
        }

        private static string FormatDateDMY(DateTime d)
        {
            return d.ToString("dd.MM.yyyy");
        }

        /// <summary>Strips time part from "DD.MM.YYYY HH:mm" for grid display.</summary>
        private static string TruncateTime(string dateStr)
        {
            if (string.IsNullOrWhiteSpace(dateStr)) return "";
            var sp = dateStr.IndexOf(' ');
            return sp > 0 ? dateStr.Substring(0, sp) : dateStr;
        }

        private static string ShortenShipTo(string shipTo)
        {
            if (string.IsNullOrWhiteSpace(shipTo)) return "";
            foreach (var kv in _shipToAbbr)
                if (shipTo.IndexOf(kv.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                    return kv.Value;
            return shipTo; // already abbreviated or unknown — return as-is
        }

        // ── Load Modems button ───────────────────────────────────────────

        private async void btnKabalLoadModems_Click(object sender, EventArgs e)
        {
            var customer = cmbKabalCustomer.Text;
            var rig = cmbKabalRig.Text;
            if (customer == "select customer")
            {
                MessageBox.Show("Please select a customer.", "Modem Shifter", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnKabalLoadModems.Enabled = false;
            lblKabalStatus.Text = "Loading modems from Gant...";

            try
            {
                var rigFilter = rig == "select rig" ? null : rig;
                _loadedModems = await GantClient.FetchModemsAsync(customer, rigFilter, dtpKabalStartDate.Value);
                BuildGrid();
                lblKabalStatus.Text = $"Loaded {_loadedModems.Count} modems.";
            }
            catch (Exception ex)
            {
                lblKabalStatus.Text = "Error: " + ex.Message;
                MessageBox.Show("Failed to load modems: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnKabalLoadModems.Enabled = true;
            }
        }

        private void BuildGrid()
        {
            dgvKabal.Columns.Clear();
            dgvKabal.Rows.Clear();

            // Columns: Select, Mob ID, Ship To, Loadout Date, Deliver To Customer, Well Number, Well Section, BHA
            var colSelect    = new DataGridViewCheckBoxColumn { Name = "Select",    HeaderText = "",                    Width = 34,  ReadOnly = false };
            var colMobId     = new DataGridViewLinkColumn    { Name = "MobId",      HeaderText = "Mob ID",              Width = 80   };
            var colShipTo    = new DataGridViewTextBoxColumn { Name = "ShipTo",     HeaderText = "Ship To",             Width = 130  };
            var colLoadout   = new DataGridViewTextBoxColumn { Name = "Loadout",    HeaderText = "Loadout Date",        Width = 100  };
            var colCustomer  = new DataGridViewTextBoxColumn { Name = "Customer",   HeaderText = "Deliver To Customer", Width = 160  };
            var colWell      = new DataGridViewTextBoxColumn { Name = "Well",        HeaderText = "Well Number",         Width = 160  };
            var colSection   = new DataGridViewTextBoxColumn { Name = "Section",    HeaderText = "Well Section",        Width = 100  };
            var colBha       = new DataGridViewTextBoxColumn { Name = "Bha",        HeaderText = "BHA",                 Width = 220  };

            _headerChecked = true;
            dgvKabal.Columns.AddRange(new DataGridViewColumn[]
                { colSelect, colMobId, colShipTo, colLoadout, colCustomer, colWell, colSection, colBha });

            foreach (var m in _loadedModems)
            {
                bool isReady = !string.IsNullOrEmpty(m.ModemStatus) &&
                    m.ModemStatus.IndexOf("Ready", StringComparison.OrdinalIgnoreCase) >= 0;

                var rowIdx = dgvKabal.Rows.Add(
                    !isReady,
                    m.Id.ToString(),
                    ShortenShipTo(m.PShipTo1 ?? ""),
                    TruncateTime(m.LoadoutDate),
                    TruncateTime(m.DateEta),
                    m.WellName ?? "",
                    m.WellSection ?? "",
                    m.GantText ?? ""
                );

                dgvKabal.Rows[rowIdx].Tag = m;  // store full GantModem for clipboard

                if (isReady)
                {
                    dgvKabal.Rows[rowIdx].Cells["Select"].ReadOnly = true;
                    dgvKabal.Rows[rowIdx].DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                    dgvKabal.Rows[rowIdx].DefaultCellStyle.ForeColor = Color.Gray;
                }

                // Color code Mob ID column based on modem status
                var status = m.ModemStatus ?? "";
                if (!isReady && status.IndexOf("activated", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (status.IndexOf("not", StringComparison.OrdinalIgnoreCase) >= 0)
                        dgvKabal.Rows[rowIdx].Cells["MobId"].Style.BackColor = Color.FromArgb(255, 200, 200); // Light red
                    else
                        dgvKabal.Rows[rowIdx].Cells["MobId"].Style.BackColor = Color.FromArgb(255, 255, 150); // Yellow
                }
            }
        }

        // ── Shift Dates button (batch shift by N days) ───────────────────

        private async void btnKabalShiftDates_Click(object sender, EventArgs e)
        {
            int days;
            if (!int.TryParse(txtKabalShiftDays.Text, out days) || days == 0)
            {
                MessageBox.Show("Enter a non-zero number of shift days.", "Modem Shifter", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selected = GetCheckedIds();

            if (selected.Count == 0)
            {
                MessageBox.Show("No modems ticked.", "Modem Shifter", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"Shift {selected.Count} modem(s) by {days} day(s)?",
                "Confirm Shift", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            btnKabalShiftDates.Enabled = false;
            lblKabalStatus.Text = "Shifting dates...";
            int ok = 0, fail = 0;

            foreach (var modemId in selected)
            {
                try
                {
                    var result = await GantClient.ShiftModemDatesAsync(modemId, days,
                        shiftLoadout: !_loadoutLocked, shiftEta: !_etaLocked,
                        shiftDateLoad: !_loadoutLocked);
                    if (result.Success)
                    {
                        ok++;
                        // Update grid row with new dates and green checkmark
                        foreach (DataGridViewRow row in dgvKabal.Rows)
                        {
                            if (row.Cells["MobId"].Value?.ToString() == modemId.ToString())
                            {
                                if (!_loadoutLocked)
                                {
                                    row.Cells["Loadout"].Value  = "\u2713 " + result.NewLoadoutDate;
                                    row.Cells["Loadout"].Style.ForeColor  = Color.Green;
                                }
                                if (!_etaLocked)
                                {
                                    row.Cells["Customer"].Value = "\u2713 " + result.NewDateEta;
                                    row.Cells["Customer"].Style.ForeColor = Color.Green;
                                }
                                row.DefaultCellStyle.BackColor = Color.FromArgb(232, 255, 232);
                                break;
                            }
                        }
                    }
                    else
                    {
                        fail++;
                        lblKabalStatus.Text = $"Failed #{modemId}: {result.Error}";
                    }
                }
                catch (Exception ex) { fail++; lblKabalStatus.Text = $"Exception #{modemId}: {ex.Message}"; }

                lblKabalStatus.Text = $"Shifting... {ok + fail}/{selected.Count}";
            }

            lblKabalStatus.Text = $"Shift complete. Success: {ok}, Failed: {fail}";
            btnKabalShiftDates.Enabled = true;
        }

        // ── Sync with Kabal button ───────────────────────────────────────

        private static string NormalizeDate(string dateStr)
        {
            if (string.IsNullOrWhiteSpace(dateStr)) return "";
            // Strip leading day-of-week prefix like "Wed ", "Mon ", "Fri " etc.
            var cleaned = Regex.Replace(dateStr.Trim(), @"^[A-Za-z]{2,3}\s+", "");
            DateTime dt;
            string[] formats = {
                "dd.MM.yyyy", "dd/MM/yyyy", "yyyy-MM-dd", "dd-MM-yyyy",
                "dd.MM.yyyy HH:mm",
                "dd-MM-yy", "dd/MM/yy", "dd.MM.yy" // 2-digit year (Kabal uses e.g. 25-03-26)
            };
            if (DateTime.TryParseExact(cleaned, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return dt.ToString("dd.MM.yyyy");
            return cleaned;
        }

        private async void btnKabalSyncKabal_Click(object sender, EventArgs e)
        {
            var customer = cmbKabalCustomer.Text;
            var rig = cmbKabalRig.Text;
            if (customer == "select customer" || rig == "select rig")
            {
                MessageBox.Show("Please select a customer and rig.", "Modem Shifter", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtKabalUser.Text) || string.IsNullOrWhiteSpace(txtKabalPass.Text))
            {
                MessageBox.Show("Enter Kabal username and password.", "Modem Shifter", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (dgvKabal.Rows.Count == 0)
            {
                MessageBox.Show("Load modems first before syncing with Kabal.", "Modem Shifter", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnKabalSyncKabal.Enabled = false;
            lblKabalStatus.Text = "Starting Kabal scraper...";

            // Compute date range from grid (earliest to latest Deliver To Customer)
            DateTime? gridDateFrom = null, gridDateTo = null;
            foreach (DataGridViewRow r in dgvKabal.Rows)
            {
                var dateStr = r.Cells["Customer"].Value?.ToString();
                DateTime dt;
                if (DateTime.TryParseExact(dateStr, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    if (gridDateFrom == null || dt < gridDateFrom) gridDateFrom = dt;
                    if (gridDateTo == null || dt > gridDateTo) gridDateTo = dt;
                }
            }

            // Scrape Kabal directly using Selenium (no Node.js needed)
            var result = await KabalScraperClient.ScrapeAsync(
                customer, rig, txtKabalUser.Text, txtKabalPass.Text,
                chkKabalHeadless.Checked, dryRun: true,
                onStatus: msg => BeginInvoke((Action)(() => lblKabalStatus.Text = msg)),
                dateFrom: gridDateFrom, dateTo: gridDateTo);

            if (!result.Success)
            {
                lblKabalStatus.Text = "Scrape failed: " + result.Error;
                var logPath = ScrapeLog.LogPath;
                var msg = "Scraping failed: " + result.Error;
                if (logPath != null)
                    msg += $"\n\nFull log saved to:\n{logPath}\n\nOpen the log file?";
                var btn = logPath != null ? MessageBoxButtons.YesNo : MessageBoxButtons.OK;
                var dlg = MessageBox.Show(msg, "Error", btn, MessageBoxIcon.Error);
                if (dlg == DialogResult.Yes && logPath != null)
                    System.Diagnostics.Process.Start(logPath);
                btnKabalSyncKabal.Enabled = true;
                return;
            }

            // Annotate existing grid with Kabal dates
            int matched = 0;
            foreach (DataGridViewRow row in dgvKabal.Rows)
            {
                var modemId = row.Cells["MobId"].Value?.ToString();
                var kabalMatch = result.Modems.FirstOrDefault(km =>
                    km.ModemNumber == modemId || km.PackageName?.Contains(modemId) == true);

                if (kabalMatch != null && !string.IsNullOrEmpty(kabalMatch.ShippingDate))
                {
                    matched++;
                    var gantDate = row.Cells["Customer"].Value?.ToString()?.Trim() ?? "";
                    var kabalDate = NormalizeDate(kabalMatch.ShippingDate);
                    bool datesMatch = string.Equals(gantDate, kabalDate, StringComparison.OrdinalIgnoreCase);
                    row.Cells["Customer"].Tag = new KabalDateInfo { KabalDate = kabalDate, Matches = datesMatch };
                }
            }

            dgvKabal.Refresh();

            lblKabalStatus.Text = $"Sync done. {matched}/{dgvKabal.Rows.Count} matched ({result.Modems.Count} Kabal records).";
            btnKabalSyncKabal.Enabled = true;
        }
    }

    //public class TreeViewTagObj
    //{
    //    public Dictionary<int, MwdCompPosts> ToolPart { get; set; }
    //    public Dictionary<int, MwdSoftPosts> SoftPart { get; set; }
    //}

    public enum FocusedTreeview
    {
        treeview1,
        treeview2,
        none
    }
}
