using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPCServer_Console.CIPCServer
{
    public class ClientHost
    {
        public UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT client;

        public ClientStatus clientstatus;

        public delegate void DataReceivedEventHandler(object sender, byte[] e);
        public event DataReceivedEventHandler DataReceived;
        protected virtual void OnDataReceived(byte[] e)
        {
            if (this.DataReceived != null) { this.DataReceived(this, e); }
        }

        public ClientHost(int Port)
        {
            this.clientstatus = new ClientStatus();
            this.clientstatus.ServerPort = Port;

            this.client = new UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT(Port);
            this.client.DataReceived += client_DataReceived_Init;
        }

        private void client_DataReceived_Init(object sender, byte[] e)
        {
            try
            {
                ConnectionHostData.ClientHostReceive clienthostreceive = new ConnectionHostData.ClientHostReceive(e);
                this.clientstatus.FPS = clienthostreceive.clientstatus.FPS;
                this.clientstatus.Mode = clienthostreceive.clientstatus.Mode;
                this.clientstatus.Name = clienthostreceive.clientstatus.Name;
                this.clientstatus.ReceiverIP = this.client.RemoteEP.Address.ToString();
                this.clientstatus.ReceiverPort = this.client.RemoteEP.Port;
                this.StatePrint();

                System.Threading.Thread.Sleep(100);

                ConnectionHostData.NormalResponse normalresponse = new ConnectionHostData.NormalResponse();
                this.client.Send(normalresponse.data);

                this.client.DataReceived -= client_DataReceived_Init;
                this.client.DataReceived += client_DataReceived;
            }
            catch (Exception ex)
            {
                Errorlog.Print(ex, this);
            }
        }

        private void StatePrint()
        {
            Report.PrintDateBar(this);
            Report.Print("Name : " + this.clientstatus.Name, this);
            Report.Print("RemoteEP : " + this.clientstatus.ReceiverIP +" : "+this.clientstatus.ReceiverPort.ToString(), this);
            Report.Print("MyPort : " + this.clientstatus.ServerPort.ToString(), this);
            Report.Print("FPS : " + this.clientstatus.FPS.ToString(), this);
            Report.Print("Mode : " + this.clientstatus.Mode.ToString(), this);
        }

        public void Close()
        {
            this.client.DataReceived -= client_DataReceived;
            this.DataReceived = null;
            this.client.Close();
        }

        void client_DataReceived(object sender, byte[] e)
        {
            this.OnDataReceived(e);
        }

        public void Send(byte[] data)
        {
            this.client.Send(data);
        }

    }
}
