using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace UDP_PACKETS_CLIANT
{
    public class UDP_PACKETS_RECIEVER
    {
        #region private field
        private IPEndPoint remoteEP;
        private UdpClient udpcliant;
        #endregion

        #region propaty
        /// <summary>
        /// 接続先のremoteEPを設定、および取得します。
        /// </summary>
        public IPEndPoint RemoteEP 
        {
            set
            {
                remoteEP = value;
            }
            get 
            {
                return remoteEP;
            }
        }
        #endregion

        #region constructer
        /// <summary>
        /// 受信用UDPクライアントを作成します。デフォルトではlocalPortに到着したanyIPのanyPortを監視します。監視先を指定する場合は、RemoteEP(end point : system.net)プロパティを設定してください。
        /// </summary>
        /// <param name="localPort">バインドするポート番号</param>
        public UDP_PACKETS_RECIEVER(int localPort)
        {
            try
            {
                this.remoteEP = new IPEndPoint(IPAddress.Any, 0);
                this.udpcliant = new UdpClient(localPort);
            }
            catch(Exception ex) 
            {
                throw ex;
            }
        }
        #endregion

        #region public method
        /// <summary>
        /// データを受信するまで待機し、取得したデータを返します。リモートホストを指定していた場合は繁栄され、デフォルトの場合はRemoteEPプロパティに取得先のエンドポイントが反映されます。
        /// </summary>
        /// <returns></returns>
        public byte[] Recieve() 
        {
            try 
            {
                return this.udpcliant.Receive(ref this.remoteEP);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region private method
        #endregion
    }
}
