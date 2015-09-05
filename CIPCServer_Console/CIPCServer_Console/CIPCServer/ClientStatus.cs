using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPCServer_Console.CIPCServer
{
    public class ClientStatus
    {
        public enum MODE
        {
            Sender,
            Receiver,
            Both,
            DirectConnect,
            NoInit
        }

        public MODE Mode { set; get; }
        public int ServerPort { set; get; }
        public int ReceiverPort { set; get; }
        public string ReceiverIP { set; get; }
        public int FPS { set; get; }
        public string Name { set; get; }

    }
}
