using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.IO;
using System.Collections.ObjectModel;

namespace CentralInterProcessCommunicationServer
{
    

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields
        private DispatcherTimer dispatchertimer;

        /// <summary>
        /// RemoteHostのリストを保有するクラス
        /// </summary>
        private RemoteHostServer rhServer;
        /// <summary>
        /// データコネクションを行うインスタンス　ConnectionListを保有する
        /// </summary>
        private DATA_CONNECTION.DataConnectionServer DCS;
        public DATA_CONNECTION.DataConnectionServer DataConnectionServer
        {
            get
            {
                return this.DCS;
            }
        }

        private TerminalConnection.TerminalConnection TC;
        private int old_Num_Port = 0;
        /// <summary>
        /// デバッグ用ウィンドウ　各サーバでこのインスタンスの参照を利用してデバッグに出力する
        /// </summary>
        private DebugWindow debugwindow;
        public MinimizedWindow minimizedwindow;
        public List<System.Diagnostics.Process> List_Processes;
        #endregion

        public MainWindow()
        {
            try
            {
                int minWorkerThread, minCompletionPortThread;
                ThreadPool.GetMinThreads(out minWorkerThread, out minCompletionPortThread);
                ThreadPool.SetMinThreads(1000, minCompletionPortThread);



                #region window initialize
                this.Title = "CentralInterProcessCommunicationServer";
                this.MouseLeftButtonDown += (sender, e) => this.DragMove();

                this.Closing += MainWindow_Closing;
                #endregion

                #region DispatcherTimer

                dispatchertimer = new DispatcherTimer(DispatcherPriority.Normal);
                dispatchertimer.Interval = new TimeSpan(0, 0, 1);
                dispatchertimer.Tick += dispatchertimer_Tick;
                dispatchertimer.Start();

                #endregion DispatcherTimer

                try
                {
                    #region Minimizedwindow and debugwindow
                    this.debugwindow = new DebugWindow();
                    this.minimizedwindow = new MinimizedWindow();
                    this.minimizedwindow.mainwindow = this;
                    #endregion
                    #region RemoteHostServer
                    this.rhServer = new RemoteHostServer();
                    this.rhServer.debugwindow = this.debugwindow;
                    #endregion
                    #region DataConnectionServer
                    this.DCS = new DATA_CONNECTION.DataConnectionServer();
                    this.DCS.debugwindow = this.debugwindow;
                    #endregion
                    #region TerminalConnection
                    this.TC = new TerminalConnection.TerminalConnection();
                    this.TC.debugwindow = this.debugwindow;
                    this.TC.RHS = this.rhServer;
                    this.TC.DCS = this.DCS;
                    this.TC.mainwindow = this;
                    this.AddTCFunction();

                    this.rhServer.terminalconnection = this.TC;
                    #endregion
                    #region ProcessList
                    this.List_Processes = new List<System.Diagnostics.Process>();
                    #endregion
                    this.rhServer.parent = this;
                    InitializeComponent();
                    debugwindow.DebugLog = "[CIPCServer]CIPCServerを開始します．";
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
                }
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
            }

        }

        private void AddTCFunction()
        {
            this.TC.Eventer.Close += Eventer_Close;
            this.TC.Eventer.DemmandInfo += Eventer_DemmandInfo;
            this.TC.Eventer.Restart += Eventer_Restart;
            this.TC.Eventer.Connect += Eventer_Connect;
            this.TC.Eventer.DisConnect += Eventer_DisConnect;
            this.TC.Eventer.AllDisConnect += Eventer_AllDisConnect;
            this.TC.Eventer.LoadConnectionFast += Eventer_LoadConnectionFast;
            this.TC.Eventer.SaveConnectionFast += Eventer_SaveConnectionFast;
            this.TC.Eventer.TurnOnSyncConnect += Eventer_TurnOnSyncConnect;
            this.TC.Eventer.TurnOffSyncConnect += Eventer_TurnOffSyncConnect;
        }

