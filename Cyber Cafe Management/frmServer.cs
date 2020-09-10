using Cyber_Cafe_Management.Models;
using Newtonsoft.Json;
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

namespace Cyber_Cafe_Management
{
    public partial class frmServer : Form
    {
        private readonly Server server;
        private int mins = 0, hours = 0;
        public frmServer()
        {
            InitializeComponent();
            server = new Server(pnlContainer, btnContextMenu);
        }

        private void frmServer_Load(object sender, EventArgs e)
        {
            server.Init();

            // start listinging for requests
            Task.Run(async () =>
            {
                while(true)
                {
                    await server.AcceptConnections();
                }
            });

            Task.Run(async () =>
            {
                while(true)
                {
                    foreach(var comp in server.Computers)
                    {
                        if (comp.Socket.Available > 0)
                        {
                            var buffer = new byte[1024];
                            await comp.Socket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                            var result = Encoding.Default.GetString(buffer);
                            var computer = server.Computers.FirstOrDefault(c => c.Socket.LocalEndPoint.ToString() == result);
                            var button = FindButton(computer.Name);
                            if (button != null)
                            {
                                pnlContainer.Controls.Remove(button);
                            }
                        }
                    }
                }
            });
        }

        private async void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (server.IsSelected == true && server.clickedBtn != null && nmHours.Value != 0 || numMins.Value != 0)
            {
                var computer = server.Computers.FirstOrDefault(c => c.Name == server.clickedBtn.Name);

                if (computer.Socket.Connected && !computer.IsOnline)
                {
                    computer.IsOnline = true;
                    Dictionary<string, string> headers = new Dictionary<string, string>();
                    headers.Add("Type", "Open");
                    var message = new SharedLib.Message
                    {
                        Headers = headers
                    };
                    var buffer = Encoding.Default.GetBytes(JsonConvert.SerializeObject(message));
                    await Task.Run(() =>
                    {
                        computer.Socket.Send(buffer);
                    });

                    // set the timer for that computer

                    computer.Timer.Enabled = true;
                    if (nmHours.Value != 0 && numMins.Value != 0)
                    {
                        mins = (int)numMins.Value;
                        hours = (int)nmHours.Value;
                    }
                    else if (nmHours.Value == 0 && numMins.Value != 0)
                    {
                        mins = (int)numMins.Value;
                    }
                    else if (nmHours.Value != 0 && numMins.Value == 0)
                    {
                        hours = (int)nmHours.Value;
                    }
                    Button btn = FindButton(computer.Name);
                    computer.StartTimer(hours, mins, ref btn);
                }
                else
                {
                    pnlContainer.Controls.Remove(server.clickedBtn);
                }
            }
            else
            {
                MessageBox.Show("Please select a pc first");
            }
        }

        private async void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (server.IsSelected == true && server.clickedBtn != null)
            {
                var computer = server.Computers.FirstOrDefault(c => c.Name == server.clickedBtn.Name);

                if (computer.Socket.Connected && computer.IsOnline)
                {
                    computer.IsOnline = false;
                    Dictionary<string, string> headers = new Dictionary<string, string>();
                    headers.Add("Type", "Close");
                    var message = new SharedLib.Message
                    {
                        Headers = headers
                    };
                    var buffer = Encoding.Default.GetBytes(JsonConvert.SerializeObject(message));
                    await Task.Run(() =>
                    {
                        computer.Socket.Send(buffer);
                    });

                    // set the timer for that computer

                    computer.Timer.Enabled = false;
                }
                else
                {
                    pnlContainer.Controls.Remove(server.clickedBtn);
                }
            }
            else
            {
                MessageBox.Show("Please select a pc first");
            }
        }

        private async void suspendToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (server.IsSelected == true && server.clickedBtn != null)
            {
                var computer = server.Computers.FirstOrDefault(c => c.Name == server.clickedBtn.Name);

                if (computer.Socket.Connected)
                {
                    Dictionary<string, string> headers = new Dictionary<string, string>();
                    headers.Add("Type", "Suspend");
                    var message = new SharedLib.Message
                    {
                        Headers = headers
                    };
                    var buffer = Encoding.Default.GetBytes(JsonConvert.SerializeObject(message));
                    await Task.Run(() =>
                    {
                        computer.Socket.Send(buffer);
                    });

                    // set the timer for that computer
                    computer.Timer.Enabled = false;
                }
                else
                {
                    pnlContainer.Controls.Remove(server.clickedBtn);
                }
            }
            else
            {
                MessageBox.Show("Please select a pc first");
            }
        }

        private Button FindButton(string name)
        {
            foreach (var control in pnlContainer.Controls)
            {
                if (control is Button)
                {
                    Button btn = ((Button)control);
                    if (btn.Name == name)
                    {
                        return btn;
                    }
                }
            }
            return null;
        }
    }
}
