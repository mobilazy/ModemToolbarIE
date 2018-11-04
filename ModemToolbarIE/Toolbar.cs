using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;

using System.Text;

using System.Windows.Forms;
using BandObjectLib;
using System.Runtime.InteropServices;

namespace ModemToolbarIE
{
    [Guid("0823E052-F731-40A2-BE47-42527C602B0D")]
    [BandObject("Hello World Bar", BandObjectStyle.Horizontal
         | BandObjectStyle.ExplorerToolbar, HelpText = "Shows bar that says hello.")]
    public partial class Toolbar : BandObject
    {

        public Toolbar()
        {
            InitializeComponent();
        }


        private System.Windows.Forms.Button button1;






        private void button1_Click(object sender, System.EventArgs e)
        {
            MessageBox.Show("Hello, World!");
            
        }
    }
}
