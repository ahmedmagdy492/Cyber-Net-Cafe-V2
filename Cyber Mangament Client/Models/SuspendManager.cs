using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyber_Mangament_Client.Models
{
    public static class SuspendManager
    {
        public static void Suspend()
        {
            CommandExecutorManager.ExecuteCommand("RUNDLL32.EXE powrprof.dll,SetSuspendState 0,1,0");
        }
    }
}
