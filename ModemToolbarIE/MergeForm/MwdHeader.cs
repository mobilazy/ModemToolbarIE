using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModemToolbarIE.MergeForm
{
    public partial class MwdHeader : UserControl
    {
        public TextBox BhaDescription { get; set; }
        public TextBox BhaComments { get; set; }
        public CheckBox HardConnectCheckbox { get; set; }
        public MwdHeader()
        {
            InitializeComponent();
            BhaDescription = txtDescription;
            BhaComments = txtComments;
            HardConnectCheckbox = checkBoxHardConnect;
        }
    }
}
