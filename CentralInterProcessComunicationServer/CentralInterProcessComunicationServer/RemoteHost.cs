using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Threading;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

using FPSAdjuster;

namespace CentralInterProcessCommunicationServer
{
    /// <summary>
    /// 接続先ホスト
    /// </summary>
    public class RemoteHost
    {
        private UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT UP_Cliant;
        private UDP_PACKETS_CODER.UDP_PACKETS_ENCODER UP_Encoder;
        private UDP_PACKETS_CODER.UDP_PACKETS_DECODER UP_Decoder;

        private string name = "default";
        private int id = 0;
        private ConnectionState cstate = ConnectionState.nonstate;
        private int FPS = 0;
        private bool isConnected = false;

        /// <summary>
        /// データを接続する宛先。相手のconnect_hostは自分のアドレス
        /// </summary>
        private RemoteHost connect_host = null;

        /// <summary>
        /// 保持しているデータおよび受信したデータの参照　実体を入れないように。
        /// </summary>
        private byte[] data = null;

        private Task mytask;
        private CancellationTokenSource CTS;


        private FPSAdjuster.FPSAdjuster fpsa;

        public TerminalConnection.TerminalConnection TC { set; get; }

        static Object SyncObject = new object();

        #region Live Streaming
        private int Division_num;
        #endregion
        public delegate void DataReceivedEventHandler(object sender, byte[] e);
        public event DataReceivedEventHandler DataReceived;
        protected virtual void OnDataReceived(byte[] e)
        {
            if (this.DataReceived != null) { this.DataReceived(this, e); }
        }
        public bool IsSyncSend { set; get; }


