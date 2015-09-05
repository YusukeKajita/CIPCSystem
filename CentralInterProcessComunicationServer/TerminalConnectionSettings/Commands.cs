using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalConnectionSettings
{
    public enum Sender
    {
        CIPCTerminal,
        CIPCSever
    }

    public enum TerminalCommand
    {
        Connect,
        DisConnect,
        DemandInfo,
        Close,
        Restart,
        CreateClient,
        Undo,
        Redo,
        Emergence,
        AllDisConnect,
        LoadConnectionFast,
        SaveConnectionFast,
        TurnOnSyncConnect,
        TurnOffSyncConnect
    }
    public enum ServerCommand
    {
        ReplyInfo,
        ReportInfo,
        Emergence
    }

    public class Command : EventArgs
    {
        public Sender sendertype;
        public TerminalCommand terminalcommand;
        public ServerCommand servercommand;
        protected string data;
        public string Data
        {
            get
            {
                return data;
            }
        }
    }

    public class CommandBuffer
    {
        public List<Command> CommandList;

        public CommandBuffer()
        {
            this.CommandList = new List<Command>();
        }

        public void AddCommand(Command command)
        {
            this.CommandList.Add(command);
        }
    }
}
