using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Net;
using System.Threading;

namespace CIPCServer_Console.CIPCServer
{
    public class MainServer
    {
        private readonly string SettingsPath = "data/settings.xml";

        public CIPCServer.Settings settings;

        public UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT client;

        public List<ClientHost> List_ClientHosts;
        public List<int> List_Port;

        public List<DataConnection> List_DataConnection;

        public int CurrentPort { set; get; }
        public int StartPort { set; get; }
        public ConnectionHostData.MainCommunication ReceiveData { set; get; }

        public MainServer()
        {
            this.Init_Classes();

            this.Load_Settings();

            this.CurrentPort = int.Parse(this.settings.StartPort);
            this.StartPort = this.CurrentPort;

            this.client = new UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT(int.Parse(this.settings.PortNum));
            this.client.DataReceived += client_DataReceived;

        }

        void client_DataReceived(object sender, byte[] e)
        {
            try
            {
                ConnectionHostData.MainCommunication ReceiveData = new ConnectionHostData.MainCommunication(e);
                this.ReceiveData = ReceiveData;
                switch (ReceiveData.connection_command)
                {
                    case ConnectionHostData.MainCommunication.ConnectionCommand.Demand:
                        this.ClientHostAdd();
                        this.NoticeAddedClientHost();
                        break;
                    case ConnectionHostData.MainCommunication.ConnectionCommand.End:
                        this.ClientHostDelete(ReceiveData);
                        this.CloseResponse();
                        break;
                    case ConnectionHostData.MainCommunication.ConnectionCommand.Connect:
                        this.ConnectClientHosts(ReceiveData);
                        break;
                    case ConnectionHostData.MainCommunication.ConnectionCommand.DisConnect:
                        this.DisconnectClientHosts(ReceiveData);
                        break;
                    case ConnectionHostData.MainCommunication.ConnectionCommand.ConnectByServerPort:
                        this.ConnectClientHostsByServerPort(ReceiveData);
                        break;
                    case ConnectionHostData.MainCommunication.ConnectionCommand.ConnectByUserPort:
                        this.ConnectClientHostsByUserPort(ReceiveData);
                        break;
                    case ConnectionHostData.MainCommunication.ConnectionCommand.DisconnectByServerPort:
                        this.DisconnectClientHostsByServerPort(ReceiveData);
                        break;
                    case ConnectionHostData.MainCommunication.ConnectionCommand.DisconnectByUserPort:
                        this.DisconnectClientHostsByUserPort(ReceiveData);
                        break;
                    default:
                        throw new Exception("There is no command.");
                }
            }
            catch (Exception ex)
            {
                Errorlog.Print(ex, this);
            }
        }

        private void DisconnectClientHostsByUserPort(ConnectionHostData.MainCommunication ReceiveData)
        {
            try
            {
                this.List_DataConnection.Find(this.SelectConnectionByUser).DisConnect();
                this.List_DataConnection.RemoveAll(this.SelectConnectionByUser);
            }
            catch (Exception ex)
            {
                Errorlog.Print(ex, this);
            }
        }

        private bool SelectConnectionByUser(DataConnection obj)
        {
            return (this.ReceiveData.connection.SenderID == obj.SenderClient.clientstatus.ReceiverPort) && (this.ReceiveData.connection.ReceiverID == obj.ReceiverClient.clientstatus.ReceiverPort);
        }

        private void DisconnectClientHostsByServerPort(ConnectionHostData.MainCommunication ReceiveData)
        {
            try
            {
                this.List_DataConnection.Find(this.SelectConnectionByServer).DisConnect();
                this.List_DataConnection.RemoveAll(this.SelectConnectionByServer);
            }
            catch (Exception ex)
            {
                Errorlog.Print(ex, this);
            }
        }

        private bool SelectConnectionByServer(DataConnection obj)
        {
            return (this.ReceiveData.connection.SenderID == obj.SenderClient.clientstatus.ServerPort) && (this.ReceiveData.connection.ReceiverID == obj.ReceiverClient.clientstatus.ServerPort); 
        }

        private void ConnectClientHostsByUserPort(ConnectionHostData.MainCommunication ReceiveData)
        {
            try
            {
                if (!(this.List_ClientHosts.Exists(IsSenderByUser) && this.List_ClientHosts.Exists(IsReceiverByUser))) return;
                this.List_DataConnection.Add(new DataConnection(this.List_ClientHosts.Find(IsSenderByUser), this.List_ClientHosts.Find(IsReceiverByUser)));
                this.List_DataConnection[this.List_DataConnection.Count - 1].Connect();
            }
            catch (Exception ex)
            {
                Errorlog.Print(ex, this);
            }
        }

