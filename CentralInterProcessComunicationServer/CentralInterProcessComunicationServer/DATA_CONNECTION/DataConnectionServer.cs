using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;

namespace CentralInterProcessCommunicationServer.DATA_CONNECTION
{
    public class DataConnectionServer
    {
        #region field
        public List<MyDataConnection> List_dataconnection { set; get; }
        #endregion


        #region propaties
        public int DataConnectionCount
        {
            get
            {
                return this.List_dataconnection.Count;
            }
        }

        public DebugWindow debugwindow { set; get;}
        #endregion

        #region constructer
        public DataConnectionServer() 
        {
            this.List_dataconnection = new List<MyDataConnection>();
        }
        #endregion

        #region public method
        public void add_connection(RemoteHost sender, RemoteHost receiver, bool IsSync) 
        {
            try
            {
                if (IsSync)
                {
                    this.receiver = receiver;
                    this.sender = sender;
                    if (this.List_dataconnection.Exists(serch_connection))
                    {
                        throw new Exception("接続が存在します");
                    }

                    this.debugwindow.DebugLog = "[DataConnectionServer]データシンクロ接続を追加します．送信側リモートポート：" + sender.remotePort.ToString() + "受信側リモートポート：" + receiver.remotePort.ToString();
                    this.List_dataconnection.Add(new MyDataConnectionSync(sender, receiver));
                }
                else
                {
                    this.receiver = receiver;
                    this.sender = sender;
                    if (this.List_dataconnection.Exists(serch_connection))
                    {
                        throw new Exception("接続が存在します");
                    }

                    this.debugwindow.DebugLog = "[DataConnectionServer]データ接続を追加します．送信側リモートポート：" + sender.remotePort.ToString() + "受信側リモートポート：" + receiver.remotePort.ToString();
                    this.List_dataconnection.Add(new MyDataConnection(sender, receiver));
                }
            }
            catch (Exception ex)
            {
                this.debugwindow.DebugLog = "[" + this.ToString() + "]" + ex.Message;
            }
        }


        private RemoteHost sender;
        private RemoteHost receiver;
        public void delete_connection(RemoteHost sender,RemoteHost receiver) 
        {
            try 
            {
                this.debugwindow.DebugLog = "[DataConnectionServer]データ接続を削除します．送信側リモートポート：" + sender.remotePort.ToString() + "受信側リモートポート：" + receiver.remotePort.ToString();
                this.receiver = receiver;
                this.sender = sender;
                this.List_dataconnection.RemoveAll(serch_connection);
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void delete_all_connection()
        {
            try
            {
                this.debugwindow.DebugLog = "[DataConnectionServer]全データ接続を削除します．";
                this.List_dataconnection.RemoveAll(allconnections);
                this.List_dataconnection.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        

        public void ListBox_update(ListBox mylistbox) 
        {
            try
            {
                if (mylistbox.Items != null)
                {
                    mylistbox.Items.Clear();
                }
                if (this.List_dataconnection.Count == 0) 
                {
                    return;
                }
                foreach (var dataconnection in this.List_dataconnection)
                {
                    StackPanel stp = new StackPanel();
                    stp.Orientation = Orientation.Horizontal;

                    var TB1 = new TextBlock();
                    TB1.Text = dataconnection.SENDER.ID.ToString();
                    stp.Children.Add(TB1);

                    var TB = new TextBlock();
                    TB.Text = " ⇒ ";
                    stp.Children.Add(TB);

                    var TB2 = new TextBlock();
                    TB2.Text = dataconnection.RECEIVER.ID.ToString();
                    stp.Children.Add(TB2);

                    var TB3 = new TextBlock();
                    TB3.Text = dataconnection.IsConnectSync ? " $ Sync Mode" : " | Async Mode" ;
                    stp.Children.Add(TB3);

                    mylistbox.Items.Add(stp);
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }

        public MyDataConnection get_SelectedDataConnection(int index)
        {
            try
            {
                return this.List_dataconnection[index];
            }
            catch (Exception ex)
            {
                while (true)
                {
                    myDialog dialog = new myDialog(ex.Message);
                    if (dialog.ShowDialog() == true)
                    {
                        break;
                    }
                }
                return null;
            }
        }
        #endregion

        #region private method
        private bool serch_connection(MyDataConnection obj)
        {
            if (obj.SENDER == this.sender && obj.RECEIVER == this.receiver)
            {
                obj.Disconnect();
                return true;
            }
            else 
            {
                return false;
            }
        }
        private bool allconnections(MyDataConnection obj)
        {
            obj.Disconnect();
            return true;
        }
        #endregion


    }
}
