using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading;

namespace CIPC_CS_Unity.CLIENT
{
    public class CLIENT : BASE
    {
        #region constructer
        public CLIENT()
            : base(SETTINGS.MyInfo.Port, SETTINGS.RemoteExchangeServer.IP, SETTINGS.RemoteExchangeServer.Port, SETTINGS.MyInfo.Name, SETTINGS.MyInfo.fps)
        {
        }

        public CLIENT(int myPort, string remoteIP, int serverPort)
            : base(myPort, remoteIP, serverPort, SETTINGS.MyInfo.Name, SETTINGS.MyInfo.fps)
        {
        }
        public CLIENT(int myPort, string remoteIP, int serverPort, string name, int fps)
            : base(myPort, remoteIP, serverPort, name, fps)
        {

        }
        ~CLIENT()
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {

            }
        }


        #endregion


        #region private method
        protected override void Connect_add()
        {
            this.enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
            enc += SETTINGS.ConnectionCommands.GREETING.DEMANDS;
            this.udp_client.Send(enc.data);

            dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
            dec.Source = this.udp_client.Recieve();
            if (this.dec.get_int() != SETTINGS.ConnectionCommands.REPLY.OK)
            {
                throw new Exception("接続失敗");
            }
            this.udp_client.Close();
            this.udp_client = null;
            Thread.Sleep(100);
            this.remotePort = dec.get_int();
        }

        protected override void Connect_Sender_add()
        {
            this.enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
            this.enc += SETTINGS.ConnectionCommands.GREETING.DEMANDS;
            this.enc += this.fps;
            this.enc += SETTINGS.ConnectionCommands.MODE.SEND;
            this.enc += this.name;
            this.udp_client.Send(this.enc.data);

            dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
            dec.Source = this.udp_client.Recieve();
        }

        protected override void Connect_Receiver_add()
        {
            this.enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
            this.enc += SETTINGS.ConnectionCommands.GREETING.DEMANDS;
            this.enc += this.fps;
            this.enc += SETTINGS.ConnectionCommands.MODE.RECEIVE;
            this.enc += this.name;
            this.udp_client.Send(this.enc.data);

            dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
            dec.Source = this.udp_client.Recieve();

            this.udp_client.DataReceived += udp_client_DataReceived;
        }

        void udp_client_DataReceived(object sender, byte[] e)
        {
            this.OnDataReceived(e);
        }

        protected override void Close_add()
        {
            this.udp_client = new UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT(this.remoteIP, this.serverPort, this.myPort);
            this.enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
            enc += SETTINGS.ConnectionCommands.GREETING.END;
            enc += this.remotePort;
            this.udp_client.Send(this.enc.data);

            dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
            dec.Source = this.udp_client.Recieve();
            if (dec.get_int() == SETTINGS.ConnectionCommands.GREETING.END)
            {

            }
            else
            {

            }
            this.udp_client.Close();
            this.udp_client = null;
        }
        #endregion

        #region Static Method
        public static void Connect(int SenderID, int ReceiverID, string RemoteIP, int RemotePort, int myPort)
        {

            UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT client = new UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT(RemoteIP, RemotePort, myPort);
            Udp_Send(client, new TerminalConnectionSettings.TerminalProtocols.Connect(SenderID, ReceiverID));
            client.Close();
        }
        public static void ConnectByClientName(string SenderClientName, string ReceiverClientName, string RemoteIP, int RemotePort, int myPort)
        {
            UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT client = new UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT(RemoteIP, RemotePort, myPort);
            Udp_Send(client, new TerminalConnectionSettings.TerminalProtocols.DisConnectByName(SenderClientName, ReceiverClientName));
            client.Close();
        }
        
        public static void Disconnect(int SenderID, int ReceiverID, string RemoteIP, int RemotePort, int myPort)
        {
            UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT client = new UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT(RemoteIP, RemotePort, myPort);
            Udp_Send(client, new TerminalConnectionSettings.TerminalProtocols.DisConnect(SenderID, ReceiverID));
            client.Close();
        }
        public static void DisconnectByClientName(string SenderClientName, string ReceiverClientName, string RemoteIP, int RemotePort, int myPort)
        {
            UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT client = new UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT(RemoteIP, RemotePort, myPort);
            Udp_Send(client, new TerminalConnectionSettings.TerminalProtocols.DisConnectByName(SenderClientName, ReceiverClientName));
            client.Close();
        }

        public static void AllDisconnect(string RemoteIP, int RemotePort, int myPort)
        {
            UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT client = new UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT(RemoteIP, RemotePort, myPort);
            Udp_Send(client, new TerminalConnectionSettings.TerminalProtocols.AllDisConnect());
            client.Close();
        }
        public static void TurnOffSyncConnect(string RemoteIP, int RemotePort, int myPort)
        {
            UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT client = new UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT(RemoteIP, RemotePort, myPort);
            Udp_Send(client, new TerminalConnectionSettings.TerminalProtocols.TurnOffSyncConnect());
            client.Close();
        }
        public static void TurnOnSyncConnect(string RemoteIP, int RemotePort, int myPort)
        {
            UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT client = new UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT(RemoteIP, RemotePort, myPort);
            Udp_Send(client, new TerminalConnectionSettings.TerminalProtocols.TurnOnSyncConnect());
            client.Close();
        }

        public static void Udp_Send(UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT client, TerminalConnectionSettings.TerminalProtocols.CIPCTerminalCommand terminalcommand)
        {
            try
            {
                UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
                enc += (int)20;
                enc += (string)terminalcommand.Data;
                client.Send(enc.data);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

    };
}
