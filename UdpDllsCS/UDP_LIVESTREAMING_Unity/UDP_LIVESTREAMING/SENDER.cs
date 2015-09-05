using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDP_PACKETS_CODER;
using UDP_PACKETS_CLIANT;

namespace UDP_LIVESTREAMING
{
    public class SENDER
    {
        #region private field
        private UDP_PACKETS_ENCODER UPE;

        private UDP_PACKETS_SENDER UPS;
        private UDP_PACKETS_RECIEVER UPR;

        private readonly int SendSize = 61440;
        private bool isconnected = false;
        #endregion

        #region propaty
        
        #endregion


        #region constructer
        /// <summary>
        /// 
        /// </summary>
        /// <param name="remoteIPaddress"></param>
        /// <param name="remotePort"></param>
        /// <param name="senderPort"></param>
        /// <param name="recieverPort"></param>
        public SENDER(string remoteIPaddress, int remotePort, int senderPort, int recieverPort)
        {
            UPE = new UDP_PACKETS_ENCODER();

            UPS = new UDP_PACKETS_SENDER(remoteIPaddress,remotePort,senderPort);
            UPR = new UDP_PACKETS_RECIEVER(recieverPort);
        }
        #endregion

        #region public method
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Connect(byte[] data,int width, int height, int stride) 
        {
            //sendblock
            UPE = new UDP_PACKETS_ENCODER();
            UPE += (int)(data.Length / SendSize + 1);//１フレームに送信するパケット数　キネクトの深度データで大凡10パケット
            UPE += SendSize;//分割したサイズ
            UPE += data.Length;
            UPE += width;
            UPE += height;
            UPE += stride;
            UPS.Send(UPE.data);

            isconnected = true;
            return true;
        }
        public bool Connect(bool[] data, int width, int height, int stride)
        {
            //sendblock
            UPE = new UDP_PACKETS_ENCODER();
            UPE += (int)(data.Length / SendSize + 1);//１フレームに送信するパケット数　キネクトの深度データで大凡10パケット
            UPE += SendSize;//分割したサイズ
            UPE += data.Length;
            UPE += width;
            UPE += height;
            UPE += stride;
            UPS.Send(UPE.data);

            isconnected = true;
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="id"></param>
        public void Send (byte[] data,int id)
        {
            if (isconnected == true)
            {
                int i = 0;
                for (i = 0; i < (int)(data.Length / SendSize); i++)
                {
                    byte[] _data = new byte[SendSize];
                    Array.Copy(data, i * SendSize, _data, 0, SendSize);
                    //sendblock
                    UPE = new UDP_PACKETS_ENCODER();
                    UPE += id;
                    UPE += i;
                    UPE += _data;
                    UPS.Send(UPE.data);
                }

                byte[] __data = new byte[data.Length % SendSize];
                Array.Copy(data, ((int)(data.Length / SendSize)-1) * SendSize, __data, 0, data.Length % SendSize);
                //sendblock
                UPE = new UDP_PACKETS_ENCODER();
                UPE += id;
                UPE += i;
                UPE += __data;
                UPS.Send(UPE.data);

            }
            else 
            {
                throw new Exception("error from UDP_STREAMING, connect before to send.");
            }
        }

        public void Send(bool[] data, int id)
        {
            if (isconnected == true)
            {
                int i = 0;
                for (i = 0; i < (int)(data.Length / SendSize); i++)
                {
                    bool[] _data = new bool[SendSize];
                    Array.Copy(data, i * SendSize, _data, 0, SendSize);
                    //sendblock
                    UPE = new UDP_PACKETS_ENCODER();
                    UPE += id;
                    UPE += i;
                    for (int t = 0; t < _data.Length; t++ )
                    {
                        UPE += _data[t];
                    }
                    UPS.Send(UPE.data);
                }

                bool[] __data = new bool[data.Length % SendSize];
                Array.Copy(data, ((int)(data.Length / SendSize) - 1) * SendSize, __data, 0, data.Length % SendSize);
                //sendblock
                UPE = new UDP_PACKETS_ENCODER();
                UPE += id;
                UPE += i;
                for (int t = 0; t < __data.Length; t++)
                {
                    UPE += __data[t];
                }
                UPS.Send(UPE.data);

            }
            else
            {
                throw new Exception("error from UDP_STREAMING, connect before to send.");
            }
        }
        #endregion

        #region private method
        #endregion
    }
}
