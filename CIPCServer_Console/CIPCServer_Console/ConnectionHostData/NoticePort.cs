using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPCServer_Console.ConnectionHostData
{
    public class NoticePort
    {
        public byte[] SendData
        {
            set;
            get;
        }
        private UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc;
        
        public NoticePort(int Port)
        {
            this.enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
            //OK
            this.enc += (int)0;
            this.enc += (int)Port;
            this.SendData = this.enc.data;
        }
    }
}
