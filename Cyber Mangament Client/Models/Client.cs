using Newtonsoft.Json;
using SharedLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Cyber_Mangament_Client.Models
{
    public class Client
    {
        private readonly Socket socket;
        private byte[] buffer;

        public Socket ClientSocket => socket;

        public Client()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            buffer = new byte[1024];
        }

        public async Task Connect(string ipAddress, int port)
        {
            await socket.ConnectAsync(ipAddress, port);
        }

        public async Task<Message> RecMessage()
        {
            await socket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
            var message = JsonConvert.DeserializeObject<Message>(Encoding.Default.GetString(buffer));
            buffer = new byte[1024];
            return message;
        }
    }
}
