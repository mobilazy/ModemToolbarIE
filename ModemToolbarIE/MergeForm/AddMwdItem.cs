using ModemWebUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ModemToolbarIE.MergeForm
{
    public partial class AddMwdItem : Form
    {
        private Toolbar engine;
        private ModemMwdPostObjects linkObjects;
        BindingList<MwdCompPosts> compBinding = new BindingList<MwdCompPosts>();
        BindingList<MwdSoftPosts> softBinding = new BindingList<MwdSoftPosts>();
        private bool dataGridChanged = false;

        private List<string> listOfSoftsensor = new List<string>();
        private List<string> listOfThread = new List<string>();
        private List<string> listOfTorque = new List<string>();

        private MwdHeader mwdHeader = new MwdHeader();

        private bool withHeader = false;

        private string targetModem;

        public AddMwdItem(Toolbar _engine, ModemMwdPostObjects _linkObject, bool _withHeader = false)
        {
            engine = _engine;

            linkObjects = _linkObject;
            withHeader = _withHeader;
            targetModem = engine.ModemNoEngine;

            if (_withHeader)
            {
                engine.SmartNavigate(HDocUtility.UrlModemEdit + targetModem);
                this.Focus();
            }
            

            string pathListOfSoftsensor = engine.dataFolder + @"\listofsoftsensor.dat";
            string pathListOfThread = engine.dataFolder + @"\listofthread.dat";
            string pathListOfTorque = engine.dataFolder + @"\listoftorque.dat";

            listOfSoftsensor = File.ReadAllLines(pathListOfSoftsensor).ToList();
            listOfThread = File.ReadAllLines(pathListOfThread).ToList();
            listOfTorque = File.ReadAllLines(pathListOfTorque).ToList();

            InitializeComponent();

            //dataGridMwd.ColumnCount = 6;
            //dataGridSoftware.ColumnCount = 3;

            DataGridViewColumn mwd0 = new DataGridViewTextBoxColumn();
            dataGridMwd.Columns.Add(mwd0);
            dataGridMwd.Columns[0].HeaderText = "Seq";
            dataGridMwd.Columns[0].DataPropertyName = "P_SEQ_NO";

            DataGridViewComboBoxColumn mwd1 = new DataGridViewComboBoxColumn();
            mwd1.Items.AddRange(listOfTorque.ToArray());
            mwd1.Name = "Torque";
            mwd1.HeaderText = "Torque";
            mwd1.DataPropertyName = "P_L_TORQUE";
            dataGridMwd.Columns.Add(mwd1);
            //dataGridMwd.Columns[1].HeaderText = "Torque";
            //dataGridMwd.Columns[1].DataPropertyName = "P_L_TORQUE";
            //dataGridMwd.Columns[1].Name = "Torque";

            DataGridViewComboBoxColumn mwd2 = new DataGridViewComboBoxColumn();
            mwd2.Items.AddRange(listOfThread.ToArray());
            mwd2.Name = "ThreadTop";
            mwd2.HeaderText = "Thread Top";
            mwd2.DataPropertyName = "P_L_THREAD_TOP";
            dataGridMwd.Columns.Add(mwd2);
            //dataGridMwd.Columns[2].HeaderText = "Thread Top";
            //dataGridMwd.Columns[2].DataPropertyName = "P_L_THREAD_TOP";
            //dataGridMwd.Columns[2].Name = "ThreadTop";

            DataGridViewComboBoxColumn mwd3 = new DataGridViewComboBoxColumn();
            mwd3.Items.AddRange(listOfThread.ToArray());
            mwd3.Name = "ThreadBottom";
            mwd3.HeaderText = "Thread Bottom";
            mwd3.DataPropertyName = "P_L_THREAD_BTM";
            dataGridMwd.Columns.Add(mwd3);
            //dataGridMwd.Columns[3].HeaderText = "Thread Bottom";
            //dataGridMwd.Columns[3].DataPropertyName = "P_L_THREAD_BTM";
            //dataGridMwd.Columns[3].Name = "ThreadBottom";

            DataGridViewColumn mwd4 = new DataGridViewTextBoxColumn();
            dataGridMwd.Columns.Add(mwd4);
            dataGridMwd.Columns[4].HeaderText = "Description";
            dataGridMwd.Columns[4].DataPropertyName = "P_DESCRIPTION";

            DataGridViewColumn mwd5 = new DataGridViewTextBoxColumn();
            dataGridMwd.Columns.Add(mwd5);
            dataGridMwd.Columns[5].HeaderText = "Comments";
            dataGridMwd.Columns[5].DataPropertyName = "P_COMMENTS";

            DataGridViewComboBoxColumn soft0 = new DataGridViewComboBoxColumn();
            soft0.Items.AddRange(listOfSoftsensor.ToArray());
            soft0.Name = "Sensor";
            soft0.HeaderText = "Sensor";
            soft0.DataPropertyName = "P_L_MSR_SENSOR";
            dataGridSoftware.Columns.Add(soft0);
            //dataGridSoftware.Columns[0].HeaderText = "Sensor";
            //dataGridSoftware.Columns[0].DataPropertyName = "P_L_MSR_SENSOR";
            //dataGridSoftware.Columns[0].Name = "Sensor";


            DataGridViewColumn soft1 = new DataGridViewTextBoxColumn();
            dataGridSoftware.Columns.Add(soft1);
            dataGridSoftware.Columns[1].HeaderText = "Soft Version";
            dataGridSoftware.Columns[1].DataPropertyName = "P_OPS_VERSION";

              DataGridViewColumn soft2 = new DataGridViewTextBoxColumn();
            dataGridSoftware.Columns.Add(soft2);
            dataGridSoftware.Columns[2].HeaderText = "WS (ignore)";
            dataGridSoftware.Columns[2].DataPropertyName = "P_WS_VERSION";

            dataGridMwd.DataSource = compBinding;
            dataGridSoftware.DataSource = softBinding;


            PopulateMwdGrid(linkObjects.MwdCompPostDict);
            PopulateSoftwareGrid(linkObjects.MwdSoftPostDict);

            if (withHeader)
            {
                this.Size = new System.Drawing.Size(this.Size.Width, this.Size.Height + 200);

                //this.btnAdd.Location = new System.Drawing.Point(btnAdd.Location.X, btnAdd.Location.Y + 200);
                //this.btnCancel.Location = new System.Drawing.Point(btnCancel.Location.X, btnCancel.Location.Y + 200);
                //this.dataGridMwd.Location = new System.Drawing.Point(dataGridMwd.Location.X, dataGridMwd.Location.Y + 200);
                //this.dataGridSoftware.Location = new System.Drawing.Point(dataGridSoftware.Location.X, dataGridSoftware.Location.Y + 200);

                foreach (Control item in this.Controls)
                {
                    item.Location = new System.Drawing.Point(item.Location.X, item.Location.Y + 200);
                }

                mwdHeader.Location = new System.Drawing.Point(0, 0);
                this.Controls.Add(mwdHeader);

                mwdHeader.BhaDescription.Text = linkObjects.MwdBhaPost.P_BHA_DESC;
                mwdHeader.BhaComments.Text = linkObjects.MwdBhaPost.P_MWDDWD_ADD_INFO;

                if (linkObjects.MwdBhaPost.P_HC_TOOL == "Yes")
                {
                    mwdHeader.HardConnectCheckbox.Checked = true;
                }

                mwdHeader.BhaComments.TextChanged += BhaComments_TextChanged;
                mwdHeader.BhaDescription.TextChanged += BhaDescription_TextChanged;
                mwdHeader.HardConnectCheckbox.CheckedChanged += HardConnectCheckbox_CheckedChanged;

            }

            Show();
            UpdateGrid(null, null);
            //Timer t = new Timer() { Interval = 1000};

            //t.Tick += UpdateGrid;
            //t.Start();


        }

        private void HardConnectCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (mwdHeader.HardConnectCheckbox.Checked )
            {
                linkObjects.MwdBhaPost.P_HC_TOOL = "Yes";
            }
            else
            {
                linkObjects.MwdBhaPost.P_HC_TOOL = "No";
            }
        }

        private void BhaDescription_TextChanged(object sender, EventArgs e)
        {
            linkObjects.MwdBhaPost.P_BHA_DESC = mwdHeader.BhaDescription.Text;
        }

        private void BhaComments_TextChanged(object sender, EventArgs e)
        {
            linkObjects.MwdBhaPost.P_MWDDWD_ADD_INFO = mwdHeader.BhaComments.Text;
        }

        private void UpdateGrid(object sender, EventArgs e)
        {

            foreach (DataGridViewColumn item in dataGridMwd.Columns)
            {
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            foreach (DataGridViewColumn item in dataGridSoftware.Columns)
            {
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            foreach (DataGridViewColumn item in dataGridMwd.Columns)
            {
                int colWidth = item.Width;
                item.Width = colWidth+10;
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }

            foreach (DataGridViewColumn item in dataGridSoftware.Columns)
            {
                int colWidth = item.Width;
                item.Width = colWidth+10;
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }


            dataGridMwd.Columns[5].Width = 250;

        }

        private void PopulateMwdGrid(Dictionary<int, MwdCompPosts> mwdCompPostDict)
        {
            if (mwdCompPostDict.Count > 0)
            {
                for (int i = 0; i < mwdCompPostDict.Count; i++)
                {
                    compBinding.Add(mwdCompPostDict[i]);
                }

                dataGridMwd.FirstDisplayedScrollingRowIndex = compBinding.Count - 1;
            }
        }

        private void PopulateSoftwareGrid(Dictionary<int, MwdSoftPosts> mwdSoftPostDict)
        {
            if (mwdSoftPostDict.Count > 0)
            {
                for (int i = 0; i < mwdSoftPostDict.Count; i++)
                {
                    softBinding.Add(mwdSoftPostDict[i]);
                }

                dataGridSoftware.FirstDisplayedScrollingRowIndex = softBinding.Count - 1;
            }
        }



        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (dataGridChanged)
            {
               
                Dictionary<int, MwdCompPosts> mwd = new Dictionary<int, MwdCompPosts>();
                Dictionary<int, MwdSoftPosts> soft = new Dictionary<int, MwdSoftPosts>();

                //int i = 0;
                //foreach (var item in compBinding)
                //{
                //    mwd[i].P_SEQ_NO = item.P_SEQ_NO;
                //    mwd[i].P_L_TORQUE = item.P_L_TORQUE;
                //    mwd[i].P_L_THREAD_TOP = item.P_L_THREAD_TOP;
                //    mwd[i].P_L_THREAD_BTM = item.P_L_THREAD_BTM;
                //    mwd[i].P_DESCRIPTION = item.P_DESCRIPTION;
                //    mwd[i].P_COMMENTS = item.P_COMMENTS;
                //    i++;
                //}
                for (int i = 0; i < compBinding.Count; i++)
                {
                    if (compBinding[i].P_SEQ_NO == "")
                    {
                        continue;
                    }

                    MwdCompPosts m = new MwdCompPosts();
                    m.P_SEQ_NO = compBinding[i].P_SEQ_NO;
                    m.P_L_TORQUE = String.IsNullOrEmpty(compBinding[i].P_L_TORQUE)?" ":compBinding[i].P_L_TORQUE;
                    
                    m.P_L_THREAD_TOP = String.IsNullOrEmpty(compBinding[i].P_L_THREAD_TOP)?" ": compBinding[i].P_L_THREAD_TOP;
                    m.P_L_THREAD_BTM = String.IsNullOrEmpty(compBinding[i].P_L_THREAD_BTM)?" ": compBinding[i].P_L_THREAD_BTM;
                    m.P_DESCRIPTION = compBinding[i].P_DESCRIPTION;
                    m.P_COMMENTS = compBinding[i].P_COMMENTS;
                    mwd.Add(i, m);

                }
                //i = 0;
                //foreach (var item in softBinding)
                //{
                //    soft[i].P_L_MSR_SENSOR = item.P_L_MSR_SENSOR;
                //    soft[i].P_OPS_VERSION = item.P_OPS_VERSION;
                //    soft[i].P_WS_VERSION = item.P_WS_VERSION;
                //    i++;
                //}

                for (int i = 0; i < softBinding.Count; i++)
                {
                    if (softBinding[i].P_L_MSR_SENSOR == "")
                    {
                        continue;
                    }
                    MwdSoftPosts s = new MwdSoftPosts();
                    s.P_L_MSR_SENSOR = softBinding[i].P_L_MSR_SENSOR;
                    
                    s.P_OPS_VERSION = softBinding[i].P_OPS_VERSION;
                    s.P_WS_VERSION = softBinding[i].P_WS_VERSION;
                    soft.Add(i, s);

                }

                linkObjects.MwdCompPostDict = mwd;
                linkObjects.MwdSoftPostDict = soft;
            }

            

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(HDocUtility.ConvertMshtmlToString(engine.HtmlDoc));
            string mwdId = HDocUtility.GetInputByName("P_104", doc);

            if (engine.ModemStat == BandObjectLib.ModemEvents.BhaEdit)
            {

                ModemParameters mp = new ModemParameters(engine.HtmlDoc, true, "P_10");
                ModemMwdInsert mi = new ModemMwdInsert(mp, linkObjects, true);

            }
            else if (engine.ModemStat == BandObjectLib.ModemEvents.Edit)
            {
                string modemViewUrl = HDocUtility.UrlModemView;
                ModemConnection mc = new ModemConnection(modemViewUrl + engine.ModemNoEngine);
                ModemParameters mp = new ModemParameters(mc.GetHtmlAsHdoc(), true, "P_SSORD_ID");
                //ModemParameters mp = new ModemParameters(engine.HtmlDoc, true, "P_SSORD_ID");
                ModemMwdInsert mi = new ModemMwdInsert(mp, linkObjects, false);
            }


            if (!String.IsNullOrEmpty(mwdId))
            {
                engine.SmartNavigate(HDocUtility.BhaEditUrlwMwdId + mwdId);
            }
            else
            {
                //engine.RefreshPage();
                engine.SmartNavigate(HDocUtility.UrlModemEdit + targetModem);
            }


            this.Close();
            this.Hide();
            this.Dispose();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Hide();
            Close();
            Dispose();
        }

        private void dataGridMwd_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            dataGridChanged = true;
        }

        private void dataGridSoftware_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            dataGridChanged = true;
        }
    }
}
