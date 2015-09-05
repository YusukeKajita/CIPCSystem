using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPCServer_Console.ConnectionHostData
{
    public class NormalResponse
    {
        public byte[] data
        {
            set;
            get;
        }
        private UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc;
        
        public NormalResponse()
        {
            this.enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
            this.enc += (int)0;
            this.data = this.enc.data;
        }
    }
}
