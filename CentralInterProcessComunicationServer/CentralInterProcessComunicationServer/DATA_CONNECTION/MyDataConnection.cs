using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralInterProcessCommunicationServer.DATA_CONNECTION
{
    public class MyDataConnection
    {
        #region field
        protected RemoteHost Sender;
        protected RemoteHost Receiver;
        public bool IsConnectSync { protected set; get; }
        #endregion

        #region propaties
        public RemoteHost SENDER 
        {
            get 
            {
                return this.Sender;
            }
        }
        public RemoteHost RECEIVER
        {
            get 
            {
                return this.Receiver;
            }
        }
        #endregion

        #region constructer
        public MyDataConnection(RemoteHost sender, RemoteHost receiver)
        {
            this.Sender = sender;
            this.Receiver = receiver;

            this.Sender.ConnectionHost = this.Receiver;
            this.Receiver.ConnectionHost = this.Sender;
            this.IsConnectSync = false;
        }
        #endregion

        #region public method
        public virtual void Disconnect() 
        {
            if (this.RECEIVER == null) return;
            if (this.SENDER == null) return;
            this.Receiver.ConnectionHost = null;
            this.Sender.ConnectionHost = null;
            this.Receiver.Data = null;
        }
        #endregion

        #region private method
        #endregion
    }
}
