using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CentralInterProcessCommunicationServer
{
    /// <summary>
    /// ファイルの読み書きに対応したリストを作成します
    /// リストに追加できるのは値型のみです
    /// </summary>
    public class FileIO<Type>
        where Type : struct
    {
        private List<Type> data;

        #region propaty
        public List<Type> Data 
        {
            set 
            {
                this.data = value;
            }
            get 
            {
                return this.data;
            }
        }
        #endregion

        public FileIO()
        {
            data = new List<Type>();
        }

        public void Save(string path)
        {
            using(StreamWriter streamwriter = new StreamWriter(path,true))
            {
                string str = "";
                foreach (var _data in data) 
                {
                    str += _data.ToString();
                    str += ",";
                }
                streamwriter.WriteLine(str);
            }
        }

        public void Read(string path)
        {
            System.Type tpA = typeof(Type);
            System.Reflection.MethodInfo Parse = tpA.GetMethod("Parse", (System.Reflection.BindingFlags)
                                    System.Reflection.BindingFlags.Static |
                                    System.Reflection.BindingFlags.Public |
                                    System.Reflection.BindingFlags.InvokeMethod, null, new System.Type[] { typeof(string) }, null);
            if (Parse == null)
            {
                throw new Exception(tpA.FullName + "にはParseがありません。");
            }


            using (StreamReader streamreader = new StreamReader(path))
            {
                string data = streamreader.ReadLine();
                int t = 0;
                for (int i = 0; i < data.Length; i++)
                {
                    if(data[i]==',')
                    {
                        dynamic _data = data.Substring(t,i-t);
                        this.data.Add((Type)Parse.Invoke(tpA, new object[] { _data }));
                        t = i+1;
                    }
                }
            }
        }
    }
}
