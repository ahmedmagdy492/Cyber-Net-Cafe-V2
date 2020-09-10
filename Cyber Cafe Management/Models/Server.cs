using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cyber_Cafe_Management.Models
{
    public class Server
    {
        private readonly Socket socket;
        private readonly List<Computer> computers;
        private readonly FlowLayoutPanel container;
        private bool isSelected = false;
        private ContextMenuStrip btnMenu;
        public Button clickedBtn { get; private set; }

        public bool IsSelected => isSelected;
        public Socket ServerSocket => socket;
        public IEnumerable<Computer> Computers => computers;

        public Server(FlowLayoutPanel container, ContextMenuStrip btnMenu)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            computers = new List<Computer>();
            this.container = container;
            this.btnMenu = btnMenu;
        }

        public void Init()
        {
            socket.Bind(new IPEndPoint(IPAddress.Any, 9000));
            socket.Listen(50);
        }

        public async Task AcceptConnections()
        {
            var client = await socket.AcceptAsync();
            var computer = new Computer
            {
                Id = Computers.Count() + 1,
                Name = Guid.NewGuid().ToString(),
                Socket = client
            };
            computers.Add(computer);
            Button btn = new Button
            {
                Name = computer.Name,
                Text = "No." + computer.Id + "\n" + client.RemoteEndPoint.ToString(),
                Image = Properties.Resources.computer,
                AutoSize = true,
                TextImageRelation = TextImageRelation.ImageAboveText
            };
            btn.ContextMenuStrip = btnMenu;
            btn.Click += Btn_Click;
            btn.MouseDown += Btn_MouseDown;
            container.Invoke(new Action(() => container.Controls.Add(btn)));
        }

        private void Btn_MouseDown(object sender, MouseEventArgs e)
        {
            clickedBtn = (Button)sender;

            if (isSelected)
            {
                isSelected = false;
            }
            else
            {
                isSelected = true;
            }
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            clickedBtn = (Button)sender;
            if (isSelected)
            {
                isSelected = false;
            }
            else
            {
                isSelected = true;
            }
        }
    }
}
