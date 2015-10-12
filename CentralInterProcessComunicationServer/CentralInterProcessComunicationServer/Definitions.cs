using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralInterProcessCommunicationServer
{
    /// <summary>
    /// サーバー設定　および　接続時のプロトコル
    /// </summary>
    public class Definitions
    {
        #region CONNECTION_COMANNDS
        /// <summary>
        /// 接続要求
        /// </summary>
        public const int CONNECTION_DEMANDS = 1;

        /// <summary>
        /// 接続終了通知
        /// </summary>
        public const int CONNECTION_END = 9;

        /// <summary>
        /// リモートホストのモード　送信
        /// </summary>
        public const int CONNECTION_MODE_SEND = 2;

        /// <summary>
        /// リモートホストのモード　受信
        /// </summary>
        public const int CONNECTION_MODE_RECEIVE = 3;

        /// <summary>
        /// 返信　ok
        /// </summary>
        public const int CONNECTION_OK = 0;

        /// <summary>
        /// サーバー操作用コマンド
        /// </summary>
        public const int CONNECTION_SERVER_OPERATE = 20;
        #endregion

        #region Server
        /// <summary>
        /// RemoteHostにポートを分配するサーバのポート　まず初めにここにコネクトする
        /// </summary>
        public const int REMOTEHOSTSERVER_PORT = 12000;

        /// <summary>
        /// ターミナルと接続する用のポート番号
        /// </summary>
        public const int TERMINALCONNECTION_PORT = 12500;

        /// <summary>
        /// RemoteHostが使用するポートの最初の番号　これおよび以降のポートを使用する。
        /// </summary>
        public const int PORT_START_NUMBER = 2000;
        #endregion Server
    }
}
