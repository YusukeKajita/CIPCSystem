using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamController
{
    public enum MODE { 
        Sender,
        Receiver
    };

    public struct StreamClient
    {
        public string name { set; get; }
        public int myport { set; get; }
        public string serverIP { set; get; }
        public int serverport { set; get; }
        public MODE mode { set; get; }
        public int fps { set; get; }
        public string filename { set; get; }
    }
}
