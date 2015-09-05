using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPCServer_Console.CIPCServer
{
    public class DataConnection
    {
        public enum State
        {
            AllRight,
            Error,
            NoInit,
        }
        public State state { set; get; }

        public int SenderID { set; get; }
        public int ReceiverID { set; get; }

        public ClientHost SenderClient { set; get; }
        public ClientHost ReceiverClient { set; get; }

        public DataConnection(int SenderID, int ReceiverID, List<ClientHost> listclienthost)
        {
            try
            {
                this.state = State.NoInit;
                this.SenderID = SenderID;
                this.ReceiverID = ReceiverID;

                this.SenderClient = listclienthost[SenderID];
                this.ReceiverClient = listclienthost[ReceiverID];
                this.state = State.AllRight;
            }
            catch (Exception ex)
            {
                Errorlog.Print(ex, this);
                this.state = State.Error;
            }
        }
        public DataConnection(ClientHost senderclient, ClientHost receiverclient)
        {
            try
            {
                this.state = State.NoInit;

                this.SenderClient = senderclient;
                this.ReceiverClient = receiverclient;
                this.state = State.AllRight;
            }
            catch (Exception ex)
            {
                Errorlog.Print(ex, this);
                this.state = State.Error;
            }
        }

        public void Connect()
        {
            try
            {
                if (this.SenderClient.clientstatus.Mode == ClientStatus.MODE.DirectConnect && this.ReceiverClient.clientstatus.Mode == ClientStatus.MODE.DirectConnect)
                {
                    

                    ConnectionHostData.DirectConnect_NoticePort noticeport = new ConnectionHostData.DirectConnect_NoticePort(this.SenderClient.clientstatus);
                    this.ReceiverClient.Send(noticeport.SendData);
                    ConnectionHostData.DirectConnect_NoticePort noticeport2 = new ConnectionHostData.DirectConnect_NoticePort(this.ReceiverClient.clientstatus);
                    this.SenderClient.Send(noticeport2.SendData);

                    Report.PrintDateBar(this);
                    Report.Print("DirectConnect " + this.SenderID + " <---> " + this.ReceiverID, this);
                }
                else if (!(this.SenderClient.clientstatus.Mode == ClientStatus.MODE.DirectConnect || this.ReceiverClient.clientstatus.Mode == ClientStatus.MODE.DirectConnect))
                {
                    this.SenderClient.DataReceived += this.SenderClient_DataReceived;
                    Report.PrintDateBar(this);
                    Report.Print("Connect " + this.SenderID + " => " + this.ReceiverID, this);
                }
            }
            catch (Exception ex)
            {
                Errorlog.Print(ex, this);
                this.state = State.Error;

            }
        }


        void SenderClient_DataReceived(object sender, byte[] e)
        {
            try
            {
                this.ReceiverClient.Send(e);
            }
            catch (Exception ex)
            {
                Errorlog.Print(ex, this);
            }
        }
        public void DisConnect()
        {
            try
            {
                if (this.SenderClient.clientstatus.Mode == ClientStatus.MODE.DirectConnect && this.ReceiverClient.clientstatus.Mode == ClientStatus.MODE.DirectConnect)
                {
                    Report.PrintDateBar(this);
                    Report.Print("Disconnect : " + this.SenderID + " <---> " + this.ReceiverID, this);
                }
                else if (!(this.SenderClient.clientstatus.Mode == ClientStatus.MODE.DirectConnect || this.ReceiverClient.clientstatus.Mode == ClientStatus.MODE.DirectConnect))
                {
                    this.SenderClient.DataReceived -= SenderClient_DataReceived;
                    Report.PrintDateBar(this);
                    Report.Print("Disconnect : " + this.SenderID + " => " + this.ReceiverID, this);
                }
            }
            catch (Exception ex)
            {
                Errorlog.Print(ex, this);
            }
        }
    }
}
