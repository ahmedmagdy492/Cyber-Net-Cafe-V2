using Cyber_Mangament_Client.Models;
using SharedLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cyber_Mangament_Client
{
    public partial class frmClient : Form
    {
        private readonly Client client;
        public frmClient()
        {
            InitializeComponent();
            client = new Client();
        }

        private async void frmClient_Load(object sender, EventArgs e)
        {
            int x = (this.Width - lblLogo.Width) / 2;
            int y = (this.Height - lblLogo.Height) / 2;
            lblLogo.Location = new Point(x, y);
            lblNote.Location = new Point((this.Width - lblNote.Width )/ 2, (this.Height - lblNote.Height) /2 + 50);
            // connect to the server
            try
            {
                string ipAddress = Properties.Settings.Default.ServerIP;
                await client.Connect(ipAddress, 9000);

                // reciving data from the server
                await Task.Run(async () =>
                {
                    while (true)
                    {
                        if (client.ClientSocket.Available > 0)
                        {
                            var message = await client.RecMessage();
                            // TODO: check for the incoming headers
                            ExecuteAction(message);
                        }
                    }
                });
            }
            catch (SocketException)
            {
                MessageBox.Show("The Admin PC Currently is not Online, Please try again later", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExecuteAction(SharedLib.Message message)
        {
            string type = message.Headers["Type"];
            if (type == "Open")
            {
                OnPC();
            }
            else if(type == "Close")
            {
                OffPC();
            }
            else if(type == "Suspend")
            {
                SuspendPC();
            }
        }

        public void SuspendPC()
        {
            SuspendManager.Suspend();
        }

        public void OnPC()
        {
            this.Hide();
        }

        public void OffPC()
        {
            this.Show();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            frmLogin frmLogin = new frmLogin();
            frmLogin.ShowDialog();
        }

        private async void frmClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            var buffer = Encoding.Default.GetBytes(client.ClientSocket.LocalEndPoint.ToString());
            await client.ClientSocket.SendAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
        }
    }
}
