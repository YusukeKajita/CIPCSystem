using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerminalConnectionSettings
{
    /// <summary>
    /// 受信データを文字列に変換するクラス．UTF-8を使用
    /// </summary>
    public class NetworkData
    {
        /// <summary>
        /// エンコードのタイプ
        /// </summary>
        public readonly System.Text.Encoding enc = System.Text.Encoding.UTF8;
        /// <summary>
        /// networkstreamから受信するバッファ
        /// </summary>
        public byte[] buffer { set; get; }

        public const int DataBufferMax = 1024 * 5;
        public int DataIndex { set; get; }

        /// <summary>
        /// 受信時のCount値を格納
        /// </summary>
        public int datalength { set; get; }

        private int buffermaxsize;
        private byte[] data { set; get; }
        /// <summary>
        /// セットされたデータを文字列として取得・設定します．
        /// </summary>
        public string stringData
        {
            set
            {
                this.data = enc.GetBytes(value);
                this.DataIndex = this.data.Length;
            }
            get
            {
                return enc.GetString(this.data, 0, this.DataIndex);
            }
        }
        /// <summary>
        /// バッファの最大値
        /// </summary>
        public int buffermax
        {
            get
            {
                return this.buffermaxsize;
            }
        }
        /// <summary>
        /// 送信用のデータ
        /// </summary>
        public byte[] encodedbytes
        {
            get
            {
                return data;
            }
        }

        /// <summary>
        /// ネットワークデータを初期化します
        /// </summary>
        /// <param name="buffermax"></param>
        public NetworkData(int buffermax)
        {
            this.buffermaxsize = buffermax;
            this.buffer = new byte[buffermax];
            this.data = new byte[DataBufferMax];
            this.DataIndex = 0;
        }

        /// <summary>
        /// 受信したバッファから文字列データを生成します．
        /// </summary>
        /// <param name="length"></param>
        public int string_set(int length)
        {
            for (int i = 0; i < length; i++, this.DataIndex++)
            {
                this.data[this.DataIndex] = this.buffer[i];
            }
            return length;
        }
    }
}
