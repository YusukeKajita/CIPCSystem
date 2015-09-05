using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPCServer_Console.CIPCServer
{
    public class Settings
    {
        #region field
        private int _PortNum;
        private int _StartPort;
        #endregion

        #region property
        public string PortNum
        {
            set
            {
                this._PortNum = int.Parse(value);
            }
            get
            {
                return this._PortNum.ToString();
            }
        }

        public string StartPort
        {
            set
            {
                this._StartPort = int.Parse(value);
            }
            get
            {
                return this._StartPort.ToString();
            }
        }
        #endregion

        #region method
        public void Initialize()
        {
            Console.WriteLine("Init Start.");

            Console.WriteLine("ServerPort : ");
            this.PortNum = Console.ReadLine();

            Console.WriteLine("StartPort : ");
            this.StartPort = Console.ReadLine();

            Console.WriteLine("Initted.");
        }
        #endregion
    }
}
