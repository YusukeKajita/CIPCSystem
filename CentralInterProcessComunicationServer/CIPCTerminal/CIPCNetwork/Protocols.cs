using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPCTerminal.CIPCNetwork
{
    public class Protocols
    {
        public class Message
        {
            public enum HeaderType
            {
                Default,
                GetFileList,
                SendFileList,

                Num
            }
            public HeaderType Header;
            public string Data;

            public Message()
            {
                this.Header = HeaderType.Default;
                this.Data = "";
            }
            public override string ToString()
            {
                return this.Header.ToString() + " " + this.Data;
            }

            public static Message SetFromString(string string_message)
            {
                Message msg = new Message();
                string[] str =  string_message.Split(' ');
                for(int i=0;i<(int)HeaderType.Num;i++){
                    if(str[0] == ((HeaderType)i).ToString()){
                        msg.Header = (HeaderType)i;
                    }
                }
                msg.Data = string_message.Substring(string_message.IndexOf(' ') + 1);
                return msg; 
            }
            
        }



    }
}
