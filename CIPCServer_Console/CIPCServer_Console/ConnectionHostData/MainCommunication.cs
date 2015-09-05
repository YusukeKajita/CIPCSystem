using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPCServer_Console.ConnectionHostData
{
    public class MainCommunication
    {
        public enum ConnectionCommand
        {
            Demand,
            End,
            Connect,
            DisConnect,
            DemandStatus,
            ConnectByServerPort,
            DisconnectByServerPort,
            ConnectByUserPort,
            DisconnectByUserPort,
            error
        }
        public ConnectionCommand connection_command { set; get; }
        public int DeletePort { set; get; }

        public class Connection
        {
            public int SenderID;
            public int ReceiverID;
        }
        public Connection connection;

        public MainCommunication(byte[] data)
        {
            this.connection = new Connection();

            UDP_PACKETS_CODER.UDP_PACKETS_DECODER dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
            dec.Source = data;
            switch(dec.get_int())
            {
                case 1:
                    this.connection_command = ConnectionCommand.Demand;
                    break;
                case 9:
                    this.connection_command = ConnectionCommand.End;
                    this.DeletePort = dec.get_int();
                    break;
                case 5:
                    this.connection_command = ConnectionCommand.Connect;
                    this.connection.SenderID = dec.get_int();
                    this.connection.ReceiverID = dec.get_int();
                    break;
                case 6:
                    this.connection_command = ConnectionCommand.DisConnect;
                    this.connection.SenderID = dec.get_int();
                    this.connection.ReceiverID = dec.get_int();
                    break;
                case 11:
                    this.connection_command = ConnectionCommand.ConnectByServerPort;
                    this.connection.SenderID = dec.get_int();
                    this.connection.ReceiverID = dec.get_int();
                    break;
                case 12:
                    this.connection_command = ConnectionCommand.DisconnectByServerPort;
                    this.connection.SenderID = dec.get_int();
                    this.connection.ReceiverID = dec.get_int();
                    break;
                case 13:
                    this.connection_command = ConnectionCommand.ConnectByUserPort;
                    this.connection.SenderID = dec.get_int();
                    this.connection.ReceiverID = dec.get_int();
                    break;
                case 14:
                    this.connection_command = ConnectionCommand.DisconnectByUserPort;
                    this.connection.SenderID = dec.get_int();
                    this.connection.ReceiverID = dec.get_int();
                    break;
                default:
                    this.connection_command = ConnectionCommand.error;
                    break;
            }
        }
    }
}
