using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using TerminalConnectionSettings;

namespace CentralInterProcessCommunicationServer.TerminalConnection
{
    /// <summary>
    /// terminalとコネクションするホスト
    /// 2014/10/14 update前まで完了　メモリーストリームの作成　送信形式　プロトコルに注意すること
    /// http://dobon.net/vb/dotnet/internet/tcpclientserver.html
    /// </summary>
    //public class TerminalConnection : IDisposable
    //{
    //    #region field
    //    #region const
    //    /// <summary>
    //    /// TCP接続時のポート
    //    /// </summary>
    //    public const int tcpport = Definitions.TERMINALCONNECTION_PORT;
    //    #endregion
    //    #region variable
        

    //    #endregion
    //    #region class
    //    #region TCP classes
    //    public const int buffermax = 1024;
    //    public TerminalConnectionSettings.NetworkData receive = new TerminalConnectionSettings.NetworkData(buffermax);
    //    public TerminalConnectionSettings.NetworkData send = new TerminalConnectionSettings.NetworkData(buffermax);
    //    /// <summary>
    //    /// TCP接続のリスナー　受け取ったらTCP接続用クライアントをインスタンス化する
    //    /// </summary>
    //    public System.Net.Sockets.TcpListener tcpListener;
    //    public System.Net.Sockets.TcpClient tcpClient;
    //    public System.Net.Sockets.NetworkStream networkstream;
    //    #endregion
    //    #region around tasks
    //    /// <summary>
    //    /// メインタスク
    //    /// </summary>
    //    public Task mytask;
    //    /// <summary>
    //    /// メインタスクに付随する停止用クラス
    //    /// </summary>
    //    public CancellationTokenSource CTS;
    //    #endregion
    //    #region terminalprotocols
    //    public TerminalConnectionSettings.CommandEventer Eventer { set; get; }
    //    #endregion
    //    #endregion
    //    #endregion

    //    #region Property
    //    public DebugWindow debugwindow { set; get; }
    //    public CentralInterProcessCommunicationServer.RemoteHostServer RHS { set; get; }
    //    public CentralInterProcessCommunicationServer.DATA_CONNECTION.DataConnectionServer DCS { set; get; }
    //    public MainWindow mainwindow { set; get; }
    //    #endregion


    //    #region constructer
    //    /// <summary>
    //    /// ターミナル用のコネクションを作成します．
    //    /// 使用通信形式はTCPで，使用する応答形式はDeffinitionsに遵守します．
    //    /// 使用するタスクは１つで，キャンセラレーショントークンソースを利用します．
    //    /// また，イベントを登録することで各自の要求を非同期に処理することができます．
    //    /// 詳しくは開発者に問い合わせてください．
    //    /// kajita yusuke
    //    /// yozora1080@gmail.com
    //    /// </summary>
    //    public TerminalConnection(){
    //        this.Init_Eventer();
    //        this.Init_Task();
            
    //    }
    //    #endregion

    //    #region methods
    //    #region 各クラスの初期化群
    //    /// <summary>
    //    /// TCPリスナーの初期化
    //    /// </summary>
    //    void Init_TCPlistener()
    //    {
    //        try
    //        {
    //            if (this.tcpListener != null)
    //            {
    //                this.tcpListener.Stop();
    //                this.tcpListener = null;
    //            }
    //            this.tcpListener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Any, TerminalConnection.tcpport);
    //            this.tcpListener.Server.SetSocketOption(System.Net.Sockets.SocketOptionLevel.IPv6, System.Net.Sockets.SocketOptionName.IPv6Only, 0);
    //            this.tcpListener.Start();
    //        }
    //        catch (Exception ex)
    //        {
    //            this.debugwindow.DebugLog = "[TerminalConnection]" + ex.Message;
    //        }
    //    }

    //    /// <summary>
    //    /// networkstreamを初期化します．Tcplistenerから取得します．
    //    /// </summary>
    //    private void Init_tcpclient()
    //    {
    //        this.tcpClient = this.tcpListener.AcceptTcpClient();
    //    }


    //    /// <summary>
    //    /// networkstreamを初期化します．TcpClientから取得します．
    //    /// </summary>
    //    private void Init_networkstream()
    //    {
    //        this.networkstream = this.tcpClient.GetStream();
    //    }

    //    /// <summary>
    //    /// タスクの初期化　すでに使用時には初期化する
    //    /// 初期化の際に計200msの待機時間がある
    //    /// </summary>
    //    void Init_Task()
    //    {
    //        try
    //        {
    //            if (this.CTS != null)
    //            {
    //                this.CTS.Cancel();
    //                Thread.Sleep(100);
    //                this.CTS = null;
    //                this.mytask = null;
    //                Thread.Sleep(100);
    //            }
    //            this.CTS = new CancellationTokenSource();
    //            this.mytask = new Task(maintask, this.CTS);
    //            this.mytask.Start();
    //        }
    //        catch(Exception ex)
    //        {
    //            this.debugwindow.DebugLog = "[TerminalConnection]" + ex.Message;
    //        }
    //    }

