using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace UDP_PACKETS_CLIANT
{
    /// <summary>
    /// 送信と受信を両方することのできるクラスを提供します。
    /// </summary>
    public class UDP_PACKETS_CLIANT
    {
        #region private field
        private byte[] byte_data;
        private byte[] Received_data;
        private IPEndPoint remotehost;
        private UdpClient udpcliant;
        private bool b_datasetted = false;
        private bool is_conected = false;
        private bool get_Rdata = true;
        /// <summary>
        /// UDPクライアントが受信しているバイト数を取得します
        /// </summary>
        public int Available
        {
            get
            {
                return this.udpcliant.Available;
            }
        }

        public delegate void DataReceivedEventHandler(object sender, byte[] e);
        public event DataReceivedEventHandler DataReceived;
        protected virtual void OnDataReceived(byte[] e)
        {
            if (this.DataReceived != null) { this.DataReceived(this, e); }
        }

        #endregion

        #region propaty
        public byte[] Received_Data 
        {
            get 
            {
                get_Rdata = false;
                return this.Received_data;
            }
        }
        /// <summary>
        /// 送信するデータをセットします。
        /// </summary>
        public byte[] Data 
        {
            set {
                this.byte_data = value;
                b_datasetted = true;
            }
        }
        /// <summary>
        /// 接続されているかどうかを取得します。
        /// </summary>
        public bool IsConected 
        {
            get {
                return is_conected;
            }
        }
        /// <summary>
        /// リモートホストを設定、取得します。
        /// </summary>
        public IPEndPoint RemoteEP 
        {
            set 
            {
                RemoteEP = value;
            }
            get 
            {
                return this.remotehost;
            }
        }
        #endregion

        #region constructer

        public UDP_PACKETS_CLIANT(int localPort)
        {
            try
            {
                udpcliant = new UdpClient(localPort);
                this.Received_data = null;

                //udpcliant.BeginReceive(ReceiveCallback, udpcliant);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 送信用UDPクライアントを作成します。remoteIPadressには相手先のIPアドレスを記述してください。例："127.0.0.1" //ループバックアドレス
        /// </summary>
        /// <param name="remoteIPaddress"></param>
        /// <param name="remotePort"></param>
        /// <param name="localPort"></param>
        public UDP_PACKETS_CLIANT(string remoteIPaddress, int remotePort, int localPort)
        {
            try
            {
                IPAddress remoteIP;
                if(!IPAddress.TryParse(remoteIPaddress, out remoteIP))
                {
                    throw new Exception("Error from UDP_PACKETS_CLIANT, remoteIP is not available.");
                }
                remotehost = new IPEndPoint(remoteIP, remotePort);
                udpcliant = new UdpClient(localPort);
                udpcliant.Connect(remotehost);

                this.Received_data = null;

                //udpcliant.BeginReceive(ReceiveCallback, udpcliant);
                is_conected = true;
            }
            catch(Exception ex) 
            {
                throw ex;
            }
        }

        #endregion

        #region public method
        //データをあらかじめセットしてある場合はこちらのメソッドを利用してください。
        public void Send() 
        {
            if (is_conected)
            {
                if (b_datasetted)
                {
                    udpcliant.Send(this.byte_data, this.byte_data.Length);
                }
                else
                {
                    throw new Exception("error from UDP_PACKETS_CLIANT, data is not setted. use theother method.");
                }
            }
            else 
            {
                try 
                {
                    this.udpcliant.Connect(this.remotehost);
                    this.is_conected = true;
                    if (b_datasetted)
                    {
                        udpcliant.Send(this.byte_data, this.byte_data.Length);
                    }
                    else
                    {
                        throw new Exception("error from UDP_PACKETS_CLIANT, data is not setted. use theother method.");
                    }
                }
                catch(Exception ex)
                {
                    throw new Exception("error can't send. " + ex.Message);
                }
            }
        }

        /// <summary>
        /// UdpCliant class を利用してデータを送信します。
        /// </summary>
        /// <param name="data"></param>
        public void Send(byte[] data) 
        {
            if (is_conected)
            {
                udpcliant.Send(data, data.Length);
            }
            else 
            {
                try
                {
                    this.udpcliant.Connect(this.remotehost);
                    this.is_conected = true;
                    if (b_datasetted)
                    {
                        udpcliant.Send(data, data.Length);
                    }
                    else
                    {
                        throw new Exception("error from UDP_PACKETS_CLIANT, data is not setted. use theother method.");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("error can't send. " + ex.Message);
                }
            }
        }
        /// <summary>
        /// データを受信するまで待機し、取得したデータを返します。リモートホストを指定していた場合は繁栄され、デフォルトの場合はRemoteEPプロパティに取得先のエンドポイントが反映されます。
        /// </summary>
        /// <returns></returns>
        public byte[] Recieve() 
        {
            try 
            {
                this.is_conected = true;
                byte[] _data = this.udpcliant.Receive(ref this.remotehost);
                this.OnDataReceived(_data);
                this.udpcliant.Connect(this.remotehost);
                return _data;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


        //private void ReceiveCallback(IAsyncResult ar)
        //{
        //    try
        //    {
        //        this.Received_data = this.udpcliant.EndReceive(ar, ref this.remotehost);
        //        get_Rdata = true;
        //        this.is_conected = true;
        //        udpcliant.Connect(this.remotehost);
        //        this.udpcliant.BeginReceive(ReceiveCallback, udpcliant);
        //    }
        //    catch (Exception ex)
        //    {
        //        if (this.udpcliant != null) 
        //        {
        //            throw;
        //        }
        //    }
        //}

        /// <summary>
        /// 接続を明示的に終了します。再接続する際は新しくオブジェクトを作成してください。
        /// </summary>
        public void Close()
        {
            this.udpcliant.Close();
            this.udpcliant = null;
            is_conected = false;
        }
        #endregion

        #region private method
        #endregion
    }
}
