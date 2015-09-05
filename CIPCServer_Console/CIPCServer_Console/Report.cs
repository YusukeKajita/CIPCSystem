using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPCServer_Console
{
    public class Report
    {
        public static void Print(string str, object obj)
        {
            Console.WriteLine("[" + obj.ToString() + "] " + str);
        }
        public static void PrintHorizontalBar(object obj)
        {
            Console.WriteLine("------------" + obj.ToString() + "------------");
        }
        public static void PrintDateBar(object obj)
        {
            Console.WriteLine("------------" + DateTime.Now.ToString("yyyy_MM/dd HH:mm:ss:ff") + "------------");
        }
    }
}