    //    void Init_Eventer()
    //    {
    //        try
    //        {
    //            this.Eventer = new CommandEventer();
    //        }
    //        catch (Exception ex)
    //        {
    //            this.debugwindow.DebugLog = "[TerminalConnection]" + ex.Message;
    //        }
    //    }
    //    #endregion

    //    /// <summary>
    //    /// メインタスク
    //    /// リスナーが接続要求を受けるまで待機し，それが完了するとTCP接続クライアントを作成しTCP通信を開始する．
    //    /// TCP接続によって情報を受けるたびにそれを解析し，システムにイベントハンドラで命令をします．
    //    /// また，システムの現在の情報を相手クライアントに逐次（フレームを指定可能）送信します．
    //    /// </summary>
    //    /// <param name="obj"></param>
    //    private void maintask(object obj)
    //    {
    //        while (!this.CTS.IsCancellationRequested)
    //        {
    //            try
    //            {

    //                this.Setup_in_Task();
    //                while (!this.CTS.IsCancellationRequested)
    //                {
    //                    this.Update_in_Task();
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                this.debugwindow.DebugLog = "[TerminalConnection]" + ex.Message + ex.Source;
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// タスク内で行うループ処理前の初期動作処理　TCPのリスナーはここで待機　タスクを利用しているため，非同期処理となる
    //    /// </summary>
    //    private void Setup_in_Task()
    //    {
    //        this.Init_TCPlistener();
    //        this.debugwindow.DebugLog = "[TerminalConnection]接続受付を開始します．ポート：" + TerminalConnection.tcpport.ToString();
    //        this.Init_tcpclient();
    //        this.debugwindow.DebugLog = "[TerminalConnection]接続を確立しました．remoteEP:" + this.tcpClient.Client.RemoteEndPoint.ToString();
    //        this.Init_networkstream();
            
    //    }
        
    //    /// <summary>
    //    /// タスク内で行うループ処理　中身
    //    /// </summary>
    //    private void Update_in_Task()
    //    {
    //        try
    //        {
    //            if (this.tcpClient != null && this.networkstream != null)
    //            {
    //                this.Tcp_Receive();
    //                this.Eventer.Handle(receive.stringData);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            throw;
    //        }
    //    }

    //    /// <summary>
    //    /// CIPCの状態を送信する
    //    /// </summary>
    //    public void Tcp_Send()
    //    {
    //        try
    //        {
    //            List<TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Client> clientlist = new List<TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Client>();
    //            List<TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Connection> connectionlist = new List<TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Connection>();
    //            if (this.RHS.List_RemoteHost.Count > 0)
    //            {
    //                foreach (var count in this.RHS.List_RemoteHost)
    //                {
    //                    TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Client.Mode mode;
    //                    if (count.Connection_Mode == ConnectionState.Sender)
    //                    {
    //                        mode = TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Client.Mode.Sender;
    //                    }
    //                    else if (count.Connection_Mode == ConnectionState.Receiver)
    //                    {
    //                        mode = TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Client.Mode.Receiver;
    //                    }
    //                    else
    //                    {
    //                        mode = TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Client.Mode.Default;
    //                    }

    //                    clientlist.Add(new TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Client(count.ID, count.Name, int.Parse(count.remotePort), count.remoteIP, count.Fps, mode));
    //                }
    //            }
    //            if (this.DCS.List_dataconnection.Count > 0)
    //            {
    //                foreach (var count in this.DCS.List_dataconnection)
    //                {
    //                    connectionlist.Add(new TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Connection(count.SENDER.ID, count.RECEIVER.ID));
    //                }
    //            }


    //            bool IsSyncConnect=false;
    //            System.Windows.Threading.DispatcherOperation DO = this.mainwindow.Dispatcher.BeginInvoke(new Action(() =>
    //            {
    //                IsSyncConnect = this.mainwindow.CheckBox_IsSyncReceived.IsChecked == true ? true : false;
    //            }));
    //            DO.Wait();

    //            TerminalConnectionSettings.ServerProtocols.ReportInfo reportinfo = new TerminalConnectionSettings.ServerProtocols.ReportInfo(new TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo(clientlist, connectionlist,IsSyncConnect));

    //            this.send.stringData = reportinfo.Data;
    //            this.networkstream.Write(this.send.encodedbytes, 0, this.send.encodedbytes.Length);
    //            this.debugwindow.DebugLog = "[TerminalConnection]送信:" + this.send.stringData;
    //        }
    //        catch (Exception ex)
    //        {
    //            this.debugwindow.DebugLog = "[" + this + "]" + ex.Message;
    //        }
    //    }

    //    #endregion
    //    /// <summary>
    //    /// TCPによる遠隔操作の受付
    //    /// </summary>
    //    private void Tcp_Receive()
    //    {
    //        this.receive.DataIndex = 0;
    //        while (this.receive.string_set(this.networkstream.Read(this.receive.buffer, 0, this.receive.buffer.Length)) == this.receive.buffer.Length)
    //        {
                
    //        }
    //        this.debugwindow.DebugLog = "[TerminalConnection]受信:" + this.receive.stringData;
    //    }

    //    public void Dispose()
    //    {
    //        this.CTS.Cancel();
    //    }
    //}
}
