using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.ObjectModel;



namespace CIPC_CS_Unity.CLIENT
{
    public enum MODE
    {
        Sender,
        Receiver,
        non
    };

    public class BASE
    {
        #region Field
        protected MODE mode;
        protected UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT udp_client;
        protected UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc;
        protected UDP_PACKETS_CODER.UDP_PACKETS_DECODER dec;

        protected int myPort;
        protected string remoteIP;
        protected int serverPort;
        protected int remotePort;

        protected string name;
        protected int fps;
        public int IsAvailable
        {
            get
            {
                return this.udp_client.Available;
            }
        }

        public delegate void DataReceivedEventHandler(object sender, byte[] e);
        public event DataReceivedEventHandler DataReceived;
        protected virtual void OnDataReceived(byte[] e)
        {
            if (this.DataReceived != null) { this.DataReceived(this, e); }
        }
        #endregion

        #region constructer
        public BASE(int myPort, string remoteIP, int serverPort, string name, int fps) 
        {
            this.myPort = myPort;
            this.remoteIP = remoteIP;
            this.serverPort = serverPort;
            this.name = name;
            this.fps = fps;
        }
        #endregion

        #region method
        public void Setup(MODE mode)
        {
            this.udp_client = new UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT(this.remoteIP, this.serverPort, this.myPort);
            this.mode = mode;
            this.Connect();
        }

        
        public void Update(ref byte[] data) 
        {
            this._Update(ref data);
            this.Update_add(data);
        }

        
        #region public
        #endregion
        #region protected
        protected virtual void Send(ref byte[] data) 
        {
            try
            {
                udp_client.Send(data);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        protected virtual void Receive(ref byte[] data)
        {
            //Console.WriteLine("受信開始");
            if (this.udp_client != null)
            {
                data = this.udp_client.Recieve();
            }
            //Console.WriteLine("受信終了");
        }
        protected virtual void Connect_Sender_add() 
        {
            //Console.WriteLine("connect_sender_add_base");
        }
        protected virtual void Connect_Receiver_add()
        {
            //Console.WriteLine("connect_Receiver_add_base");
        }
        protected virtual void Connect_add()
        {
            //Console.WriteLine("connect_add_base");
        }
        protected virtual void Update_add(byte[] data)
        {
            //Console.WriteLine("update_add_base");
        }
        public void Close() 
        {
            this.udp_client.Close();
            this.udp_client = null;
            Thread.Sleep(100);
            this.Close_add();

        }

        protected virtual void Close_add()
        {
            Console.WriteLine("close_add_base");
        }

        #endregion
        #region private
        private void _Update(ref byte[] data)
        {
            switch (this.mode)
            {
                case MODE.non:
                    try
                    {
                        this.Connect();
                    }
                    catch (Exception ex) 
                    {
                        throw ex;
                    }
                    break;
                case MODE.Sender:
                    try
                    {
                        this.Send(ref data);
                    }
                    catch (Exception ex) 
                    {
                        throw ex;
                    }
                    break;
                case MODE.Receiver:
                    try
                    {
                        this.Receive(ref data);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    break;
                default:
                    break;
            }
        }
        private void Connect()
        {
            this.Connect_add();
            switch (this.mode)
            {
                case MODE.non:
                    this.mode = SETTINGS.MyInfo.Mode;
                    Connect();
                    break;
                case MODE.Sender:
                    this.udp_client = new UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT(this.remoteIP, this.remotePort, this.myPort);
                    Connect_Sender_add();
                    break;
                case MODE.Receiver:
                    this.udp_client = new UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT(this.remoteIP, this.remotePort, this.myPort);
                    Connect_Receiver_add();
                    break;
                default:
                    break;
            }
        }
        #endregion
        #endregion
    }
}
