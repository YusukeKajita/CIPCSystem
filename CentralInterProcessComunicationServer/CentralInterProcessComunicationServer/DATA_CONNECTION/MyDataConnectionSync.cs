using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralInterProcessCommunicationServer.DATA_CONNECTION
{
    public class MyDataConnectionSync : MyDataConnection
    {
        public MyDataConnectionSync(RemoteHost sender, RemoteHost receiver)
            :base(sender,receiver)
        {
            if (this.RECEIVER == null) return;
            if (this.SENDER == null) return;

            this.Sender.DataReceived += Sender_DataReceived;
            this.Receiver.IsSyncSend = true;
            this.IsConnectSync = true;
        }

        void Sender_DataReceived(object sender, byte[] e)
        {
            this.Receiver.SendSync(e);
            
        }
        public override void Disconnect()
        {
            if (this.RECEIVER == null) return;
            if (this.SENDER == null) return;
            this.Sender.DataReceived -= Sender_DataReceived;
            this.Receiver.IsSyncSend = false;
            base.Disconnect();
        }
    }
}
