using ModemWebUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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




        public MergeForm()
        {
            InitializeComponent();
            UpdateTargetNodes(treeView2, "default");
            btnAdd.Enabled = false;
            btnGetModem.Enabled = false;


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
            txtTargetModem.Text = modemNo;

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
            AttachmentForm attachmentForm = new AttachmentForm();
            attachmentForm.ShowDialog();
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
