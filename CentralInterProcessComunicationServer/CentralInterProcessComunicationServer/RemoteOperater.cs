using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralInterProcessCommunicationServer
{
    public class RemoteOperater
    {
        public HashSet<System.Net.IPEndPoint> m_SetIPEndPoint;
        public UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT Client { private set; get; }
        public DATA_CONNECTION.DataConnectionServer DCS { set; get; }
        public RemoteHostServer RHS { set; get; }
        public MainWindow mainwindow { set; get; }
        public DebugWindow debugwindow { set; get; }

        public RemoteOperater(UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT client)
        {
            m_SetIPEndPoint = new HashSet<System.Net.IPEndPoint>();

            this.Client = client;
        }

        public bool addRemoteEP()
        {
            return this.addRemoteEP(this.Client.RemoteEP);
        }
        public bool addRemoteEP(System.Net.IPEndPoint item)
        {
            return this.m_SetIPEndPoint.Add(item);
        }

        public bool removeRemoteEP()
        {
            return this.removeRemoteEP(this.Client.RemoteEP);
        }

        public bool removeRemoteEP(System.Net.IPEndPoint item)
        {
            return this.m_SetIPEndPoint.Remove(item);
        }
        public void sendStatesToAllEP()
        {
            this.sendToAllEP(this.currentState);
        }
        public string currentState
        {
            get
            {
                List<TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Client> clientlist = new List<TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Client>();
                List<TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Connection> connectionlist = new List<TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Connection>();
                if (this.RHS.List_RemoteHost.Count > 0)
                {
                    foreach (var count in this.RHS.List_RemoteHost)
                    {
                        TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Client.Mode mode;
                        if (count.Connection_Mode == ConnectionState.Sender)
                        {
                            mode = TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Client.Mode.Sender;
                        }
                        else if (count.Connection_Mode == ConnectionState.Receiver)
                        {
                            mode = TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Client.Mode.Receiver;
                        }
                        else
                        {
                            mode = TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Client.Mode.Default;
                        }

                        clientlist.Add(new TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Client(count.ID, count.Name, int.Parse(count.remotePort), count.remoteIP, count.Fps, mode));
                    }
                }
                if (this.DCS.List_dataconnection.Count > 0)
                {
                    foreach (var count in this.DCS.List_dataconnection)
                    {
                        connectionlist.Add(new TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Connection(count.SENDER.ID, count.RECEIVER.ID));
                    }
                }


                bool IsSyncConnect = false;
                System.Windows.Threading.DispatcherOperation DO = this.mainwindow.Dispatcher.BeginInvoke(new Action(() =>
                {
                    IsSyncConnect = this.mainwindow.CheckBox_IsSyncReceived.IsChecked == true ? true : false;
                }));
                DO.Wait();

                TerminalConnectionSettings.ServerProtocols.ReportInfo reportinfo = new TerminalConnectionSettings.ServerProtocols.ReportInfo(new TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo(clientlist, connectionlist, IsSyncConnect));

                return reportinfo.Data;
            }
        }

        public void sendToAllEP(string str)
        {
            try
            {
                var tmp = this.Client.RemoteEP;
                foreach (var p in this.m_SetIPEndPoint)
                {
                    this.Client.RemoteEP = p;
                    UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
                    enc += str;
                    this.Client.Send(enc.data);
                }
                this.Client.RemoteEP = tmp;
            }
            catch (Exception ex)
            {
                this.debugwindow.DebugLog = "[" + this + "]" + ex.Message;
            }
        }

        public void UpdateListBox()
        {
            if (this.mainwindow == null) return;
            this.mainwindow.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.mainwindow.ListBox_RemoteOperater.ItemsSource = this.m_SetIPEndPoint.ToList();
            }));
        }
    }
}
