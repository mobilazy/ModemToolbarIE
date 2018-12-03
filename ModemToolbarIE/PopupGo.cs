using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ModemToolbarIE
{
    public partial class PopupGo : Form
    {
        private Toolbar tlb;

        public PopupGo(Toolbar toolbar)
        {
            tlb = toolbar;
            InitializeComponent();
            btnOpenModem.Enabled = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Hide();
        }

        private void btnOpenModem_Click(object sender, EventArgs e)
        {
            tlb.Navigate2(@"http://tanwebs.corp.halliburton.com/pls/log_web/mobssus_vieword$order_mc.QueryViewByKey?P_SSORD_ID=" + txtModemNo.Text);
        }

        private void txtModemNo_TextChanged(object sender, EventArgs e)
        {
            int i;

            if (txtModemNo.Text.Length == 7 && int.TryParse(txtModemNo.Text, out i))
            {
                btnOpenModem.Enabled = true;
                lblStatus.Text = "Click Open";
            }
            else if (!int.TryParse(txtModemNo.Text, out i))
            {
                lblStatus.Text = "Numbers Only";
            }
            else 
            {
                lblStatus.Text = "Enter Modem No";
            }
        }
    }
}
