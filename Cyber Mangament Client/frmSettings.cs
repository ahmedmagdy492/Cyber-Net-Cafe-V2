using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cyber_Mangament_Client
{
    public partial class frmSettings : Form
    {
        public frmSettings()
        {
            InitializeComponent();
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            string defaultServerIP = Properties.Settings.Default.ServerIP;
            txtIPAddress.Text = defaultServerIP;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(txtIPAddress.Text))
            {
                Properties.Settings.Default.ServerIP = txtIPAddress.Text;
                Properties.Settings.Default.Save();
            }
            else
            {
                MessageBox.Show("Cannot leave IP Address Property as empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
