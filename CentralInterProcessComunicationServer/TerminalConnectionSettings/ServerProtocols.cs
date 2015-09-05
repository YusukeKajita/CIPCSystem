using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalConnectionSettings.ServerProtocols
{
    public class CIPCServerCommand : Command
    {

        public CIPCServerCommand()
        {
            base.sendertype = Sender.CIPCSever;
            base.data = sendertype.ToString() + "\\";
        }

        protected void Addterminalaction_to_Data()
        {
            base.data += base.servercommand.ToString() + "\\";
        }
    }

    /// <summary>
    /// 二つのポートを指定しその接続設定を追加する要求
    /// </summary>
    public class ReportInfo : CIPCServerCommand
    {
        public class CIPCInfo
        {
            #region innerclass
            /// <summary>
            /// CIPCクライアントを叙述するクラス
            /// </summary>
            public class Client
            {
                /// <summary>
                /// 自分のポート
                /// </summary>
                public int MyPort { set; get; }
                /// <summary>
                /// 接続先のクライアント名
                /// </summary>
                public string name { set; get; }
                /// <summary>
                /// 接続先のポート番号
                /// </summary>
                public int remotePort { set; get; }
                /// <summary>
                /// 接続先のIPアドレス
                /// </summary>
                public string remoteIP { set; get; }
                /// <summary>
                /// 接続先に指定されたFPS
                /// </summary>
                public int FPS { set; get; }

                /// <summary>
                /// モード データを送信するクライアントの場合Sender，受信するクライアントの場合Receiver
                /// </summary>
                public enum Mode
                {
                    Sender,
                    Receiver,
                    Default
                };

                public Mode mode { set; get; }

                /// <summary>
                /// クライアントの情報
                /// </summary>
                /// <param name="MyPort">自分のポート</param>
                /// <param name="name">接続先のクライアント名</param>
                /// <param name="remotePort">接続先のポート番号</param>
                /// <param name="remoteIP">接続先のIPアドレス</param>
                /// <param name="FPS">接続先に指定されたFPS</param>
                public Client(int MyPort, string name, int remotePort, string remoteIP, int FPS, Mode mode)
                {
                    this.MyPort = MyPort;
                    this.name = name;
                    this.remotePort = remotePort;
                    this.remoteIP = remoteIP;
                    this.FPS = FPS;
                    this.mode = mode;
                }
                /// <summary>
                /// クライアント情報を￥で区切った文字情報としてリターンする
                /// </summary>
                /// <returns></returns>
                public override string ToString()
                {
                    return this.MyPort.ToString() + "\\"
                        + this.name + "\\"
                        + this.remotePort.ToString() + "\\"
                        + this.remoteIP + "\\"
                        + this.FPS.ToString() + "\\"
                        + this.mode.ToString() +"\\";
                }
            }

            public class Connection
            {
                /// <summary>
                /// データ送信側の接続先ポート
                /// </summary>
                public int senderport { set; get; }
                /// <summary>
                /// データ受信側の接続先ポート
                /// </summary>
                public int receiverport { set; get; }
                /// <summary>
                /// 接続情報の保持するクラス
                /// </summary>
                /// <param name="senderport">データ送信側の接続先ポート</param>
                /// <param name="receiverport">データ受信側の接続先ポート</param>
                public Connection(int senderport, int receiverport )
                {
                    this.senderport = senderport;
                    this.receiverport = receiverport;
                }
                /// <summary>
                /// 接続情報を文字情報に変換　￥によって区切られる
                /// </summary>
                /// <returns></returns>
                public override string ToString()
                {
                    return this.senderport.ToString() + "\\"
                        + this.receiverport.ToString() +"\\";
                }
            }
            #endregion

            /// <summary>
            /// Clientのリスト
            /// </summary>
            public List<Client> ClientList { set; get; }

            /// <summary>
            /// Connectionのリスト
            /// </summary>
            public List<Connection> ConnectionList { set; get; }

            public bool IsSyncConnect { set; get; }

            /// <summary>
            /// CIPCの情報を保持するクラス　各クライアントの情報と各接続の情報を保持する
            /// </summary>
            /// <param name="ClientList">クライアントのリスト</param>
            /// <param name="ConnectionList">コネクションのリスト</param>
            public CIPCInfo(List<Client> ClientList, List<Connection> ConnectionList,bool IsSyncConnect)
            {
                this.ClientList = ClientList;
                this.ConnectionList = ConnectionList;
                this.IsSyncConnect = IsSyncConnect;
            }

            /// <summary>
            /// クライアントとそのコネクションについて文字列化する
            /// すべての情報が￥で区切られた文字列になる．その際，各リストの先頭に
            /// その個数が書かれる
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                string str;
                str = this.ClientList.Count.ToString() + "\\";
                if (this.ClientList.Count != 0)
                {
                    foreach (var collection in ClientList)
                    {
                        str += collection.ToString();
                    }
                }
                str += this.ConnectionList.Count.ToString() + "\\";
                if (this.ConnectionList.Count != 0)
                {
                    foreach (var collection in ConnectionList)
                    {
                        str += collection.ToString();
                    }
                }
                str += IsSyncConnect.ToString() + "\\";
                return str;
            }
        }
        public CIPCInfo cipcinfo { set; get; }

        public ReportInfo(CIPCInfo cipcinfo)
        {
            base.servercommand = ServerCommand.ReportInfo;
            this.cipcinfo = cipcinfo;

            base.Addterminalaction_to_Data();
            base.data += this.cipcinfo.ToString();
        }
    }
    
}
