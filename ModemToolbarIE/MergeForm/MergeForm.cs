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

namespace ModemToolbarIE.MergeForm
{
    public partial class MergeForm : Form
    {
        private string targetModem;
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



        private Toolbar engine;
        public MergeForm(Toolbar engine)
        {
            InitializeComponent();
            this.engine = engine;
            targetModem = engine.ModemNoEngine;
            txtTargetModem.Text = targetModem;
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

            treeView2.Nodes.Add(targetNode);
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



            //foreach (string item in mp.GpId)
            //{

            //    ModemConnection mcTemp = new ModemConnection(gpEditViewUrl + item);
            //    GpBhaParameters mbp = new GpBhaParameters(mcTemp.GetHtmlAsHdoc());

            //    //foreach (var p in mbp.GpBhaPosts.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            //    //{
            //    //    if (p.PropertyType == typeof(Tuple<string, string>))
            //    //    {
            //    //        Tuple<string, string> temp = (Tuple<string, string>)p.GetValue(mbp.GpBhaPosts, null);
            //    //        if (p.Name == "P_L_HOLESEC")
            //    //        {
            //    //            MessageBox.Show("Original MBP: " + p.Name + " => " + temp.Item1 + " => " + temp.Item2);
            //    //        }


            //    //    }

            //    //}

            //    TreeNode tn = new TreeNode(ReplaceQuotes(mbp.GpBhaPosts.P_GP_DESC.Item2));
            //    ModemGpPostObjects tagObj = new ModemGpPostObjects();

            //    tagObj.GpCompPostDict = mbp.GpBhaCompPost;
            //    tagObj.GpBhaPost = mbp.GpBhaPosts;
            //    tn.Tag = tagObj;

            //    //foreach (var p in tagObj.GpBhaPost.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            //    //{
            //    //    if (p.PropertyType == typeof(Tuple<string, string>))
            //    //    {
            //    //        Tuple<string, string> temp = (Tuple<string, string>)p.GetValue(tagObj.GpBhaPost, null);
            //    //        if (p.Name == "P_L_HOLESEC")
            //    //        {
            //    //            MessageBox.Show("When getting from modem: "+ p.Name + " => " + temp.Item1 + " => " + temp.Item2);
            //    //        }


            //    //    }

            //    //}

            //    gpNode.Nodes.Add(tn);
            //    if (!mainNode.Nodes.Contains(gpNode))
            //    {
            //        mainNode.Nodes.Add(gpNode);
            //    }

            //}


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

                int index = 0;

                switch (node.Parent.Name)
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
            string modemViewUrl = HDocUtility.UrlModemView;
            ModemConnection mc = new ModemConnection(modemViewUrl + targetModem);


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

                                ModemParameters mpm = new ModemParameters(mc.GetHtmlAsHdoc(), targetModem);
                                ModemMwdInsert mim = new ModemMwdInsert(mpm, mpo, false);
                                break;

                            case ddName:
                                ModemDdPostObjects mdo = new ModemDdPostObjects();
                                mdo = subNode.Tag as ModemDdPostObjects;

                                ModemParameters mpd = new ModemParameters(mc.GetHtmlAsHdoc(), targetModem);
                                ModemDdInsert mid = new ModemDdInsert(mpd, mdo, false);
                                break;

                            case gpName:
                                ModemGpPostObjects mdg = new ModemGpPostObjects();
                                mdg = subNode.Tag as ModemGpPostObjects;

                                //foreach (var p in mdg.GpBhaPost.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                                //{
                                //    if (p.PropertyType == typeof(Tuple<string, string>))
                                //    {
                                //        Tuple<string, string> temp = (Tuple<string, string>)p.GetValue(mdg.GpBhaPost, null);
                                //        if (p.Name == "P_L_HOLESEC")
                                //        {
                                //            MessageBox.Show("When adding to modem: " + p.Name + " => " + temp.Item1 + " => " + temp.Item2);
                                //        }
                                        

                                //    }

                                //}

                                ModemParameters mpg = new ModemParameters(mc.GetHtmlAsHdoc(), targetModem);
                                ModemGpInsert mig = new ModemGpInsert(mpg, mdg, false);
                                break;

                            case looseName:
                                ModemLoosePostObjects mdl = new ModemLoosePostObjects();
                                mdl = subNode.Tag as ModemLoosePostObjects;

                                ModemParameters mpl = new ModemParameters(mc.GetHtmlAsHdoc(), targetModem);
                                ModemLooseInsert mil = new ModemLooseInsert(mpl, mdl, false);
                                break;

                            default:
                                break;
                        }


                    }
                }

            }

            treeView2.Nodes.Clear();
            btnCancel_Click(null, null);
            engine.SmartNavigate(HDocUtility.UrlModemEdit + targetModem);


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
