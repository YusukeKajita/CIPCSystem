using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerminalConnectionSettings
{
    /// <summary>
    /// 文字列情報のコマンドをイベントにして実行します。
    /// イベントの登録は各自インスタンスに適用します。
    /// </summary>
    public class CommandEventer
    {
        #region Events
        public delegate void ConnectEventHandler(object sender, TerminalProtocols.Connect e);
        public event ConnectEventHandler Connect;
        protected virtual void OnConnect(TerminalProtocols.Connect e)
        {
            if (this.Connect != null) { this.Connect(this, e); }
        }
        public delegate void ConnectByNameEventHandler(object sender, TerminalProtocols.ConnectByName e);
        public event ConnectByNameEventHandler ConnectByName;
        protected virtual void OnConnectByName(TerminalProtocols.ConnectByName e)
        {
            if (this.ConnectByName != null) { this.ConnectByName(this, e); }
        }

        public delegate void DisConnectEventHandler(object sender, TerminalProtocols.DisConnect e);
        public event DisConnectEventHandler DisConnect;
        protected virtual void OnDisConnect(TerminalProtocols.DisConnect e)
        {
            if (this.DisConnect != null) { this.DisConnect(this, e); }
        }

        public delegate void DisConnectByNameEventHandler(object sender, TerminalProtocols.DisConnectByName e);
        public event DisConnectByNameEventHandler DisConnectByName;
        protected virtual void OnDisConnectByName(TerminalProtocols.DisConnectByName e)
        {
            if (this.DisConnectByName != null) { this.DisConnectByName(this, e); }
        }

        public delegate void CloseEventHandler(object sender, TerminalProtocols.Close e);
        public event CloseEventHandler Close;
        protected virtual void OnClose(TerminalProtocols.Close e)
        {
            if (this.Close != null) { this.Close(this, e); }
        }

        public delegate void RestartEventHandler(object sender, TerminalProtocols.Restart e);
        public event RestartEventHandler Restart;
        protected virtual void OnRestart(TerminalProtocols.Restart e)
        {
            if (this.Restart != null) { this.Restart(this, e); }
        }

        public delegate void DemmandInfoEventHandler(object sender, TerminalProtocols.DemmandInfo e);
        public event DemmandInfoEventHandler DemmandInfo;
        protected virtual void OnDemmandInfo(TerminalProtocols.DemmandInfo e)
        {
            if (this.DemmandInfo != null) { this.DemmandInfo(this, e); }
        }

        public delegate void UndoEventHandler(object sender, TerminalProtocols.Undo e);
        public event UndoEventHandler Undo;
        protected virtual void OnUndo(TerminalProtocols.Undo e)
        {
            if (this.Undo != null) { this.Undo(this, e); }
        }

        public delegate void RedoEventHandler(object sender, TerminalProtocols.Redo e);
        public event RedoEventHandler Redo;
        protected virtual void OnRedo(TerminalProtocols.Redo e)
        {
            if (this.Redo != null) { this.Redo(this, e); }
        }

        public delegate void EmergenceTEventHandler(object sender, TerminalProtocols.Emergence e);
        public event EmergenceTEventHandler EmergenceT;
        protected virtual void OnEmergenceT(TerminalProtocols.Emergence e)
        {
            if (this.EmergenceT != null) { this.EmergenceT(this, e); }
        }

        public delegate void ReportInfoEventHandler(object sender, ServerProtocols.ReportInfo e);
        public event ReportInfoEventHandler ReportInfo;
        protected virtual void OnReportInfo(ServerProtocols.ReportInfo e)
        {
            if (this.ReportInfo != null) { this.ReportInfo(this, e); }
        }

        public delegate void AllDisConnectEventHandler(object sender, TerminalProtocols.AllDisConnect e);
        public event AllDisConnectEventHandler AllDisConnect;
        protected virtual void OnAllDisConnect(TerminalProtocols.AllDisConnect e)
        {
            if (this.AllDisConnect != null) { this.AllDisConnect(this, e); }
        }
        public delegate void LoadConnectionFastEventHandler(object sender, TerminalProtocols.LoadConnectionFast e);
        public event LoadConnectionFastEventHandler LoadConnectionFast;
        protected virtual void OnLoadConnectionFast(TerminalProtocols.LoadConnectionFast e)
        {
            if (this.LoadConnectionFast != null) { this.LoadConnectionFast(this, e); }
        }
        public delegate void SaveConnectionFastEventHandler(object sender, TerminalProtocols.SaveConnectionFast e);
        public event SaveConnectionFastEventHandler SaveConnectionFast;
        protected virtual void OnSaveConnectionFast(TerminalProtocols.SaveConnectionFast e)
        {
            if (this.SaveConnectionFast != null) { this.SaveConnectionFast(this, e); }
        }

        public delegate void TurnOnSyncConnectEventHandler(object sender, TerminalProtocols.TurnOnSyncConnect e);
        public event TurnOnSyncConnectEventHandler TurnOnSyncConnect;
        protected virtual void OnTurnOnSyncConnect(TerminalProtocols.TurnOnSyncConnect e)
        {
            if (this.TurnOnSyncConnect != null) { this.TurnOnSyncConnect(this, e); }
        }
        public delegate void TurnOffSyncConnectEventHandler(object sender, TerminalProtocols.TurnOffSyncConnect e);
        public event TurnOffSyncConnectEventHandler TurnOffSyncConnect;
        protected virtual void OnTurnOffSyncConnect(TerminalProtocols.TurnOffSyncConnect e)
        {
            if (this.TurnOffSyncConnect != null) { this.TurnOffSyncConnect(this, e); }
        }
        #endregion

        #region propaty
        #endregion

        public CommandEventer()
        {

        }

        public void Handle(string data)
        {
            try
            {
                string[] srg = data.Split('\\');
                if (srg.Length == 0)
                {
                    return;
                }

                if (srg[0] == TerminalConnectionSettings.Sender.CIPCTerminal.ToString())
                {
                    if (srg[1] == TerminalCommand.Connect.ToString())
                    {
                        this.OnConnect(new TerminalProtocols.Connect(int.Parse(srg[2]), int.Parse(srg[3])));
                    }
                    else if (srg[1] == TerminalCommand.ConnectByName.ToString())
                    {
                        this.OnConnectByName(new TerminalProtocols.ConnectByName(srg[2], srg[3]));
                    }
                    else if (srg[1] == TerminalCommand.DisConnect.ToString())
                    {
                        this.OnDisConnect(new TerminalProtocols.DisConnect(int.Parse(srg[2]), int.Parse(srg[3])));
                    }
                    else if (srg[1] == TerminalCommand.DisConnectByName.ToString())
                    {
                        this.OnDisConnectByName(new TerminalProtocols.DisConnectByName(srg[2], srg[3]));
                    }
                    else if (srg[1] == TerminalCommand.Close.ToString())
                    {
                        this.OnClose(new TerminalProtocols.Close());
                    }
                    else if (srg[1] == TerminalCommand.Restart.ToString())
                    {
                        this.OnRestart(new TerminalProtocols.Restart());
                    }
                    else if (srg[1] == TerminalCommand.DemandInfo.ToString())
                    {
                        this.OnDemmandInfo(new TerminalProtocols.DemmandInfo());
                    }
                    else if (srg[1] == TerminalCommand.Undo.ToString())
                    {
                        this.OnUndo(new TerminalProtocols.Undo());
                    }
                    else if (srg[1] == TerminalCommand.Redo.ToString())
                    {
                        this.OnRedo(new TerminalProtocols.Redo());
                    }
                    else if (srg[1] == TerminalCommand.Emergence.ToString())
                    {
                        this.OnEmergenceT(new TerminalProtocols.Emergence());
                    }
                    else if (srg[1] == TerminalCommand.AllDisConnect.ToString())
                    {
                        this.OnAllDisConnect(new TerminalProtocols.AllDisConnect());
                    }
                    else if (srg[1] == TerminalCommand.LoadConnectionFast.ToString())
                    {
                        this.OnLoadConnectionFast(new TerminalProtocols.LoadConnectionFast());
                    }
                    else if (srg[1] == TerminalCommand.SaveConnectionFast.ToString())
                    {
                        this.OnSaveConnectionFast(new TerminalProtocols.SaveConnectionFast());
                    }
                    else if (srg[1] == TerminalCommand.TurnOnSyncConnect.ToString())
                    {
                        this.OnTurnOnSyncConnect(new TerminalProtocols.TurnOnSyncConnect());
                    }
                    else if (srg[1] == TerminalCommand.TurnOffSyncConnect.ToString())
                    {
                        this.OnTurnOffSyncConnect(new TerminalProtocols.TurnOffSyncConnect());
                    }
                    else
                    {
                        return;
                    }
                }
                else if (srg[0] == TerminalConnectionSettings.Sender.CIPCSever.ToString())
                {
                    if (srg[1] == TerminalConnectionSettings.ServerCommand.ReportInfo.ToString())
                    {
                        int index = 2;
                        List<ServerProtocols.ReportInfo.CIPCInfo.Client> ClientList = new List<ServerProtocols.ReportInfo.CIPCInfo.Client>();
                        List<ServerProtocols.ReportInfo.CIPCInfo.Connection> ConnectionList = new List<ServerProtocols.ReportInfo.CIPCInfo.Connection>();

                        int clientlength = int.Parse(srg[index]);
                        index++;

                        for (int i = 0; i < clientlength; i++)
                        {
                            int myport = int.Parse(srg[index]);
                            index++;
                            string name = srg[index];
                            index++;
                            int remoteport = int.Parse(srg[index]);
                            index++;
                            string remoteIP = srg[index];
                            index++;
                            int FPS = int.Parse(srg[index]);
                            index++;
                            ServerProtocols.ReportInfo.CIPCInfo.Client.Mode mode;
                            if (srg[index] == ServerProtocols.ReportInfo.CIPCInfo.Client.Mode.Sender.ToString())
                            {
                                mode = ServerProtocols.ReportInfo.CIPCInfo.Client.Mode.Sender;
                                index++;
                            }
                            else if (srg[index] == ServerProtocols.ReportInfo.CIPCInfo.Client.Mode.Receiver.ToString())
                            {
                                mode = ServerProtocols.ReportInfo.CIPCInfo.Client.Mode.Receiver;
                                index++;
                            }
                            else
                            {
                                mode = ServerProtocols.ReportInfo.CIPCInfo.Client.Mode.Default;
                                index++;
                            }
                            ClientList.Add(new ServerProtocols.ReportInfo.CIPCInfo.Client(myport, name, remoteport, remoteIP, FPS, mode));
                        }

                        int connectionlength = int.Parse(srg[index]);
                        index++;

                        for (int i = 0; i < connectionlength; i++)
                        {
                            int senderport = int.Parse(srg[index]);
                            index++;
                            int receiverport = int.Parse(srg[index]);
                            index++;
                            ConnectionList.Add(new ServerProtocols.ReportInfo.CIPCInfo.Connection(senderport, receiverport));
                        }

                        bool IsSyncConnect = bool.Parse(srg[index]);
                        index++;

                        ServerProtocols.ReportInfo reportinfo = new ServerProtocols.ReportInfo(new ServerProtocols.ReportInfo.CIPCInfo(ClientList, ConnectionList,IsSyncConnect));
                        this.OnReportInfo(reportinfo);
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
