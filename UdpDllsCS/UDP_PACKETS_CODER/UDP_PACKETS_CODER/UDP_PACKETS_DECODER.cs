using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace UDP_PACKETS_CODER
{
    /// <summary>
    /// 生のバイトデータを設定すると、内部に含まれているデータの種類を検索し、それぞれの型ごとに分割します
    /// </summary>
    public class UDP_PACKETS_DECODER
    {
        #region private field
        private byte[] lowdata;
        private int bitindex;
        #endregion

        #region propaty
        /// <summary>
        /// 変換元データを設定します。
        /// </summary>
        public byte[] Source 
        {
            set 
            {
                this.lowdata = value;
                bitindex = 0;
            }
        }

        /// <summary>
        /// バイト配列の長さを取得します
        /// </summary>
        public int Length
        {
            get
            {
                return lowdata.Length;
            }
        }

        public int RemainBytesLength
        {
            get
            {
                return this.lowdata.Length - this.bitindex;
            }
        }
        #endregion

        #region constructer
        public UDP_PACKETS_DECODER()
        {
            bitindex = 0;
        }
        public UDP_PACKETS_DECODER(byte[] recieveddata) 
        {
            this.lowdata = recieveddata;
            bitindex = 0;
        }
        #endregion

        #region public method
        public int get_int() 
        {
            try
            {
                this.ExceptionThrower();
                int data = BitConverter.ToInt32(lowdata, bitindex);
                bitindex += sizeof(int);
                return data;
            }
            catch {
                throw;
            } 
        }
        public double get_double()
        {
            try
            {
                this.ExceptionThrower();
                double data = BitConverter.ToDouble(lowdata, bitindex);
                bitindex += sizeof(double);
                return data;
            }
            catch
            {
                throw;
            }
        }
        public float get_float() 
        {
            try
            {
                this.ExceptionThrower();
                float data = BitConverter.ToSingle(lowdata, bitindex);
                bitindex += sizeof(float);
                return data;
            }
            catch
            {
                throw;
            }
        }
        public uint get_uint()
        {
            try
            {
                this.ExceptionThrower();
                uint data = BitConverter.ToUInt32(lowdata, bitindex);
                bitindex += sizeof(uint);
                return data;
            }
            catch
            {
                throw;
            }
        }
        public long get_long() 
        {
            try
            {
                this.ExceptionThrower();
                long data = BitConverter.ToInt64(lowdata, bitindex);
                bitindex += sizeof(long);
                return data;
            }
            catch
            {
                throw;
            }
        }
        public ulong get_ulong() 
        {
            try
            {
                this.ExceptionThrower();
                ulong data = BitConverter.ToUInt64(lowdata, bitindex);
                bitindex += sizeof(ulong);
                return data;
            }
            catch
            {
                throw;
            }
        }
        public bool get_bool() {
            try 
            {
                this.ExceptionThrower();
                bool data = BitConverter.ToBoolean(lowdata, bitindex);
                bitindex += sizeof(bool);
                return data;
            }
            catch
            {
                throw;
            }
        }
        public byte get_byte()
        {
            try
            {
                this.ExceptionThrower();
                byte data = Convert.ToByte(lowdata[bitindex]);
                bitindex += sizeof(byte);
                return data;
            }
            catch
            {
                throw;
            }
        }
        public byte[] get_bytes(int size) 
        {
            try
            {
                this.ExceptionThrower();
                byte[] data = new byte[size];
                for (int t = 0; t < size; t++)
                {
                    data[t] = lowdata[bitindex + t];
                }
                bitindex += size;
                return data;
            }
            catch
            {
                throw;
            }
        }
        public sbyte get_sbyte() 
        {
            try
            {
                this.ExceptionThrower();
                sbyte data = Convert.ToSByte( lowdata[bitindex]);
                bitindex += sizeof(sbyte);
                return data;
            }
            catch
            {
                throw;
            }
        }
        public ushort get_ushort()
        {
            try
            {
                this.ExceptionThrower();
                ushort data = Convert.ToUInt16(lowdata[bitindex]);
                bitindex += sizeof(ushort);
                return data;
            }
            catch
            {
                throw;
            }
        }
        public short get_short()
        {
            try
            {
                this.ExceptionThrower();
                short data = Convert.ToInt16(lowdata[bitindex]);
                bitindex += sizeof(short);
                return data;
            }
            catch
            {
                throw;
            }
        }
        public string get_string() 
        {
            try
            {
                this.ExceptionThrower();
                int length = BitConverter.ToInt32(lowdata, bitindex);
                bitindex += sizeof(int);

                string data = Encoding.Unicode.GetString(lowdata, bitindex,length);
                bitindex += length;
                return data;
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region private method
        
        private void ExceptionThrower()
        {
            if (bitindex >= this.lowdata.Length)
            {
                throw new Exception("overflow.");
            }
        }

        #endregion
    };
}
