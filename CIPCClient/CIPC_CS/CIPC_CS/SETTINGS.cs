using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPC_CS
{
    public class SETTINGS
    {
        /// <summary>
        /// リモートホストサーバ（ＣＩＰＳ）の設定
        /// </summary>
        public struct RemoteExchangeServer
        {
            public const string IP = "192.168.11.10";
            public const int Port = 12000;
        };

        /// <summary>
        /// 自情報の設定
        /// </summary>
        public struct MyInfo
        {
            public const int Port = 4000;
            public const string Name = "Host_Name";
            public const int fps = 60;
            public const CIPC_CS.CLIENT.MODE Mode = CLIENT.MODE.Sender;
        };

        /// <summary>
        /// 接続・切断時のプロトコル
        /// </summary>
        public struct ConnectionCommands
        {
            public struct GREETING
            {
                public const int DEMANDS = 1;
                public const int END = 9;
            };
            public struct MODE
            {
                public const int SEND = 2;
                public const int RECEIVE = 3;
                public const int BOTH = 4;
                public const int DIRECT = 15;

            };
            public struct REPLY
            {
                public const int OK = 0;
            };
        };
    }
}
