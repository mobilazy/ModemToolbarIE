using System;
using BandObjectLib;
using System.Runtime.InteropServices;
using mshtml;
using SHDocVw;
using System.Windows.Forms;

namespace ModemToolbarIE
{
    [Guid("0823E052-F731-40A2-BE47-42527C602B0D")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)] //this is added from ieengtbr

    [BandObject("Modem Toolbar", BandObjectStyle.Horizontal
         | BandObjectStyle.ExplorerToolbar, HelpText = "Modem Toolbar for helping with modem creation")]

    public partial class Toolbar : BandObject
    {

        private bool bhaEditMode = false;
        


        private bool BhaEditMode
        {
            get
            {
                return bhaEditMode;
            }

            set
            {
                bhaEditMode = value;
                EnableToolstrip(value);
            }
        }

        public Toolbar()
        {
            InitializeComponent();
            HtmlDocCompleted += Toolbar_HtmlDocComplete;
        }




        private void Toolbar_HtmlDocComplete(object sender, ModemEventArgs e)
        {
            ModemEvents modemState = e.ModemEvent;
            string modemNo = e.ModemNo;

            
            
            switch (modemState)
            {
                case ModemEvents.None:
                    txtStatus.Text = "Not Modem";
                    BhaEditMode = false;
                    break;
                case ModemEvents.Gant:
                    txtStatus.Text = "Modem Gant";
                    BhaEditMode = false;
                    break;
                case ModemEvents.View:
                    txtStatus.Text = modemNo + " - View";
                    BhaEditMode = false;
                    break;
                case ModemEvents.BhaView:
                    txtStatus.Text = modemNo + " - Mwd View";
                    BhaEditMode = false;
                    break;
                case ModemEvents.DdView:
                    txtStatus.Text = modemNo + " - Dd View";
                    BhaEditMode = false;
                    break;
                case ModemEvents.GpView:
                    txtStatus.Text = modemNo + " - Gp View";
                    BhaEditMode = false;
                    break;
                case ModemEvents.Edit:
                    txtStatus.Text = modemNo + " - Edit";
                    BhaEditMode = false;
                    break;
                case ModemEvents.BhaEdit:
                    txtStatus.Text = modemNo + " - Mwd Edit";
                    BhaEditMode = CheckIfMwdEditEmptry(modemNo);
                    break;
                case ModemEvents.DdEdit:
                    txtStatus.Text = modemNo + " - Dd Edit";
                    BhaEditMode = false;
                    break;
                case ModemEvents.GpEdit:
                    txtStatus.Text = modemNo + " - Gp Edit";
                    BhaEditMode = false;
                    break;
                default:
                    BhaEditMode = false;
                    break;
            }
        }

        private void EnableToolstrip(bool enable)
        {
            this.addRLLToolStripMenuItem.Enabled = enable;
            this.addPulserPMToolStripMenuItem.Enabled = enable;
            this.addLWDToolStripMenuItem.Enabled = enable;
            this.addLooseToolStripMenuItem.Enabled = enable;
            this.addOthersToolStripMenuItem.Enabled = enable;
        }

        private bool CheckIfMwdEditEmptry(string _modemNo)
        {

            try
            {
                Utility.ModemParameters modemParameters = new Utility.ModemParameters(htmlDocument, _modemNo);
            }
            catch (Exception e)
            {

                MessageBox.Show(e.ToString());
            }
            

            //MessageBox.Show(modemParameters.MwdBhaCount.ToString());

            //if (modemParameters.MwdBhaCount == 0)
            //{
            //    return true;
            //}

           

            return false;
        }

        private void toolStripContainer1_TopToolStripPanel_Click(object sender, EventArgs e)
        {

        }

        private void addLWDToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void Toolbar_Load(object sender, EventArgs e)
        {

        }

        private void btnGoToModem_Click(object sender, EventArgs e)
        {
            PopupGo pp = new PopupGo(this);
            pp.Show();
        }

        private void btnGoToGant_Click(object sender, EventArgs e)
        {
            this.Navigate2(@"http://tanwebs.corp.halliburton.com/pls/log_web/gant.web");
        }
    }
}