        private bool IsReceiverByUser(ClientHost obj)
        {
            return this.ReceiveData.connection.ReceiverID == obj.clientstatus.ReceiverPort;
        }

        private bool IsSenderByUser(ClientHost obj)
        {
            return this.ReceiveData.connection.SenderID == obj.clientstatus.ReceiverPort;
        }

        private void ConnectClientHostsByServerPort(ConnectionHostData.MainCommunication ReceiveData)
        {
            try
            {
                if(!(this.List_ClientHosts.Exists(IsSenderByServer) && this.List_ClientHosts.Exists(IsReceiverByServer)))return;
                this.List_DataConnection.Add(new DataConnection(this.List_ClientHosts.Find(IsSenderByServer), this.List_ClientHosts.Find(IsReceiverByServer)));
                this.List_DataConnection[this.List_DataConnection.Count - 1].Connect();
            }
            catch (Exception ex)
            {
                Errorlog.Print(ex, this);
            }
        }

        private bool IsReceiverByServer(ClientHost obj)
        {
            return this.ReceiveData.connection.ReceiverID == obj.clientstatus.ServerPort;
        }

        private bool IsSenderByServer(ClientHost obj)
        {
            return this.ReceiveData.connection.SenderID == obj.clientstatus.ServerPort; 
        }


        private void DisconnectClientHosts(ConnectionHostData.MainCommunication ReceiveData)
        {
            try
            {
                this.List_DataConnection.Find(this.SelectConnection).DisConnect();
                this.List_DataConnection.RemoveAll(this.SelectConnection);
            }
            catch (Exception ex)
            {
                Errorlog.Print(ex, this);
            }
        }

        public void DisconnectClientHostsCloseByIndex(int id)
        {
            try
            {
                if (!(this.List_DataConnection.Count >= id)) return;
                this.List_DataConnection[id].DisConnect();
                this.List_DataConnection.RemoveAt(id);
            }
            catch (Exception ex)
            {
                Errorlog.Print(ex, this);
            }
        }