        #region propaty
        /// <summary>
        /// リモートホストとつながっているかどうかを取得します。
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return this.isConnected;
            }
        }

        /// <summary>
        /// リモートホストの名前を取得します。
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// データ接続している他ホストの名前を取得します。
        /// </summary>
        public string Connect_Name
        {
            get
            {
                if (this.connect_host == null)
                {
                    return "null";
                }
                else
                {
                    return this.connect_host.Name;
                }
            }
        }

        /// <summary>
        ///接続先のＩＰアドレスを取得します。 
        /// </summary>
        public string remoteIP
        {
            get
            {
                if (this.UP_Cliant != null)
                {
                    if (this.UP_Cliant.RemoteEP != null)
                    {
                        return this.UP_Cliant.RemoteEP.Address.ToString();
                    }
                }
                return "null";
            }
        }

        /// <summary>
        ///接続先のポートを取得します。 
        /// </summary>
        public string remotePort
        {
            get
            {
                if (this.UP_Cliant != null)
                {
                    if (this.UP_Cliant.RemoteEP != null)
                    {
                        return this.UP_Cliant.RemoteEP.Port.ToString();
                    }
                }
                return "null";
            }
        }

        /// <summary>
        /// id(使用しているポート番号)を取得します。
        /// </summary>
        public int ID
        {
            get
            {
                return this.id;
            }
        }

        /// <summary>
        /// Fpsを設定、取得します。
        /// </summary>
        public int Fps
        {
            set
            {
                this.FPS = value;
            }
            get
            {
                return this.FPS;
            }
        }

        /// <summary>
        /// 送受信するデータを取得、設定します。
        /// </summary>
        public byte[] Data
        {
            set
            {
                this.data = value;
            }
            get
            {
                return this.data;
            }
        }

        /// <summary>
        ///現在のモードを取得します。 
        /// </summary>
        public ConnectionState Connection_Mode
        {
            get
            {
                return this.cstate;
            }
        }

        /// <summary>
        /// 接続するリモートホストを設定します。
        /// </summary>
        public RemoteHost ConnectionHost
        {
            set
            {
                this.connect_host = value;
            }
        }

        public DebugWindow debugwindow { set; get; }
        #endregion


        #region constructer
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="localPort">自分のポート</param>
        public RemoteHost(int localPort, DebugWindow debugwindow, TerminalConnection.TerminalConnection TC)
        {
            try
            {
                this.UP_Cliant = new UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT(localPort);
                this.UP_Encoder = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
                this.UP_Decoder = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
                this.id = localPort;

                this.CTS = new CancellationTokenSource();
                this.mytask = new Task(() => this.Main_Task(), CTS.Token);

                this.mytask.Start();
                this.fpsa = new FPSAdjuster.FPSAdjuster();

                this.debugwindow = debugwindow;
                this.debugwindow.DebugLog = "[Port:" + localPort.ToString() + "]受信を開始します";

                this.TC = TC;
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
        #endregion constructer

        #region maintask
        /// <summary>
        /// メインタスク。Whileの中にループを、その前に接続確認などの初期化処理を書く。
        /// 送信モードの場合、リモートホストを設定した場合、そこにデータを渡す。
        /// 受信モードの場合、データの中身があればそこにデータを送信する。
        /// </summary>
        private void Main_Task()
        {
            try
            {
                this.Connect();
                this.debugwindow.DebugLog = "[Port:" + this.ID.ToString() + "]接続に成功しました．";
                this.debugwindow.DebugLog = "[Port:" + this.ID.ToString() + "]RemoteIP:" + this.remoteIP;
                this.debugwindow.DebugLog = "[Port:" + this.ID.ToString() + "]RemotePort:" + this.remotePort.ToString();
                this.debugwindow.DebugLog = "[Port:" + this.ID.ToString() + "]Name:" + this.Name;
                this.debugwindow.DebugLog = "[Port:" + this.ID.ToString() + "]FPS:" + this.FPS;
                
                try
                {
                    this.TC.Tcp_Send();
                }
                catch (Exception ex)
                {
                    debugwindow.DebugLog = "[" + this.ToString() + "]" + ex.Message;
                }
                while (!CTS.IsCancellationRequested)
                {
                    try
                    {
                        if (this.cstate == ConnectionState.Receiver)
                        {
                            if (this.IsSyncSend == false)
                            {
                                if (this.connect_host != null)
                                {
                                    try
                                    {
                                        this.data = this.connect_host.data;
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message);
                                    }
                                }
                                if (this.connect_host != null)
                                {
                                    this.Send();
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        this.FPSAdjuster();
                    }
                }
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
                Thread.Sleep(100);
                this.CTS = new CancellationTokenSource();
                this.mytask = new Task(() => this.Main_Task(), CTS.Token);

                this.mytask.Start();

            }
        }

        /// <summary>
        /// ＦＰＳ調整用コード　送受信の際に利用する
        /// 現在非計測形式
        /// </summary>
        private void FPSAdjuster()
        {
            try
            {
                //受信スレッドのＦＰＳ調整用コード
                if (this.FPS > 0)
                {
                    this.fpsa.Adjust();
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        #endregion maintask


        #region private method

        /// <summary>
        /// リモートホストと接続します。
        /// </summary>
        /// <returns>接続先のＩＰエンドポイント</returns>
        private IPEndPoint Connect()
        {
            try
            {
                while (true)
                {
                    if (this.UP_Cliant.Received_Data != null)
                    {
                        this.UP_Decoder.Source = this.UP_Cliant.Received_Data;
                        break;
                    }
                }
                if (this.UP_Decoder.get_int() == Definitions.CONNECTION_DEMANDS)
                {

                    this.FPS = this.UP_Decoder.get_int();
                    this.fpsa.Fps = this.FPS;
                    this.fpsa.Start();

                    switch (this.UP_Decoder.get_int())
                    {
                        case Definitions.CONNECTION_MODE_SEND:
                            this.cstate = ConnectionState.Sender;
                            this.UP_Cliant.DataReceived += UP_Cliant_DataReceived;
                            break;
                        case Definitions.CONNECTION_MODE_RECEIVE:
                            this.cstate = ConnectionState.Receiver;
                            break;
                        default:
                            throw new Exception("CONNECTION_MODE が無効です。senderの設定を確かめてください。");
                    }
                    name = this.UP_Decoder.get_string();

                    Thread.Sleep(100);

                    this.UP_Encoder = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
                    this.UP_Encoder += Definitions.CONNECTION_OK;
                    this.UP_Cliant.Send(this.UP_Encoder.data);

                    this.isConnected = true;

                }
                else
                {
                    throw new Exception("コネクションの際に例外が発生しました。");
                }
                return UP_Cliant.RemoteEP;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.UP_Cliant.DataReset();
            }
            return UP_Cliant.RemoteEP;
        }

        void UP_Cliant_DataReceived(object sender, byte[] e)
        {
            this.OnDataReceived(e);
            this.data = e;
        }


        /// <summary>
        /// 接続を切断します。
        /// </summary>
        public void disconnect()
        {
            try
            {
                this.debugwindow.DebugLog = "[Port:" + this.ID.ToString() + "]切断します．";
                UP_Cliant.Close();
                this.data = null;
                if (this.connect_host != null)
                {
                    this.connect_host.connect_host = null;
                    this.connect_host = null;
                }

                CTS.Cancel();
                isConnected = false;
                this.UP_Encoder = null;
                this.UP_Decoder = null;

                this.UP_Cliant = null;

                this.DataReceived = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "接続の切断に失敗しました。");
            }
            finally
            {
            }
        }

        /// <summary>
        /// 設定されている宛先に、持っているデータ、受け取ったデータをぶん投げます。
        /// </summary>
        private void Send()
        {
            try
            {
                if (this.data != null)
                {
                    UP_Cliant.Send(this.data);
                }
            }
            catch (Exception ex)
            {
                this.debugwindow.DebugLog = "[Port:" + this.ID.ToString() + "]データを送信できません" + ex.Message;
            }
        }

        public void SendSync(byte[] data)
        {
            try
            {
                if (data != null)
                {
                    UP_Cliant.Send(data);
                }
            }
            catch (Exception ex)
            {
                this.debugwindow.DebugLog = "[Port:" + this.ID.ToString() + "]データを送信できません" + ex.Message;
            }
        }

        /// <summary>
        /// データをリモートホストから受信します。
        /// Exceptionをコメントアウトすることでレシーブのエクセプションをはじきます。
        /// 現在不使用
        /// </summary>
        private void Receive()
        {
            try
            {
                if (this.UP_Cliant != null)
                {
                    if (this.UP_Cliant.Received_Data != null)
                    {
                        lock (SyncObject)
                        {
                            this.data = this.UP_Cliant.Received_Data;
                        }
                        this.OnDataReceived(this.data);
                    }
                }
            }
            catch (Exception ex)
            {
                //this.debugwindow.DebugLog = "[Port:" + this.ID.ToString() + "]データを受信していません。"+ ex.Message;
            }
        }

        #region live streaming
        /// <summary>
        /// ライブストリーミング受信
        /// </summary>
        private void LiveReceive()
        {
            try
            {
                if (this.UP_Cliant != null)
                {
                    if (this.UP_Cliant.Received_Data != null)
                    {
                        this.data = this.UP_Cliant.Received_Data;
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("データを受信していません。"+ ex.Message);
            }
        }

        /// <summary>
        /// ライブストリーミング送信
        /// </summary>
        private void LiveSend()
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #endregion private method

    }

    /// <summary>
    /// コネクションの形式を設定する列挙型
    /// </summary>
    public enum ConnectionState
    {
        Sender,
        Receiver,
        LiveSender,
        LiveReceiver,
        nonstate
    }
}
