using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalConnectionSettings.TerminalProtocols
{

    public class CIPCTerminalCommand : Command
    {
        public CIPCTerminalCommand()
        {
            base.sendertype = Sender.CIPCTerminal;
            base.data = sendertype.ToString() + "\\";
        }

        protected void Addterminalaction_to_Data()
        {
            base.data += base.terminalcommand.ToString() + "\\";
        }
    }

    /// <summary>
    /// 二つのポートを指定しその接続設定を追加する要求
    /// </summary>
    public class Connect : CIPCTerminalCommand
    {
        public int SenderPort;
        public int ReceiverPort;
        public Connect(int SenderPort, int ReceiverPort)
        {
            base.terminalcommand = TerminalCommand.Connect;
            this.SenderPort = SenderPort;
            this.ReceiverPort = ReceiverPort;

            base.Addterminalaction_to_Data();
            base.data += this.SenderPort.ToString() + "\\" + this.ReceiverPort.ToString() + "\\";
        }
    }

    /// <summary>
    /// 二つのポートを指定しその接続設定を削除する要求
    /// </summary>
    public class DisConnect : CIPCTerminalCommand
    {
        public int SenderPort;
        public int ReceiverPort;
        public DisConnect(int SenderPort, int ReceiverPort)
        {
            base.terminalcommand = TerminalCommand.DisConnect;
            this.SenderPort = SenderPort;
            this.ReceiverPort = ReceiverPort;

            base.Addterminalaction_to_Data();
            base.data += this.SenderPort.ToString() + "\\" + this.ReceiverPort.ToString() + "\\";
        }
    }

    /// <summary>
    /// すべての接続設定を削除する要求
    /// </summary>
    public class AllDisConnect : CIPCTerminalCommand
    {
        public AllDisConnect()
        {
            base.terminalcommand = TerminalCommand.AllDisConnect;
            base.Addterminalaction_to_Data();
        }
    }

    /// <summary>
    /// 接続設定を簡易読込みさせる要求
    /// </summary>
    public class LoadConnectionFast : CIPCTerminalCommand
    {
        public LoadConnectionFast()
        {
            base.terminalcommand = TerminalCommand.LoadConnectionFast;
            base.Addterminalaction_to_Data();
        }
    }
    /// <summary>
    /// 接続設定を簡易保存させる要求
    /// </summary>
    public class SaveConnectionFast : CIPCTerminalCommand
    {
        public SaveConnectionFast()
        {
            base.terminalcommand = TerminalCommand.SaveConnectionFast;
            base.Addterminalaction_to_Data();
        }
    }

    /// <summary>
    /// 現在のCIPCSの状態を要求
    /// </summary>
    public class DemmandInfo : CIPCTerminalCommand
    {
        public DemmandInfo()
        {
            base.terminalcommand = TerminalCommand.DemandInfo;
            base.Addterminalaction_to_Data();
        }
    }

    /// <summary>
    /// システムの終了を要求
    /// </summary>
    public class Close : CIPCTerminalCommand
    {
        public Close()
        {
            base.terminalcommand = TerminalCommand.Close;
            base.Addterminalaction_to_Data();
        }
    }

    /// <summary>
    /// システムの再起動を要求
    /// </summary>
    public class Restart : CIPCTerminalCommand
    {
        public Restart()
        {
            base.terminalcommand = TerminalCommand.Restart;
            base.Addterminalaction_to_Data();
        }
    }

    /// <summary>
    /// ひとつ前のコマンドを巻き戻す要求
    /// </summary>
    public class Undo : CIPCTerminalCommand
    {
        public Undo()
        {
            base.terminalcommand = TerminalCommand.Undo;
            base.Addterminalaction_to_Data();
        }
    }

    /// <summary>
    /// ひとつ前のコマンドをもう一度行う要求 ただし，ひとつ前がAndoだった場合，Andoを取り消す
    /// </summary>
    public class Redo : CIPCTerminalCommand
    {
        public Redo()
        {
            base.terminalcommand = TerminalCommand.Redo;
            base.Addterminalaction_to_Data();
        }
    }

    /// <summary>
    /// 非常事態を警告
    /// </summary>
    public class Emergence : CIPCTerminalCommand
    {
        public Emergence()
        {
            base.terminalcommand = TerminalCommand.Emergence;
            base.Addterminalaction_to_Data();
        }
    }

    /// <summary>
    /// SyncModeに変更
    /// </summary>
    public class TurnOnSyncConnect : CIPCTerminalCommand
    {
        public TurnOnSyncConnect()
        {
            base.terminalcommand = TerminalCommand.TurnOnSyncConnect;
            base.Addterminalaction_to_Data();
        }
    }

    /// <summary>
    /// SyncModeを解除
    /// </summary>
    public class TurnOffSyncConnect : CIPCTerminalCommand
    {
        public TurnOffSyncConnect()
        {
            base.terminalcommand = TerminalCommand.TurnOffSyncConnect;
            base.Addterminalaction_to_Data();
        }
    }
}
