using Newtonsoft.Json;
using SharedLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cyber_Cafe_Management.Models
{
    public class Computer
    {
        private int hours, mins, secs;
        private Button btn;

        public int Id { get; set; }
        public string Name { get; set; }
        public Socket Socket { get; set; }
        public Timer Timer { get; set; }
        public bool IsOnline { get; set; }
        public string timeStr { get; set; }

        public Computer()
        {
            Timer = new Timer();
            Timer.Interval = 1000;
            Timer.Tick += Timer_Tick;
        }

        public void StartTimer(int hours, int mins, ref Button lbl)
        {
            this.hours = hours;
            this.mins = mins;
            secs = 0;
            Timer.Enabled = true;
            btn = lbl;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(secs > 0)
            {
                secs--;
            }
            else
            {
                if (mins > 0)
                {
                    mins--;
                    secs = 59;
                }
                else
                {
                    if (hours > 0)
                    {
                        hours--;
                        mins = 59;
                    }
                    else
                    {
                        // stop the timer and send the message to the client
                        Timer.Enabled = false;
                        Dictionary<string, string> headers = new Dictionary<string, string>();
                        headers.Add("Type", "Close");
                        var msg = new SharedLib.Message
                        {
                            Headers = headers
                        };
                        var strObj = JsonConvert.SerializeObject(msg);

                        Task.Run(() =>
                        {
                            this.Socket.Send(Encoding.Default.GetBytes(strObj));
                        });
                        IsOnline = false;
                    }
                }
            }
            
            btn.Text = $"No.{Id} \n {this.Socket.RemoteEndPoint} \n {hours}:{mins}:{secs}";
        }

    }
}
