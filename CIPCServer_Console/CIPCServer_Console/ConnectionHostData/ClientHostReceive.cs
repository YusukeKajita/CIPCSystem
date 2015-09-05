using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPCServer_Console.ConnectionHostData
{
    public class ClientHostReceive
    {
        /// <summary>
        /// only use fps, name and mode.
        /// </summary>
        public CIPCServer.ClientStatus clientstatus { set; get; }

        public ClientHostReceive(byte[] data)
        {
            this.clientstatus = new CIPCServer.ClientStatus();
            UDP_PACKETS_CODER.UDP_PACKETS_DECODER dec =new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
            dec.Source = data;
            if (dec.get_int() == 1)
            {
                this.clientstatus.FPS = dec.get_int();
                switch (dec.get_int())
                {
                    case 2:
                        this.clientstatus.Mode = CIPCServer.ClientStatus.MODE.Sender;
                        break;
                    case 3:
                        this.clientstatus.Mode = CIPCServer.ClientStatus.MODE.Receiver;
                        break;
                    case 4:
                        this.clientstatus.Mode = CIPCServer.ClientStatus.MODE.Both;
                        break;
                    case 15:
                        this.clientstatus.Mode = CIPCServer.ClientStatus.MODE.DirectConnect;
                        break;
                    default:
                        this.clientstatus.Mode = CIPCServer.ClientStatus.MODE.NoInit;
                        break;
                }
                this.clientstatus.Name = dec.get_string();
            }
        }
    }
}
