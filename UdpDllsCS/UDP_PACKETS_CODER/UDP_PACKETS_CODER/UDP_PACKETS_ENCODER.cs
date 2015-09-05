using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDP_PACKETS_CODER
{
    //演算子+=でデータを追加し、プロパティのゲットアクセサから出来上がったデータを取得します
    public class UDP_PACKETS_ENCODER
    {
        #region private field
        private List<byte> Ldata;
        #endregion

        #region propaty

        //出来上がったデータを取得します
        public byte[] data 
        {
            get 
            {
                byte[] a = new byte[this.Ldata.Count];
                for(int t = 0; t<this.Ldata.Count; t++){
                    a[t] = this.Ldata[t];
                }
                return a;
            }
        }

        #endregion

        #region constructer

        #region normal
        public UDP_PACKETS_ENCODER()
        {
            this.Ldata = new List<byte>();
        }

        public UDP_PACKETS_ENCODER(UDP_PACKETS_ENCODER udppm)
        {
            this.Ldata = udppm.Ldata;
        }
        #endregion


        #region additioner and converter
        public UDP_PACKETS_ENCODER(UDP_PACKETS_ENCODER udppm, byte[] add_data)
        {
            this.Ldata = udppm.Ldata;
            for (int t = 0; t < add_data.Length; t++)
            {
                this.Ldata.Add(add_data[t]);
            }
            
        }

        public UDP_PACKETS_ENCODER(UDP_PACKETS_ENCODER udppm, string str, Encoding encoding) 
        {
            this.Ldata = udppm.Ldata;
            byte[] add_data = encoding.GetBytes(str);
            byte[] _add_data = BitConverter.GetBytes(add_data.Length);
            ByteDataAdditioner(_add_data);
            ByteDataAdditioner(add_data);
        }

        public UDP_PACKETS_ENCODER(UDP_PACKETS_ENCODER udppm, int intdata)
        {
            this.Ldata = udppm.Ldata;
            byte[] add_data = BitConverter.GetBytes(intdata);
            ByteDataAdditioner(add_data);
        }
        public UDP_PACKETS_ENCODER(UDP_PACKETS_ENCODER udppm, float floatdata)
        {
            this.Ldata = udppm.Ldata;
            byte[] add_data = BitConverter.GetBytes(floatdata);
            ByteDataAdditioner(add_data);
        }
        public UDP_PACKETS_ENCODER(UDP_PACKETS_ENCODER udppm, long longdata)
        {
            this.Ldata = udppm.Ldata;
            byte[] add_data = BitConverter.GetBytes(longdata);
            ByteDataAdditioner(add_data);
        }
        public UDP_PACKETS_ENCODER(UDP_PACKETS_ENCODER udppm, double doubledata)
        {
            this.Ldata = udppm.Ldata;
            byte[] add_data = BitConverter.GetBytes(doubledata);
            ByteDataAdditioner(add_data);
        }
        public UDP_PACKETS_ENCODER(UDP_PACKETS_ENCODER udppm, bool booldata)
        {
            this.Ldata = udppm.Ldata;
            byte[] add_data = BitConverter.GetBytes(booldata);
            ByteDataAdditioner(add_data);
        }
        public UDP_PACKETS_ENCODER(UDP_PACKETS_ENCODER udppm, ulong ulongdata)
        {
            this.Ldata = udppm.Ldata;
            byte[] add_data = BitConverter.GetBytes(ulongdata);
            ByteDataAdditioner(add_data);
        }
        public UDP_PACKETS_ENCODER(UDP_PACKETS_ENCODER udppm, uint uintdata)
        {
            this.Ldata = udppm.Ldata;
            byte[] add_data = BitConverter.GetBytes(uintdata);
            ByteDataAdditioner(add_data);
        }
        public UDP_PACKETS_ENCODER(UDP_PACKETS_ENCODER udppm, sbyte sbytedata)
        {
            this.Ldata = udppm.Ldata;
            byte[] add_data = BitConverter.GetBytes(sbytedata);
            ByteDataAdditioner(add_data);
        }
        public UDP_PACKETS_ENCODER(UDP_PACKETS_ENCODER udppm, ushort ushortdata)
        {
            this.Ldata = udppm.Ldata;
            byte[] add_data = BitConverter.GetBytes(ushortdata);
            ByteDataAdditioner(add_data);
        }
        public UDP_PACKETS_ENCODER(UDP_PACKETS_ENCODER udppm, short shortdata)
        {
            this.Ldata = udppm.Ldata;
            byte[] add_data = BitConverter.GetBytes(shortdata);
            ByteDataAdditioner(add_data);
        }

        public UDP_PACKETS_ENCODER(UDP_PACKETS_ENCODER udppm, byte bytedata)
        {
            this.Ldata = udppm.Ldata;
            byte[] add_data = new byte[1];
            add_data[0] = bytedata;
            ByteDataAdditioner(add_data);
        }
        #endregion

        #endregion


        #region operator overrides
        public static UDP_PACKETS_ENCODER operator +(UDP_PACKETS_ENCODER upm, byte[] str)
        {
            return new UDP_PACKETS_ENCODER(upm, str);
        }
        public static UDP_PACKETS_ENCODER operator+ (UDP_PACKETS_ENCODER upm, string str)
        {
            return new UDP_PACKETS_ENCODER(upm, str, Encoding.Unicode);
        }
        public static UDP_PACKETS_ENCODER operator +(UDP_PACKETS_ENCODER upm, int data)
        {
            return new UDP_PACKETS_ENCODER(upm, data);
        }
        public static UDP_PACKETS_ENCODER operator +(UDP_PACKETS_ENCODER upm, bool data)
        {
            return new UDP_PACKETS_ENCODER(upm, data);
        }
        public static UDP_PACKETS_ENCODER operator +(UDP_PACKETS_ENCODER upm, ulong data)
        {
            return new UDP_PACKETS_ENCODER(upm, data);
        }
        public static UDP_PACKETS_ENCODER operator +(UDP_PACKETS_ENCODER upm, long data)
        {
            return new UDP_PACKETS_ENCODER(upm, data);
        }
        public static UDP_PACKETS_ENCODER operator +(UDP_PACKETS_ENCODER upm, uint data)
        {
            return new UDP_PACKETS_ENCODER(upm, data);
        }
        public static UDP_PACKETS_ENCODER operator +(UDP_PACKETS_ENCODER upm, sbyte data)
        {
            return new UDP_PACKETS_ENCODER(upm, data);
        }
        public static UDP_PACKETS_ENCODER operator +(UDP_PACKETS_ENCODER upm, float data)
        {
            return new UDP_PACKETS_ENCODER(upm, data);
        }
        public static UDP_PACKETS_ENCODER operator +(UDP_PACKETS_ENCODER upm, double data)
        {
            return new UDP_PACKETS_ENCODER(upm, data);
        }
        public static UDP_PACKETS_ENCODER operator +(UDP_PACKETS_ENCODER upm, ushort data)
        {
            return new UDP_PACKETS_ENCODER(upm, data);
        }
        public static UDP_PACKETS_ENCODER operator +(UDP_PACKETS_ENCODER upm, short data)
        {
            return new UDP_PACKETS_ENCODER(upm, data);
        }

        public static UDP_PACKETS_ENCODER operator +(UDP_PACKETS_ENCODER upm, byte data)
        {
            return new UDP_PACKETS_ENCODER(upm, data);
        }
        #endregion

        #region private method
        private void ByteDataAdditioner(byte[]  add_data)
        {
            for (int t = 0; t < add_data.Length; t++)
            {
                this.Ldata.Add(add_data[t]);
            }
        }
        #endregion
    }


    
}
