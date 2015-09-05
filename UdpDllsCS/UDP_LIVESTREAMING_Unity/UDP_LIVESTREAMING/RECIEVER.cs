using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDP_PACKETS_CODER;
using UDP_PACKETS_CLIANT;

namespace UDP_LIVESTREAMING
{
    public class RECIEVER
    {
        #region private field
        private UDP_PACKETS_ENCODER UPE;
        private UDP_PACKETS_DECODER UPD;

        private UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT CLIENT;

        private int recieveSize;
        private int nPackets;
        private int data_length;

        private int width;
        private int height;
        private int stride;

        private int senderPort;
        private int remotePort;
        private bool isconnected = false;
        #endregion

        #region propaty
        public int Height 
        {
            get 
            {
                return height;
            }
        }
        public int Width 
        {
            get 
            {
                return width;
            }
        }
        public int Stride 
        {
            get 
            {
                return stride;
            }
        }
        #endregion

        #region constructer
        public RECIEVER(int remotePort, int senderPort, int recieverPort) 
        {
            UPE = new UDP_PACKETS_ENCODER();
            CLIENT = new UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT(recieverPort);
            this.senderPort = senderPort;
            this.remotePort = remotePort;
        }
        #endregion

        #region public method
        public void Connect() 
        {
            byte[] data = CLIENT.Recieve();

            UPD = new UDP_PACKETS_DECODER(data);
            nPackets = UPD.get_int();
            recieveSize = UPD.get_int();
            data_length = UPD.get_int();
            width = UPD.get_int();
            height = UPD.get_int();
            stride = UPD.get_int();

            UPE=new UDP_PACKETS_ENCODER();
            UPE+=(int)0;
            CLIENT.Send(UPE.data);
            isconnected = true;
        }


        public byte[] Recieve(int id) 
        {
            if (isconnected == true)
            {
                int counter;
                byte[] data= new byte[data_length];
                for (int i = 0; i < nPackets; i++)
                {
                    byte[] _data = CLIENT.Recieve();
                    UPD = null;
                    UPD = new UDP_PACKETS_DECODER(_data);
                    if(UPD.get_int() == id)
                    {
                        counter = UPD.get_int();
                        Array.Copy(UPD.get_bytes(_data.Length - 2 * sizeof(int)), 0, data, counter * recieveSize, _data.Length - 2 * sizeof(int));
                    }
                    _data = null;
                }
                return data;
            }
            else 
            {
                throw new Exception("error from UDP_STREAMING, connect before to recieve.");
            }
        }

        public bool[] Recieve_bool(int id)
        {
            if (isconnected == true)
            {
                int counter;
                bool[] data = new bool[data_length];
                for (int i = 0; i < nPackets; i++)
                {
                    byte[] _data = CLIENT.Recieve();
                    UPD = null;
                    UPD = new UDP_PACKETS_DECODER(_data);
                    if (UPD.get_int() == id)
                    {
                        counter = UPD.get_int();
                        for (int t = 0; t < _data.Length-2*sizeof(int); t++ )
                        {
                            data[t+recieveSize*counter] = UPD.get_bool();
                        }
                    }
                    _data = null;
                }
                return data;
            }
            else
            {
                throw new Exception("error from UDP_STREAMING, connect before to recieve.");
            }
        }
        #endregion

        #region private method
        
        #endregion
    }
}
