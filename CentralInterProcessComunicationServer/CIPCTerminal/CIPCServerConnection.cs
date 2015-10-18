using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CIPCTerminal
{
    public class CIPCServerConnection
    {
        //public System.Net.Sockets.TcpClient tcpclient { set; get; }
        //public System.Net.Sockets.NetworkStream ns { set; get; }

        public TCPConnectWindow window { set; get; }
        public myparts.DebugLog debuglog { set; get; }

        //private CancellationTokenSource CTS { set; get; }
        //private Task mytask { set; get; }

        //public TerminalConnectionSettings.NetworkData receive { set; get; }
        //public TerminalConnectionSettings.NetworkData send { set; get; }

        public UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT client;

        public TerminalConnectionSettings.CommandEventer Eventer { set; get; }

        public bool IsConnectionSetupped { set; get; }
        public bool IsConnectted { set; get; }

        public MainWindow mainwindow { set; get; }

        public CIPCServerConnection(TCPConnectWindow window)
        {
            this.Init_Classes(window);
            this.IsConnectionSetupped = false;
            this.IsConnectted = false;
        }

        private void Init_Classes(TCPConnectWindow window)
        {
            this.window = window;
            //this.receive = new TerminalConnectionSettings.NetworkData(1024);
            //this.send = new TerminalConnectionSettings.NetworkData(1024);
            this.Eventer = new TerminalConnectionSettings.CommandEventer();
            //this.Init_Task();
        }

        private System.DateTime ReceivedTime;

        public void SetupUDP()
        {
            try
            {
                if (!this.IsConnectionSetupped)
                {
                    var remoteport = this.window.connectionsetting.remoteport;
                    var remoteip = this.window.connectionsetting.IsConnectionLocal ? "127.0.0.1" : this.window.connectionsetting.remoteIP;
                    this.client = new UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT(remoteip, int.Parse(remoteport), 18000);
                    this.client.DataReceived += client_DataReceived;
                    //ちょっとやそっとのエラーでもがんがん復活するスタイル
                    this.client.IsRecast = true;

                    this.IsConnectionSetupped = true;
                    this.mainwindow.dispatchertimer.Tick += this.ConfirmCIPCServer;
                }
            }
            catch
            {
                throw;
            }
        }

        void client_DataReceived(object sender, byte[] e)
        {
            UDP_PACKETS_CODER.UDP_PACKETS_DECODER dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
            dec.Source = e;
            this.Eventer.Handle(dec.get_string());
            this.ReceivedTime = System.DateTime.Now;

        }

        private void ConfirmCIPCServer(object sender, EventArgs e)
        {
            this.Udp_Send(new TerminalConnectionSettings.TerminalProtocols.DemmandInfo());
            if (System.DateTime.Now - this.ReceivedTime > new System.TimeSpan(0, 0, 2))
            {
                this.IsConnectted = false;
            }
            else
            {
                this.IsConnectted = true;
            }
            this.window.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.window.TextBlock_ConnectionState.Text = "CIPCServerIP:" + this.client.RemoteEP.Address.ToString() + "\n"
                    + "CIPCServerPort:" + this.client.RemoteEP.Port.ToString() + "\n"
                    + "LastDemandInfoTime:" + System.DateTime.Now.ToString() + "\n"
                    + "LastReceivedTime:" + this.ReceivedTime.ToString() + "\n"
                    + "IsCIPCServerLive:" + this.IsConnectted;
            }));
        }


        public void closeUDP()
        {
            try
            {
                if (this.IsConnectionSetupped)
                {
                    this.client.IsRecast = false;
                    this.client.Close();

                    this.IsConnectionSetupped = false;
                    this.mainwindow.dispatchertimer.Tick -= this.ConfirmCIPCServer;
                }
            }
            catch
            {
                throw;
            }
        }

        //private void Init_Task()
        //{
        //    try
        //    {
        //        if (this.CTS != null)
        //        {
        //            this.CTS.Cancel();
        //            Thread.Sleep(100);
        //            this.CTS = null;
        //            this.mytask = null;
        //            Thread.Sleep(100);
        //        }
        //        this.CTS = new CancellationTokenSource();
        //        this.mytask = new Task(maintask, this.CTS);
        //        this.mytask.Start();
        //    }
        //    catch (Exception ex)
        //    {
        //        this.debuglog.DebugLogPrint = "[" + this.ToString() +"]" + ex.Message;
        //    }
        //}

        //private void maintask(object obj)
        //{
        //    while (!this.CTS.IsCancellationRequested)
        //    {
        //        try
        //        {

        //            this.Setup_in_Task();
        //            while (!this.CTS.IsCancellationRequested)
        //            {
        //                this.Update_in_Task();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            this.debuglog.DebugLogPrint = "[" + this.ToString() +"]" + ex.Message;
        //            this.mainwindow.DeleteContents();
        //        }
        //    }
        //}

        //private void Update_in_Task()
        //{
        //    if (this.tcpclient != null && this.ns != null)
        //    {
        //        this.Tcp_Receive();
        //        this.Eventer.Handle(receive.stringData);
        //    }
        //}

        public void Tcp_Send(TerminalConnectionSettings.TerminalProtocols.CIPCTerminalCommand terminalcommand)
        {
            try
            {
                //this.send.stringData = terminalcommand.Data;
                //this.ns.Write(this.send.encodedbytes, 0, this.send.encodedbytes.Length);
                //this.debuglog.DebugLogPrint = "[" + this.ToString() + "]送信:" + this.send.stringData;
            }
            catch (Exception ex)
            {
                this.debuglog.DebugLogPrint = this.debuglog.DebugLogPrint = "[" + this.ToString() + "]" + ex.Message;
            }
        }
        public void Udp_Send(TerminalConnectionSettings.TerminalProtocols.CIPCTerminalCommand terminalcommand)
        {
            try
            {
                if (this.IsConnectionSetupped)
                {
                    UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
                    enc += (int)20;
                    enc += (string)terminalcommand.Data;
                    this.client.Send(enc.data);
                }
                else
                {
                    throw new Exception("接続設定をしてください");
                }
            }
            catch (Exception ex)
            {
                this.debuglog.DebugLogPrint = this.debuglog.DebugLogPrint = "[" + this.ToString() + "]" + ex.Message;
            }
        }

        //private void Tcp_Receive()
        //{
        //    this.receive.DataIndex = 0;
        //    while (this.receive.string_set(this.ns.Read(this.receive.buffer, 0, this.receive.buffer.Length)) == this.receive.buffer.Length)
        //    {

        //    }
        //    this.debuglog.DebugLogPrint = "[" + this.ToString() + "]受信:" + this.receive.stringData;
        //}

        //private void Setup_in_Task()
        //{
        //    this.Init_tcpclient();

        //}

        //private void Init_tcpclient()
        //{
        //    int times = 0;
        //    while (!this.CTS.IsCancellationRequested)
        //    {
        //        try
        //        {
        //            if (this.window.IsConnectSetuped == true)
        //            {
        //                try
        //                {
        //                    this.debuglog.DebugLogPrint = "[" + this.ToString() +"]" + "接続を試行します";
        //                    times++;
        //                    if (this.window.connectionsetting.ConnectionIPEndPoiint != null)
        //                    {
        //                        System.Net.IPEndPoint ipendpoint = this.window.connectionsetting.ConnectionIPEndPoiint;
        //                        this.window.Dispatcher.BeginInvoke(new Action(() =>
        //                        {
        //                            this.window.TextBlock_Process_IPaddress.Text = ipendpoint.Address.ToString();
        //                            this.window.TextBlock_Process_Port.Text = ipendpoint.Port.ToString();
        //                            this.window.TextBlock_Process_Time.Text = this.window.pushedtime;
        //                            this.window.TextBlock_Process_Times.Text = times.ToString();
        //                            this.window.TextBlock_Process_Text.Text = "接続試行中";
        //                            this.window.TextBlock_StateTip.Text = "接続していません。接続を試行しています。";
        //                        }));

        //                        this.tcpclient = new System.Net.Sockets.TcpClient();
        //                        this.tcpclient.Connect(ipendpoint);
        //                        this.ns = this.tcpclient.GetStream();
        //                        this.debuglog.DebugLogPrint = "[" + this.ToString() + "]" + "接続に成功しました";
        //                        this.window.Dispatcher.BeginInvoke(new Action(() =>
        //                        {
        //                            this.window.TextBlock_Process_Text.Text = "接続完了";
        //                            this.window.TextBlock_StateTip.Text = "接続しています";
        //                        }));
        //                        this.Tcp_Send(new TerminalConnectionSettings.TerminalProtocols.DemmandInfo());
        //                        break;
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    for (int i = 0; i < 5; i++)
        //                    {
        //                        Thread.Sleep(new TimeSpan(0, 0, 1));
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                Thread.Sleep(new TimeSpan(0, 0, 1));
        //                this.window.Dispatcher.BeginInvoke(new Action(() =>
        //                {
        //                    this.window.TextBlock_Process_IPaddress.Text = "";
        //                    this.window.TextBlock_Process_Port.Text = "";
        //                    this.window.TextBlock_Process_Time.Text = "";
        //                    this.window.TextBlock_Process_Times.Text = "";
        //                    this.window.TextBlock_Process_Text.Text = "未接続";
        //                    this.window.TextBlock_StateTip.Text = "接続していません。";
        //                }));
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            this.debuglog.DebugLogPrint = "[" + this.ToString() + "]" + ex.Message;

        //        }
        //    }
        //}
        //public void Dispose()
        //{
        //    this.CTS.Cancel();
        //    while (!this.mytask.IsCompleted)
        //    {
        //        if (this.ns != null)
        //        {
        //            try
        //            {
        //                this.Tcp_Send(new TerminalConnectionSettings.TerminalProtocols.Close());
        //                this.tcpclient.Close();
        //            }
        //            catch (Exception ex) 
        //            {

        //            }
        //        }
        //        Thread.Sleep(new TimeSpan(0, 0, 1));
        //    }
        //}
    }
}