        private bool SelectConnection(DataConnection obj)
        {
            if (obj.SenderID == ReceiveData.connection.SenderID
                && obj.ReceiverID == ReceiveData.connection.ReceiverID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ConnectClientHosts(ConnectionHostData.MainCommunication ReceiveData)
        {
            try
            {
                if (!(this.List_ClientHosts.Count >= ReceiveData.connection.SenderID && this.List_ClientHosts.Count >= ReceiveData.connection.ReceiverID))
                {
                    Report.Print("Indecies are over flow.", this);
                    return;
                }
                this.List_DataConnection.Add(new DataConnection(ReceiveData.connection.SenderID, ReceiveData.connection.ReceiverID, this.List_ClientHosts));
                this.List_DataConnection[this.List_DataConnection.Count - 1].Connect();
            }
            catch (Exception ex)
            {
                Errorlog.Print(ex, this);
            }
        }

        public void ConnectClientHostsByIndex(int senderindex, int receiverindex)
        {
            try
            {
                if (!(this.List_ClientHosts.Count >= senderindex && this.List_ClientHosts.Count >= receiverindex))
                {
                    Report.Print("Indecies are over flow.", this);
                    return;
                }
                this.List_DataConnection.Add(new DataConnection(senderindex, receiverindex, this.List_ClientHosts));
                this.List_DataConnection[this.List_DataConnection.Count - 1].Connect();
            }
            catch (Exception ex)
            {
                Errorlog.Print(ex, this);
            }
        }

        private void CloseResponse()
        {
            try
            {
                ConnectionHostData.CloseResponse SendData = new ConnectionHostData.CloseResponse();
                this.client.Send(SendData.data);
            }
            catch (Exception ex)
            {
                Errorlog.Print(ex, this);
            }
        }

        private void NoticeAddedClientHost()
        {
            try
            {
                ConnectionHostData.NoticePort SendData = new ConnectionHostData.NoticePort(this.CurrentPort);
                this.client.Send(SendData.SendData);
            }
            catch (Exception ex)
            {
                Errorlog.Print(ex, this);
            }
        }

        /// <summary>
        /// stop and remove client
        /// </summary>
        /// <param name="ReceiveData"></param>
        private void ClientHostDelete(ConnectionHostData.MainCommunication ReceiveData)
        {
            try
            {
                if (this.List_Port.Contains(ReceiveData.DeletePort))
                {
                    int id = this.List_Port.IndexOf(ReceiveData.DeletePort);

                    this.CheckAndDisconnectClientsConnection(id);

                    this.List_Port.RemoveAt(id);
                    this.List_ClientHosts[id].Close();
                    this.List_ClientHosts.RemoveAt(id);
                    Report.PrintDateBar(this);
                    Report.Print("Close Port : " + ReceiveData.DeletePort.ToString(), this);
                }
            }
            catch (Exception ex)
            {
                Errorlog.Print(ex, this);
            }
        }

        public void ClientHostDeleteByID(int id)
        {
            try
            {
                if (this.List_Port.Count >= id)
                {
                    this.CheckAndDisconnectClientsConnection(id);

                    this.List_Port.RemoveAt(id);
                    this.List_ClientHosts[id].Close();
                    this.List_ClientHosts.RemoveAt(id);
                    Report.PrintDateBar(this);
                    Report.Print("Close Port : " + ReceiveData.DeletePort.ToString(), this);
                }
            }
            catch (Exception ex)
            {
                Errorlog.Print(ex, this);
            }
        }

        private int ID;
        private void CheckAndDisconnectClientsConnection(int id)
        {
            try
            {
                if (this.List_DataConnection.Count == 0)
                {
                    return;
                }
                this.ID = id;
                int connectionid = this.List_DataConnection.FindIndex(SelectIndexOfConnectionByClientId);
                this.List_DataConnection[connectionid].DisConnect();
                this.List_DataConnection.RemoveAt(connectionid);
            }
            catch (Exception ex)
            {
                Errorlog.Print(ex, this);
            }
        }

        private bool SelectIndexOfConnectionByClientId(DataConnection obj)
        {
            if (obj.SenderID == ID || obj.ReceiverID == ID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// add and start client.
        /// </summary>
        private void ClientHostAdd()
        {
            try
            {
                for (int i = this.StartPort; i < this.StartPort + 200; i++)
                {
                    if (!this.List_Port.Contains(i))
                    {
                        this.List_ClientHosts.Add(new ClientHost(i));
                        this.List_Port.Add(i);
                        this.CurrentPort = i;
                        Report.PrintDateBar(this);
                        Report.Print("Open Port : " + i.ToString(), this);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Errorlog.Print(ex,this);
            }
        }

        private void Init_Classes()
        {
            this.settings = new Settings();
            this.List_ClientHosts = new List<ClientHost>();
            this.List_Port = new List<int>();
            this.List_DataConnection = new List<DataConnection>();
        }

        public void Load_Settings()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));

                if (System.IO.File.Exists(SettingsPath))
                {

                    System.IO.FileStream fs = new System.IO.FileStream(SettingsPath, System.IO.FileMode.Open);
                    this.settings = (Settings)serializer.Deserialize(fs);
                    fs.Close();
                    Report.PrintDateBar(this);
                    Report.Print("Setting Loaded.", this);
                    Report.Print("SettingFilePath : " + this.SettingsPath, this);
                    Report.Print("PortNum : " + this.settings.PortNum, this);
                    Report.Print("StartPort : " + this.settings.StartPort, this);
                }
                else
                {
                    this.settings.Initialize();
                    this.WriteSettings();
                }
            }
            catch (Exception ex)
            {
                Errorlog.Print(ex, this);
            }
        }

        private void WriteSettings()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            System.IO.FileStream fs = new System.IO.FileStream(SettingsPath, System.IO.FileMode.Create);
            serializer.Serialize(fs, settings);
            fs.Close();
        }

        public void Close()
        {
            foreach (var p in this.List_ClientHosts)
            {
                p.Close();
            }

            this.client.DataReceived -= client_DataReceived;
            this.client.Close();
            this.client = null;

        }

        internal void PrintState()
        {
            Report.PrintDateBar(this);
            Report.Print("ClientNum : " + this.List_ClientHosts.Count, this);
            Report.Print("ConnectionNum : " + this.List_DataConnection.Count, this);
            foreach (var p in this.List_ClientHosts)
            {
                var t = p.clientstatus;
                Report.Print("ID:" + this.List_ClientHosts.IndexOf(p).ToString() +" "+t.Name + " " + t.ServerPort + " " + t.ReceiverIP + " " + t.ReceiverPort + " " + t.Mode + " " + t.FPS, p);
            }
            foreach (var p in this.List_DataConnection)
            {
                Report.Print("ID:" + this.List_DataConnection.IndexOf(p).ToString() +" " +p.SenderID.ToString() + " => " + p.ReceiverID.ToString(), p);
            }
        }
    }
}
