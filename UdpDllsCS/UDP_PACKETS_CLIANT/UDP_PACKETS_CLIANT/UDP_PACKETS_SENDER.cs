using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace UDP_PACKETS_CLIANT
{
    public class UDP_PACKETS_SENDER
    {
        #region private field
        private byte[] byte_data;
        private IPEndPoint remotehost;
        private UdpClient udpcliant;
        private bool b_datasetted = false;
        private bool is_conected = false;
        #endregion

        #region propaty
        public byte[] Data 
        {
            set {
                this.byte_data = value;
                b_datasetted = true;
            }
        }
        public bool IsConected 
        {
            get {
                return is_conected;
            }
        }
        #endregion

        #region constructer
        /// <summary>
        /// 送信用UDPクライアントを作成します。remoteIPadressには相手先のIPアドレスを記述してください。例："127.0.0.1" //ループバックアドレス
        /// </summary>
        /// <param name="remoteIPaddress"></param>
        /// <param name="remotePort"></param>
        /// <param name="localPort"></param>
        public UDP_PACKETS_SENDER(string remoteIPaddress, int remotePort, int localPort)
        {
            try
            {
                IPAddress remoteIP;
                if(!IPAddress.TryParse(remoteIPaddress, out remoteIP))
                {
                    throw new Exception("Error from UDP_PACKETS_SENDER, remoteIP is not available.");
                }
                remotehost = new IPEndPoint(remoteIP, remotePort);
                udpcliant = new UdpClient(localPort);
                udpcliant.Connect(remotehost);
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
                    throw new Exception("error from UDP_PACKETS_SENDER, data is not setted. use theother method.");
                }
            }
            else 
            {
                throw new Exception("error from UDP_PACKETS_SENDER, the udpcliant is not connected.");
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
                throw new Exception("error from UDP_PACKETS_SENDER, the udpcliant is not connected.");
            }
        }

        /// <summary>
        /// 接続を明示的に終了します。再接続する際は新しくオブジェクトを作成してください。
        /// </summary>
        public void Close()
        {
            udpcliant.Close();
            is_conected = false;
        }
        #endregion

        #region private method
        #endregion
    }
}
