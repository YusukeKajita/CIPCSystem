using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;

namespace CentralInterProcessCommunicationServer
{
    public class RemoteHostServer
    {
        #region private field
        private UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT client;
        private UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc;
        private UDP_PACKETS_CODER.UDP_PACKETS_DECODER dec;

        private const int myPort = Definitions.REMOTEHOSTSERVER_PORT;

        private const int max_connect_num = 200;

        public DebugWindow debugwindow { set; get; }
        //public TerminalConnection.TerminalConnection terminalconnection { set; get; }
        public TerminalConnectionSettings.CommandEventer Eventer { set; get; }
        #region server
        /// <summary>
        /// 現在保持しているリモートホストおよびそのタスクのリスト
        /// </summary>
        private List<CentralInterProcessCommunicationServer.RemoteHost> List_remotehost;
        private int id_port = Definitions.PORT_START_NUMBER;
        #endregion

        /// <summary>
        /// 現在保持しているポートのリスト
        /// </summary>
        private List<int> LstPort;

        #endregion

        #region propaty
        public List<CentralInterProcessCommunicationServer.RemoteHost> List_RemoteHost
        {
            get
            {
                return this.List_remotehost;
            }
        }
        public int server_port
        {
            get
            {
                return myPort;
            }
        }

        public int host_num
        {
            get
            {
                return LstPort.Count;
            }
        }
        public int Num_Port
        {
            get
            {
                return this.List_remotehost.Count;
            }
        }
        public MainWindow parent { set; get; }
        public RemoteOperater remoteOperator { set; get; }
        #endregion

        #region constructer
        public RemoteHostServer()
        {
            try
            {
                this.Eventer = new TerminalConnectionSettings.CommandEventer();
                client = new UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT(myPort);
                this.remoteOperator = new RemoteOperater(client);
            }
            catch (Exception ex)
            {
                while (true)
                {
                    myDialog dialog = new myDialog(ex.Message);
                    if (dialog.ShowDialog() == true)
                    {
                        break;
                    }
                }
            }

            try
            {
                this.enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
                this.dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
                this.LstPort = new List<int>();

            }
            catch (Exception ex)
            {
                while (true)
                {
                    myDialog dialog = new myDialog(ex.Message);
                    if (dialog.ShowDialog() == true)
                    {
                        break;
                    }
                }
            }
            try
            {
                #region create remotehost servers list and one remotehost server
                this.List_remotehost = new List<RemoteHost>();
                #endregion
                client.DataReceived += client_DataReceived;
            }
            catch (Exception ex)
            {
                while (true)
                {
                    myDialog dialog = new myDialog(ex.Message);
                    if (dialog.ShowDialog() == true)
                    {
                        break;
                    }
                }
            }
        }

        void client_DataReceived(object sender, byte[] e)
        {
            int id_port = 0;
            bool Is_Connected = false;
            this.dec.Source = e;
            update(ref id_port, ref Is_Connected);
        }
        #endregion

        private bool atPort(RemoteHost obj)
        {
            if (int.Parse(obj.remotePort) == id_port)
            {
                return true;
            }
            else
            {
                return false;
            }
        }




        #region methods

        /// <summary>
        /// サーバがアクセスされるまで待機し、アクセスされた場合そのメッセージを検査し、その情報に従って
        /// ポートを割り振る、もしくは終了します。
        /// </summary>
        /// <returns></returns>
        private void update(ref int Port, ref bool isConnect)
        {
            try
            {

                this.debugwindow.DebugLog = "[RemoteHostServer]操作を開始します．RemoteEP：" + this.client.RemoteEP.ToString();
                switch (dec.get_int())
                {
                    case Definitions.CONNECTION_DEMANDS:
                        this.connect(ref Port);
                        isConnect = true;
                        this.id_port = Port;
                        this.List_remotehost.Add(new RemoteHost(id_port, this.debugwindow/*, this.terminalconnection*/));
                        break;
                    case Definitions.CONNECTION_END:
                        this.disconnect(dec.get_int());
                        isConnect = false;
                        this.id_port = Port;
                        this.List_remotehost.RemoveAll(atPort);
                        break;
                    case Definitions.CONNECTION_SERVER_OPERATE:
                        //mainwindowを毎回追加
                        this.remoteOperator.mainwindow = this.parent;
                        this.remoteOperator.RHS = this;
                        this.remoteOperator.DCS = this.parent.DataConnectionServer;
                        this.remoteOperator.addRemoteEP();
                        string message = dec.get_string();
                        this.debugwindow.DebugLog = "[TerminalConnection]受信:" + message;
                        Eventer.Handle(message);
                        try
                        {
                            this.remoteOperator.sendStatesToAllEP();
                        }
                        catch
                        {
                            this.debugwindow.DebugLog = "送信できません";
                        }
                        this.remoteOperator.UpdateListBox();
                        break;

                    default:
                        this.debugwindow.DebugLog = "要求された信号に対応するコマンドが存在しません。リモートホスト側のコネクトの設定を確認してください。";
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "ポートを割り振るサーバのアップデートでエラーが発生しました。");
            }
        }

