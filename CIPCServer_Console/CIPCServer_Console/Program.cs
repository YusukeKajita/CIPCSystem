using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPCServer_Console
{
    class Program
    {
        static CIPCServer.MainServer mainserver;
        static void Main(string[] args)
        {
            StartMethod();

            Init_Classes();

            bool IsDone = false;
            while (!IsDone)
            {
                try
                {
                    string input = Console.ReadLine();
                    string[] strargs = input.Split(' ');
                    switch (strargs[0])
                    {
                        case "exit":
                        case "Exit":
                        case "Close":
                        case "close":
                            Console.WriteLine("=> Go!");
                            IsDone = true;
                            break;
                        case "Current":
                        case "current":
                        case "state":
                        case "State":
                        case "st":
                            Console.WriteLine("=> Go!");
                            mainserver.PrintState();
                            break;
                        case "deleteclient":
                        case "DeleteClient":
                        case "dclt":
                            Console.WriteLine("=> Go!");
                            mainserver.ClientHostDeleteByID(int.Parse(strargs[1]));
                            break;
                        case "disconnect":
                        case "Disconnect":
                        case "dcnt":
                            Console.WriteLine("=> Go!");
                            mainserver.DisconnectClientHostsCloseByIndex(int.Parse(strargs[1]));
                            break;
                        case "connect":
                        case "Connect":
                        case "cnt":
                            Console.WriteLine("=> Go!");
                            mainserver.ConnectClientHostsByIndex(int.Parse(strargs[1]), int.Parse(strargs[2]));
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    string str = "CommandChecker";
                    Errorlog.Print(ex, str);
                }
            }

            mainserver.Close();
        }

        private static void Init_Classes()
        {
            mainserver = new CIPCServer.MainServer();
        }

        private static void StartMethod()
        {
            System.Diagnostics.FileVersionInfo ver = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Console.WriteLine("CIPCServer");
            Console.WriteLine(ver.ToString());
        }
    }
}
