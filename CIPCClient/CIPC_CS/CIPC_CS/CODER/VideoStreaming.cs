using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPC_CS.CODER
{
    public class VideoStreaming
    {
        private CIPC_CS.CLIENT.CLIENT client;
        public int DPP { set; get; }
        public long Datalength { set; get; }
        public byte[] ReceiveData;

        public delegate void DataReceivedEventHandler(object sender, byte[] e);
        public event DataReceivedEventHandler DataReceived;
        protected virtual void OnDataReceived(byte[] e)
        {
            if (this.DataReceived != null) { this.DataReceived(this, e); }
        }

        public VideoStreaming(string name, int MyPort, string ServerIP, int ServerPort, int fps, long datalength, int DPP = 1024)
        {
            this.client = new CLIENT.CLIENT(MyPort, ServerIP, ServerPort, name + "_strm", fps);
            this.DPP = DPP;
            this.Datalength = datalength;


            this.ReceiveData = new byte[datalength];
        }
        public void Setup()
        {
            this.client.Setup(CLIENT.MODE.Both);
            this.client.DataReceived += client_DataReceived;
        }

        void client_DataReceived(object sender, byte[] e)
        {
            UDP_PACKETS_CODER.UDP_PACKETS_DECODER dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
            dec.Source = e;
            int id = dec.get_int();
            if (id < this.Datalength / this.DPP - 1)
            {
                byte[] data = dec.get_bytes(this.DPP);
                Array.Copy(data, 0, this.ReceiveData, id * this.DPP, data.Length);
            }
            else
            {
                byte[] data = dec.get_bytes((int)this.Datalength % this.DPP);
                Array.Copy(data, 0, this.ReceiveData, id * this.DPP, data.Length);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void Send(byte[] data)
        {
            if (this.Datalength != data.LongLength) return;

            byte[] buffer = new byte[this.DPP];

            byte[] senddata;

            for (int i = 0; i < data.LongLength / this.DPP; i++)
            {
                try
                {
                    if (i < data.LongLength / this.DPP - 1)
                    {
                        Array.Copy(data, i * this.DPP, buffer, 0, this.DPP);
                    }
                    else
                    {
                        if (this.Datalength % this.DPP == 0) return;
                        Array.Copy(data, i * this.DPP, buffer, 0, this.Datalength % this.DPP);
                    }
                    UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
                    enc += i;
                    enc += buffer;
                    senddata = enc.data;
                    this.client.Update(ref senddata);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + this.DPP);
                }
            }
        }
    }
}