        private void connect(ref int port)
        {
            try
            {
                for (int current_port = Definitions.PORT_START_NUMBER; current_port < Definitions.PORT_START_NUMBER + max_connect_num; current_port++)
                {
                    if (!this.LstPort.Contains(current_port))
                    {
                        port = current_port;
                        this.LstPort.Add(current_port);

                        this.enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
                        this.enc += Definitions.CONNECTION_OK;
                        this.enc += current_port;
                        this.client.Send(this.enc.data);

                        this.debugwindow.DebugLog = "[RemoteHostServer]接続要求がきました．解放ポートを通知します．ポート番号：" + port.ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                debugwindow.DebugLog = ex.Message + "コネクトの段階でエラーが発生しました。";
            }
        }

        private int destroyport;
        private void disconnect(int port)
        {
            try
            {
                this.debugwindow.DebugLog = "[RemoteHostServer]切断要求がきました．ポート番号：" + port.ToString();

                destroyport = port;
                var connections = this.parent.DataConnectionServer.List_dataconnection.FindAll(p => p.SENDER.ID == port || p.RECEIVER.ID == port);
                foreach (var p in connections)
                {
                    this.parent.DataConnectionServer.delete_connection(p.SENDER, p.RECEIVER);
                }
                this.parent.DataConnectionServer.ListBox_update(parent.LISTBOX_DATA_CONNECTION);

                this.LstPort.Remove(port);
                this.List_remotehost.RemoveAll(check_remotehosts);

                this.enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
                this.enc += Definitions.CONNECTION_END;
                this.client.Send(this.enc.data);
            }
            catch (Exception ex)
            {
                debugwindow.DebugLog = ex.Message + "接続を終了する段階でエラーが発生しました。";
            }
        }

        /// <summary>
        /// ポートを指定して強制的にリモートホストを削除します
        /// </summary>
        /// <param name="port"></param>
        public void ShutDownDisconnect(int port)
        {
            try
            {
                this.debugwindow.DebugLog = "[RemoteHostServer]強制切断します．ポート番号：" + port.ToString();
                destroyport = port;
                this.LstPort.Remove(port);
                this.List_remotehost.RemoveAll(check_remotehosts);

                this.remoteOperator.sendStatesToAllEP();
            }
            catch (Exception ex)
            {
                debugwindow.DebugLog = ex.Message + "接続を終了する段階でエラーが発生しました。";
            }
        }

        private bool check_remotehosts(RemoteHost obj)
        {
            if (obj.ID == destroyport)
            {
                obj.disconnect();
                return true;
            }
            else
            {
                return false;
            }
        }


        #region ListAdd
        CollectionViewSource view = new CollectionViewSource();
        public ObservableCollection<Client> Clients = new ObservableCollection<Client>();
        public void Listupdate(ListView LISTVIEW)
        {

            Clients.Clear();
            if (this.List_remotehost.Count >= 0)
            {
                for (int i = 0; i < this.List_remotehost.Count; i++)
                {
                    Clients.Add(new Client()
                    {
                        Name = this.List_remotehost[i].Name,
                        Port = this.List_remotehost[i].ID,
                        remoteIP = this.List_remotehost[i].remoteIP,
                        remotePort = this.List_remotehost[i].remotePort,
                        Connect_Name = this.List_remotehost[i].Connect_Name,
                        FPS = this.List_remotehost[i].Fps,
                        mode = this.List_remotehost[i].Connection_Mode.ToString()
                    });
                }
            }
            view.Source = Clients;
            LISTVIEW.DataContext = view;
        }
        #endregion
        #endregion
    }
    public class Client
    {
        public string Name { set; get; }
        public int Port { set; get; }
        public string remoteIP { set; get; }
        public string remotePort { set; get; }
        public string Connect_Name { set; get; }
        public int FPS { set; get; }
        public string mode { set; get; }
    }
}
