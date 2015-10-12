using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CIPCTerminal
{
    public class CIPCServerConnection : IDisposable
    {
        public System.Net.Sockets.TcpClient tcpclient { set; get; }
        public System.Net.Sockets.NetworkStream ns { set; get; }

        public TCPConnectWindow window { set; get; }
        public myparts.DebugLog debuglog { set; get; }

        private CancellationTokenSource CTS { set; get; }
        private Task mytask { set; get; }

        public TerminalConnectionSettings.NetworkData receive { set; get; }
        public TerminalConnectionSettings.NetworkData send { set; get; }
        public TerminalConnectionSettings.CommandEventer Eventer { set; get; }


        public MainWindow mainwindow { set; get; }

        public CIPCServerConnection(TCPConnectWindow window)
        {
            this.Init_Classes(window);
        }

        private void Init_Classes(TCPConnectWindow window)
        {
            this.window = window;
            this.receive = new TerminalConnectionSettings.NetworkData(1024);
            this.send = new TerminalConnectionSettings.NetworkData(1024);
            this.Eventer = new TerminalConnectionSettings.CommandEventer();

            this.Init_Task();
        }

        private void Init_Task()
        {
            try
            {
                if (this.CTS != null)
                {
                    this.CTS.Cancel();
                    Thread.Sleep(100);
                    this.CTS = null;
                    this.mytask = null;
                    Thread.Sleep(100);
                }
                this.CTS = new CancellationTokenSource();
                this.mytask = new Task(maintask, this.CTS);
                this.mytask.Start();
            }
            catch (Exception ex)
            {
                this.debuglog.DebugLogPrint = "[" + this.ToString() +"]" + ex.Message;
            }
        }

        private void maintask(object obj)
        {
            while (!this.CTS.IsCancellationRequested)
            {
                try
                {

                    this.Setup_in_Task();
                    while (!this.CTS.IsCancellationRequested)
                    {
                        this.Update_in_Task();
                    }
                }
                catch (Exception ex)
                {
                    this.debuglog.DebugLogPrint = "[" + this.ToString() +"]" + ex.Message;
                    this.mainwindow.DeleteContents();
                }
            }
        }

        private void Update_in_Task()
        {
            if (this.tcpclient != null && this.ns != null)
            {
                this.Tcp_Receive();
                this.Eventer.Handle(receive.stringData);
            }
        }

        public void Tcp_Send(TerminalConnectionSettings.TerminalProtocols.CIPCTerminalCommand terminalcommand)
        {
            try
            {
                this.send.stringData = terminalcommand.Data;
                this.ns.Write(this.send.encodedbytes, 0, this.send.encodedbytes.Length);
                this.debuglog.DebugLogPrint = "[" + this.ToString() + "]送信:" + this.send.stringData;
            }
            catch (Exception ex)
            {
                this.debuglog.DebugLogPrint = this.debuglog.DebugLogPrint = "[" + this.ToString() + "]" + ex.Message;
            }
        }

        private void Tcp_Receive()
        {
            this.receive.DataIndex = 0;
            while (this.receive.string_set(this.ns.Read(this.receive.buffer, 0, this.receive.buffer.Length)) == this.receive.buffer.Length)
            {

            }
            this.debuglog.DebugLogPrint = "[" + this.ToString() + "]受信:" + this.receive.stringData;
        }

        private void Setup_in_Task()
        {
            this.Init_tcpclient();
            
        }

        private void Init_tcpclient()
        {
            int times = 0;
            while (!this.CTS.IsCancellationRequested)
            {
                try
                {
                    if (this.window.IsConnectSetuped == true)
                    {
                        try
                        {
                            this.debuglog.DebugLogPrint = "[" + this.ToString() +"]" + "接続を試行します";
                            times++;
                            if (this.window.connectionsetting.ConnectionIPEndPoiint != null)
                            {
                                System.Net.IPEndPoint ipendpoint = this.window.connectionsetting.ConnectionIPEndPoiint;
                                this.window.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    this.window.TextBlock_Process_IPaddress.Text = ipendpoint.Address.ToString();
                                    this.window.TextBlock_Process_Port.Text = ipendpoint.Port.ToString();
                                    this.window.TextBlock_Process_Time.Text = this.window.pushedtime;
                                    this.window.TextBlock_Process_Times.Text = times.ToString();
                                    this.window.TextBlock_Process_Text.Text = "接続試行中";
                                    this.window.TextBlock_StateTip.Text = "接続していません。接続を試行しています。";
                                }));

                                this.tcpclient = new System.Net.Sockets.TcpClient();
                                this.tcpclient.Connect(ipendpoint);
                                this.ns = this.tcpclient.GetStream();
                                this.debuglog.DebugLogPrint = "[" + this.ToString() + "]" + "接続に成功しました";
                                this.window.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    this.window.TextBlock_Process_Text.Text = "接続完了";
                                    this.window.TextBlock_StateTip.Text = "接続しています";
                                }));
                                this.Tcp_Send(new TerminalConnectionSettings.TerminalProtocols.DemmandInfo());
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                Thread.Sleep(new TimeSpan(0, 0, 1));
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(new TimeSpan(0, 0, 1));
                        this.window.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            this.window.TextBlock_Process_IPaddress.Text = "";
                            this.window.TextBlock_Process_Port.Text = "";
                            this.window.TextBlock_Process_Time.Text = "";
                            this.window.TextBlock_Process_Times.Text = "";
                            this.window.TextBlock_Process_Text.Text = "未接続";
                            this.window.TextBlock_StateTip.Text = "接続していません。";
                        }));
                    }
                }
                catch (Exception ex)
                {
                    this.debuglog.DebugLogPrint = "[" + this.ToString() + "]" + ex.Message;
                    
                }
            }
        }
        public void Dispose()
        {
            this.CTS.Cancel();
            while (!this.mytask.IsCompleted)
            {
                if (this.ns != null)
                {
                    try
                    {
                        this.Tcp_Send(new TerminalConnectionSettings.TerminalProtocols.Close());
                        this.tcpclient.Close();
                    }
                    catch (Exception ex) 
                    {
 
                    }
                }
                Thread.Sleep(new TimeSpan(0, 0, 1));
            }
        }
    }
}
