using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPCServer_Console
{
    public class Errorlog
    {
        public static void Print(Exception ex, object obj)
        {
            Console.WriteLine("[" + obj.ToString() + "]" + ex.Message);
        }
    }
}
