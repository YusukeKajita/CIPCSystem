using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPCServer_Console.ConnectionHostData
{
    public class DirectConnect_NoticePort
    {
        public byte[] SendData
        {
            set;
            get;
        }
        private UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc;

        public DirectConnect_NoticePort(CIPCServer.ClientStatus clientstatus)
        {
            this.enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
            //OK
            this.enc += (int)0;
            this.enc += clientstatus.ReceiverIP;
            this.enc += clientstatus.ReceiverPort;
            this.enc += clientstatus.Name;
            this.SendData = this.enc.data;
        }
    }
}