        void Eventer_TurnOffSyncConnect(object sender, TerminalConnectionSettings.TerminalProtocols.TurnOffSyncConnect e)
        {
            try
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.CheckBox_IsSyncReceived.IsChecked = false;
                }));
            }
            catch (Exception ex)
            {
                this.debugwindow.DebugLog = "[" + this.ToString() + "]" + ex.Message;
            }
        }

        void Eventer_TurnOnSyncConnect(object sender, TerminalConnectionSettings.TerminalProtocols.TurnOnSyncConnect e)
        {
            try
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.CheckBox_IsSyncReceived.IsChecked = true;
                }));
            }
            catch (Exception ex)
            {
                this.debugwindow.DebugLog = "[" + this.ToString() + "]" + ex.Message;
            }
        }

        void Eventer_DemmandInfo(object sender, TerminalConnectionSettings.TerminalProtocols.DemmandInfo e)
        {
            try
            {
                this.TC.Tcp_Send();
            }
            catch (Exception ex)
            {
                this.debugwindow.DebugLog = "[" + this.ToString() + "]" + ex.Message;
            }
        }

        void Eventer_DisConnect(object sender, TerminalConnectionSettings.TerminalProtocols.DisConnect e)
        {
            try
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.checkid_sender = e.SenderPort;
                    this.checkid_receiver = e.ReceiverPort;
                    this.DCS.delete_connection(this.rhServer.List_RemoteHost.Find(HostFindFromNetwork_sender), this.rhServer.List_RemoteHost.Find(HostFindFromNetwork_receiver));
                    this.DCS.ListBox_update(this.LISTBOX_DATA_CONNECTION);
                    this.ConnectionChangeNoticetoTerminal();
                }));
            }
            catch (Exception ex)
            {
                this.debugwindow.DebugLog = "[" + this.ToString() + "]" + ex.Message;
            }
        }

        void Eventer_SaveConnectionFast(object sender, TerminalConnectionSettings.TerminalProtocols.SaveConnectionFast e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.ConnectionSettingSaveFast();
            }));
        }

        void Eventer_LoadConnectionFast(object sender, TerminalConnectionSettings.TerminalProtocols.LoadConnectionFast e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.ConnectionSettingLoadFast();
            }));
        }

        void Eventer_AllDisConnect(object sender, TerminalConnectionSettings.TerminalProtocols.AllDisConnect e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.AllDisConnect();
            }));
        }

        void Eventer_Connect(object sender, TerminalConnectionSettings.TerminalProtocols.Connect e)
        {
            try
            {
                this.Dispatcher.BeginInvoke(new Action(() => 
                { 
                    this.checkid_sender = e.SenderPort;
                    this.checkid_receiver = e.ReceiverPort;
                    this.DCS.add_connection(this.rhServer.List_RemoteHost.Find(HostFindFromNetwork_sender), this.rhServer.List_RemoteHost.Find(HostFindFromNetwork_receiver), this.CheckBox_IsSyncReceived.IsChecked == true ? true : false);
                    this.DCS.ListBox_update(this.LISTBOX_DATA_CONNECTION);
                    this.ConnectionChangeNoticetoTerminal();
                }));
            }
            catch (Exception ex)
            {
                this.debugwindow.DebugLog = "[" + this.ToString() + "]" + ex.Message;
            }
        }

        private bool HostFindFromNetwork_receiver(RemoteHost obj)
        {
            if (this.checkid_receiver == obj.ID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool HostFindFromNetwork_sender(RemoteHost obj)
        {
            if (this.checkid_sender == obj.ID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        void Eventer_Restart(object sender, TerminalConnectionSettings.TerminalProtocols.Restart e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.Close();
                Thread.Sleep(300);
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            }));
        }

        void Eventer_Close(object sender, TerminalConnectionSettings.TerminalProtocols.Close e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.Close();
            }));
        }

        

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                this.debugwindow.Close();
                this.minimizedwindow.Close();
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
            }
        }



        private void dispatchertimer_Tick(object sender, EventArgs e)
        {
            try
            {
                #region test

                //List<TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Client> clientlist = new List<TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Client>();
                //List<TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Connection> connectionlist = new List<TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Connection>();
                //clientlist.Add(new TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Client(1000, "ohohoh", 2000, "192.168.11.1", 60));
                //clientlist.Add(new TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Client(1000, "ohohoh", 2000, "192.168.11.1", 60));
                //connectionlist.Add(new TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Connection(200, 1000));
                //TerminalConnectionSettings.ServerProtocols.ReportInfo reportinfo = new TerminalConnectionSettings.ServerProtocols.ReportInfo(new TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo(clientlist, connectionlist));
                //TerminalConnectionSettings.CommandEventer messagereader = new TerminalConnectionSettings.CommandEventer();

                //messagereader.ReportInfo += messagereader_ReportInfo;
                //messagereader.Handle(reportinfo.Data);

                //System.Text.Encoding enc = System.Text.Encoding.UTF8;
                //byte[] sendBytes = enc.GetBytes(reportinfo.Data);
                //string str = enc.GetString(sendBytes);
                
                #endregion

                text1.Text =
                     "工程名　　　　：　" + System.Diagnostics.Process.GetCurrentProcess().ProcessName + "\n"
                    + "現在時刻　　　：　" + DateTime.Now.ToLongTimeString() + "\n"
                    + "使用ポート数　：　" + rhServer.host_num + "\n"
                    + "制御用ポート　：　" + Definitions.REMOTEHOSTSERVER_PORT + "　■■　ターミナル用ポート　：　" + Definitions.TERMINALCONNECTION_PORT;
                char[] sepalater = { '\n' };
                string[] strs = this.debugwindow.DebugLog.Split(sepalater,StringSplitOptions.RemoveEmptyEntries);
                this.TextBlock_Status.Text = strs[strs.Length - 1];

                if (rhServer.Num_Port != this.old_Num_Port)
                {
                    rhServer.Listupdate(LISTVIEW1);
                }
                this.old_Num_Port = rhServer.Num_Port;
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
            }
        }



        private void update_button_clicked(object sender, RoutedEventArgs e)
        {
            rhServer.Listupdate(LISTVIEW1);
        }

        private void close_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Close();
            }catch(Exception ex)
            {
                while (true)
                {
                    myDialog dialog = new myDialog(ex.Message);
                    if (dialog.ShowDialog() == true)
                    {
                        break;
                    }
                }
            }
        }

        private void Connect_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.Parse(this.sender_port.Text) != 0 && int.Parse(this.receiver_port.Text) != 0)
                {
                    try
                    {
                        this.DCS.add_connection(this.rhServer.List_RemoteHost.Find(HostFindFromPort_sender), this.rhServer.List_RemoteHost.Find(HostFindFromPort_receiver), this.CheckBox_IsSyncReceived.IsChecked == true ? true : false);
                        this.DCS.ListBox_update(this.LISTBOX_DATA_CONNECTION);
                        this.ConnectionChangeNoticetoTerminal();
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
                    }
                }
                else
                {
                    while (true)
                    {
                        myDialog dialog = new myDialog("ポートの指定が不適切です");
                        if (dialog.ShowDialog() == true)
                        {
                            break;
                        }
                    }
                }
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
            }
        }

        private bool HostFindFromPort_sender(RemoteHost obj)
        {
            if (int.Parse(this.sender_port.Text) == obj.ID)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }
        private bool HostFindFromPort_receiver(RemoteHost obj)
        {
            if (int.Parse(this.receiver_port.Text) == obj.ID)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }

        private void Cut_Button_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                if (int.Parse(this.sender_port.Text) != 0 && int.Parse(this.receiver_port.Text) != 0)
                {
                    try
                    {
                        this.DCS.delete_connection(this.rhServer.List_RemoteHost.Find(HostFindFromPort_sender), this.rhServer.List_RemoteHost.Find(HostFindFromPort_receiver));
                        this.DCS.ListBox_update(this.LISTBOX_DATA_CONNECTION);
                        this.ConnectionChangeNoticetoTerminal();
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
                    }
                }
                else
                {
                    while (true)
                    {
                        myDialog dialog = new myDialog("ポートの指定が不適切です");
                        if (dialog.ShowDialog() == true)
                        {
                            break;
                        }
                    }
                }
                  
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
            }
        }

        private void LISTBOX_DATA_CONNECTION_selectionchanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (this.DCS.DataConnectionCount > 0 && LISTBOX_DATA_CONNECTION.SelectedIndex >= 0)
                {
                    DATA_CONNECTION.MyDataConnection MDC = this.DCS.get_SelectedDataConnection(this.LISTBOX_DATA_CONNECTION.SelectedIndex);
                    this.sender_port.Text = MDC.SENDER.ID.ToString();
                    this.receiver_port.Text = MDC.RECEIVER.ID.ToString();
                }
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
            }
        }

        private void All_Cut_Button_Click(object sender, RoutedEventArgs e)
        {
            this.AllDisConnect();
        }

        private void AllDisConnect()
        {
            try
            {
                this.DCS.delete_all_connection();
                this.DCS.ListBox_update(this.LISTBOX_DATA_CONNECTION);
                this.ConnectionChangeNoticetoTerminal();
            }
            catch (Exception ex)
            {
                this.debugwindow.DebugLog = "[" + this.ToString() + "]" + ex.Message;
            }
        }

        private void Button_ConnectionSettingSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.DCS.DataConnectionCount > 0)
                {
                    Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                    dlg.FileName = "connectionsetting.dcs"; // Default file name
                    dlg.DefaultExt = ".dcs"; // Default file extension
                    dlg.Filter = "DataConnectionSetting (.dcs)|*.dcs"; // Filter files by extension
                    dlg.CheckFileExists = false;
                    dlg.CheckPathExists = false;

                    // Show open file dialog box
                    Nullable<bool> result = dlg.ShowDialog();

                    string file;
                    // Process open file dialog box results
                    if (result == true)
                    {
                        // Open document
                        file = dlg.FileName;
                    }
                    else
                    {
                        throw new Exception("保存を中止しました．");
                    }
                    FileStream fs = new FileStream(file, FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(this.DCS.DataConnectionCount);
                    for (int i = 0; i < this.DCS.DataConnectionCount; i++)
                    {
                        DATA_CONNECTION.MyDataConnection MDC = this.DCS.get_SelectedDataConnection(i);
                        sw.WriteLine(MDC.SENDER.remotePort + "," + MDC.RECEIVER.remotePort);
                    }
                    sw.Close();
                    fs.Close();
                    debugwindow.DebugLog = "[DataConnectionSever]設定を保存しました．ファイル名：" + file;
                    while (true)
                    {
                        myDialog dialog = new myDialog("ファイルに設定情報を書き込みました．");
                        if (dialog.ShowDialog() == true)
                        {
                            break;
                        }
                    }
                }
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
            }
        }

        int checkid_sender;
        int checkid_receiver;
        private void Button_ConnectionSettingLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.FileName = "connectionsetting.dcs"; // Default file name
                dlg.DefaultExt = ".dcs"; // Default file extension
                dlg.Filter = "DataConnectionSetting (.dcs)|*.dcs"; // Filter files by extension
                dlg.CheckFileExists = false;
                dlg.CheckPathExists = false;

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                string file;
                // Process open file dialog box results
                if (result == true)
                {
                    // Open document
                    file = dlg.FileName;
                }
                else
                {
                    throw new Exception("読込を中止しました．");
                }

                this.DCS.delete_all_connection();
                this.DCS.ListBox_update(this.LISTBOX_DATA_CONNECTION);

                FileStream fs = new FileStream(file, FileMode.Open);
                StreamReader sr = new StreamReader(fs);

                int size = int.Parse(sr.ReadLine());

                for (int i = 0; i < size; i++)
                {
                    string line = sr.ReadLine();
                    var parts = line.Split(',');
                    this.checkid_sender = int.Parse(parts[0]);
                    this.checkid_receiver = int.Parse(parts[1]);
                    this.DCS.add_connection(this.rhServer.List_RemoteHost.Find(HostFindFromFile_sender), this.rhServer.List_RemoteHost.Find(HostFindFromFile_receiver), this.CheckBox_IsSyncReceived.IsChecked == true ? true : false);
                    this.DCS.ListBox_update(this.LISTBOX_DATA_CONNECTION);
                }
                sr.Close();
                fs.Close();
                debugwindow.DebugLog = "[DataConnectionSever]設定を読み込みました．ファイル名：" + file;
                this.ConnectionChangeNoticetoTerminal();
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
            }
        }

        private bool HostFindFromFile_receiver(RemoteHost obj)
        {
            if (checkid_receiver == int.Parse(obj.remotePort))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool HostFindFromFile_sender(RemoteHost obj)
        {
            if (checkid_sender == int.Parse(obj.remotePort))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void CheckBox_LogWindow_Checked(object sender, RoutedEventArgs e)
        {
            this.debugwindow.Show();
        }

        private void CheckBox_LogWindow_Unchecked(object sender, RoutedEventArgs e)
        {
            this.debugwindow.Hide();
        }

        private void Button_Restart_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Thread.Sleep(300);
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
        }

        private void Button_Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            this.CheckBox_LogWindow.IsChecked = false;
            this.minimizedwindow.Show();
        }

        private void Button_Lunch_StreamController_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(@"..\..\..\StreamController\bin\Debug\StreamController.exe");
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
            }
        }

        private void Button_Lunch_StreamAnalyzer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(@"..\..\..\StreamAnalyzer\bin\Debug\StreamAnalyzer.exe");
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
            }
        }

        private void Button_ConnectionSettingSaveFast_Click(object sender, RoutedEventArgs e)
        {
            this.ConnectionSettingSaveFast();
        }
        private void ConnectionSettingSaveFast()
        {
            try
            {
                string file = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\default.dcs";
                FileStream fs = new FileStream(file, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(this.DCS.DataConnectionCount);
                for (int i = 0; i < this.DCS.DataConnectionCount; i++)
                {
                    DATA_CONNECTION.MyDataConnection MDC = this.DCS.get_SelectedDataConnection(i);
                    sw.WriteLine(MDC.SENDER.remotePort + "," + MDC.RECEIVER.remotePort);
                }
                sw.Close();
                fs.Close();
                debugwindow.DebugLog = "[DataConnectionSever]設定を保存しました．ファイル名：" + file;
            }
            catch (Exception ex)
            {
                debugwindow.DebugLog = "[" + this.ToString() + "]" + ex.Message;
            }
        }

        private void Button_ConnectionSettingLoadFast_Click(object sender, RoutedEventArgs e)
        {
            this.ConnectionSettingLoadFast();
        }

        private void ConnectionSettingLoadFast()
        {
            try
            {
                string file = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\default.dcs";
                FileStream fs = new FileStream(file, FileMode.Open);
                StreamReader sr = new StreamReader(fs);

                int size = int.Parse(sr.ReadLine());

                for (int i = 0; i < size; i++)
                {
                    string line = sr.ReadLine();
                    var parts = line.Split(',');
                    this.checkid_sender = int.Parse(parts[0]);
                    this.checkid_receiver = int.Parse(parts[1]);
                    this.DCS.add_connection(this.rhServer.List_RemoteHost.Find(HostFindFromFile_sender), this.rhServer.List_RemoteHost.Find(HostFindFromFile_receiver), this.CheckBox_IsSyncReceived.IsChecked == true ? true : false);
                    this.DCS.ListBox_update(this.LISTBOX_DATA_CONNECTION);
                }
                sr.Close();
                fs.Close();
                debugwindow.DebugLog = "[DataConnectionSever]設定を読み込みました．ファイル名：" + file;
                this.ConnectionChangeNoticetoTerminal();
            }
            catch (Exception ex)
            {
                debugwindow.DebugLog = "[" + this.ToString() + "]" + ex.Message;
            }
        }

        private void ConnectionChangeNoticetoTerminal()
        {
            this.TC.Tcp_Send();
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
            this.CheckBox_LogWindow.IsChecked = false;
            this.minimizedwindow.Show();
        }

        private void Button_DeleteRemoteHost_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.rhServer.ShutDownDisconnect(int.Parse(this.TextBox_deleteremotehost.Text));
            }
            catch (Exception ex)
            {
                debugwindow.DebugLog = "[" + this.ToString() + "]" + ex.Message;
            }
        }

        private void LISTVIEW1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (this.LISTVIEW1.SelectedIndex == -1) return;
                this.TextBox_deleteremotehost.Text = this.rhServer.Clients[this.LISTVIEW1.SelectedIndex].Port.ToString();
                if (this.rhServer.Clients[this.LISTVIEW1.SelectedIndex].mode == "Sender")
                {
                    this.sender_port.Text = this.rhServer.Clients[this.LISTVIEW1.SelectedIndex].Port.ToString();
                }
                else if (this.rhServer.Clients[this.LISTVIEW1.SelectedIndex].mode == "Receiver")
                {
                    this.receiver_port.Text = this.rhServer.Clients[this.LISTVIEW1.SelectedIndex].Port.ToString();
                }
            }
            catch (Exception ex)
            {
                debugwindow.DebugLog = "[" + this.ToString() + "]" + ex.Message;
            }
        }
    }
}
