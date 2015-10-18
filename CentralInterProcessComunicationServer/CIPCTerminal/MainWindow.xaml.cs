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

namespace CIPCTerminal
{

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        #region field
        #region LocalCIPCSystemCanvas item
        public CanvasContents.CIPCServer CanvasContent_CIPCServer;
        public CanvasContents.CIPCTerminal CanvasContent_CIPCTerminal;
        public List<CanvasContents.CIPCClient> List_CanvasContent_CIPCClient;
        #endregion
        #region LocalCIPCServerCanvas item
        public List<CanvasContents.CIPCServerClientHost> List_CanvasContent_CIPCClientHost;
        public bool IsConnectStartedinUI { set; get; }
        public CanvasContents.CIPCServerClientHost SenderClientHost;
        #endregion
        #region CIPCInfo
        /// <summary>
        /// 接続中のCIPCServerの情報が格納される
        /// </summary>
        public TerminalConnectionSettings.ServerProtocols.ReportInfo CIPCinfo;
        #endregion
        #region NetworkInformation
        public class NetworkInformation
        {
            public class Settings
            {
                public Settings(ListBox myfolders, ListBox myfiles)
                {
                    this.MyFilePathList = new List<string>();
                    this.MyFolderPathList = new List<string>();
                    this.MyFilePathList_prov = new ListProvider<string>(this.MyFilePathList, myfiles);
                    this.MyFolderPathList_prov = new ListProvider<string>(this.MyFolderPathList, myfolders);
                }
                public List<string> MyFolderPathList;
                public ListProvider<string> MyFolderPathList_prov;
                public List<string> MyFilePathList;
                public ListProvider<string> MyFilePathList_prov;
            };
            public Settings settings;
            public NetworkInformation(Settings settings)
            {
                this.settings = settings;
            }
        };
        public NetworkInformation networkinformation;
        #endregion
        #endregion

        #region propaty

        #region CIPCDiagnostics
        public CIPCDiagnostics.CIPCDiagnosticsWindow CIPCDwindow { set; get; }
        public CIPCDiagnostics.OwnCIPCProcess ownprocess { set; get; }
        #endregion

        #region CIPCServerConnection
        /// <summary>
        /// CIPCとのコネクションを確立するクラス
        /// </summary>
        public CIPCServerConnection serverconnection { set; get; }
        /// <summary>
        /// TCPによって接続をする際の操作・情報を表示するウィンドウ
        /// </summary>
        public TCPConnectWindow tcpconnectwindow { set; get; }
        #endregion

        #region DebugWindow
        /// <summary>
        /// デバッグのログを表示するウィンドウ
        /// </summary>
        public myparts.DebugLog debuglog { set; get; }
        #endregion

        public System.Windows.Threading.DispatcherTimer dispatchertimer { set; get; }


        #endregion
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            this.Init_ButtonClickEvents();

            this.Init_Events();

            this.CreateChildWindows();

            this.Init_Classes();

            this.Init_Timer();

            this.InitContent();
        }

        private void InitContent()
        {
            this.CanvasContent_CIPCTerminal = new CanvasContents.CIPCTerminal();
            this.CanvasContent_CIPCTerminal.SetCenterPosition(this.Width / 2, this.Height / 8);
            this.Canvas_CIPCSystem.Children.Add(this.CanvasContent_CIPCTerminal);

            this.Canvas_CIPCServer.MouseRightButtonUp += Canvas_CIPCServer_MouseRightButtonUp;
        }

        void Canvas_CIPCServer_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.IsConnectStartedinUI = false;
            this.SenderClientHost = null;
        }



        private void Init_Classes()
        {
            this.ownprocess = new CIPCDiagnostics.OwnCIPCProcess(this.CIPCDwindow);
            this.serverconnection = new CIPCServerConnection(this.tcpconnectwindow);
            this.serverconnection.Eventer.ReportInfo += Eventer_ReportInfo;
            this.serverconnection.debuglog = this.debuglog;

            this.List_CanvasContent_CIPCClient = new List<CanvasContents.CIPCClient>();
            this.List_CanvasContent_CIPCClientHost = new List<CanvasContents.CIPCServerClientHost>();

            this.networkinformation = new NetworkInformation(new NetworkInformation.Settings(this.ListBox_Network_MyFolderList, this.ListBox_Network_MyFileList));
        }

        /// <summary>
        /// CIPCからデータを受信し、Canvasを更新します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Eventer_ReportInfo(object sender, TerminalConnectionSettings.ServerProtocols.ReportInfo e)
        {
            this.CIPCinfo = e;
            this.UpdateContents();
            this.UpdateCanvasServerContents();
        }



        private void Init_Timer()
        {
            this.dispatchertimer = new System.Windows.Threading.DispatcherTimer(System.Windows.Threading.DispatcherPriority.Normal);
            this.dispatchertimer.Interval = new TimeSpan(0, 0, 1);
            this.dispatchertimer.Tick += dispatchertimer_Tick;
            this.dispatchertimer.Start();
        }

        void dispatchertimer_Tick(object sender, EventArgs e)
        {
            this.ownprocess.Update();
        }
        /// <summary>
        /// 子ウィンドウの生成
        /// </summary>
        private void CreateChildWindows()
        {
            this.tcpconnectwindow = new TCPConnectWindow();
            this.tcpconnectwindow.Show();
            this.tcpconnectwindow.Left *= 0.8;
            this.CIPCDwindow = new CIPCDiagnostics.CIPCDiagnosticsWindow();
            this.CIPCDwindow.Show();
            this.CIPCDwindow.Left *= 1.2;
            this.debuglog = new myparts.DebugLog();
        }
        private void Close_ChildWindows()
        {
            this.tcpconnectwindow.Close();
            this.CIPCDwindow.Close();
            this.debuglog.Close();
        }
        #region buttonclickevents
        /// <summary>
        /// ボタンをクリックした時のイベントの登録
        /// </summary>
        private void Init_ButtonClickEvents()
        {
            this.Button_Close.Click += Button_Close_Click;
            this.Button_ChangeWindowState.Click += Button_ChangeWindowState_Click;
        }

        private void Button_ChangeWindowState_Click(object sender, RoutedEventArgs e)
        {
            switch (this.WindowState)
            {
                case System.Windows.WindowState.Maximized:
                    this.WindowState = System.Windows.WindowState.Normal;
                    this.Button_ChangeWindowState.ToolTip = "ウィンドウを最大化します";
                    break;
                case System.Windows.WindowState.Normal:
                    this.WindowState = System.Windows.WindowState.Maximized;
                    this.Button_ChangeWindowState.ToolTip = "ウィンドウサイズを元に戻します";
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 終了ボタンのクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                while (true)
                {
                    myparts.myDialog dialog = new myparts.myDialog(ex.Message);
                    if (dialog.ShowDialog() == true)
                    {
                        break;
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// statusbarの状態を変更する
        /// </summary>
        /// <param name="text"></param>
        public void Change_StatusBar_TextBlock(string text)
        {
            try
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.TextBlock_StatusCaption.Text = text;
                }));
            }
            catch (Exception ex)
            {
                while (true)
                {
                    myparts.myDialog dialog = new myparts.myDialog(ex.Message);
                    if (dialog.ShowDialog() == true)
                    {
                        break;
                    }
                }
            }
        }

        #region その他のウィンドウイベント
        /// <summary>
        /// GUIイベント以外のイベントの登録
        /// </summary>
        private void Init_Events()
        {
            this.MouseLeftButtonDown += (sender, e) => this.DragMove();
            this.Closing += MainWindow_Closing;
        }

        /// <summary>
        /// メインウィンドウ終了イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {

                myparts.myDialog dialog = new myparts.myDialog("全プロセスを終了します.");
                if (dialog.ShowDialog() == false)
                {
                    e.Cancel = true;
                }
                if (e.Cancel == false)
                {
                    this.dispatchertimer.Stop();
                    this.Close_ChildWindows();
                    this.Dispose_Classes();
                }

            }
            catch (Exception ex)
            {
                while (true)
                {
                    myparts.myDialog dialog = new myparts.myDialog(ex.Message);
                    if (dialog.ShowDialog() == true)
                    {
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// 最終処理が必要なクラスの最終処理を記述
        /// </summary>
        private void Dispose_Classes()
        {
            //this.serverconnection.Dispose();
        }

        /// <summary>
        /// ロードが完了した時点でサブウィンドウを起動する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.CIPCDwindow.mainwindow = this;
            this.CIPCDwindow.Activate();

            this.tcpconnectwindow.mainwindow = this;
            this.tcpconnectwindow.Activate();

            this.debuglog.mainwindow = this;
            this.serverconnection.mainwindow = this;

            this.TabControl_Main.SelectedIndex = 1;
            this.TabControl_Main.SelectedIndex = 0;
        }
        #endregion

        private void MenuItem_Window_CIPCTCPConnectionWindow_Click(object sender, RoutedEventArgs e)
        {
            if (this.tcpconnectwindow.IsVisible)
            {
                this.Hide_tcpconnectwindow();
            }
            else
            {
                this.Show_tcpconnectwindow();
            }
        }
        private void MenuItem_Window_CIPCDiagnosticsWindow_Click(object sender, RoutedEventArgs e)
        {
            if (this.CIPCDwindow.IsVisible)
            {
                this.Hide_cipcdiagnosticswindow();
            }
            else
            {
                this.Show_cipcdiagnosticswindow();
            }
        }

        #region 子ウィンドウの表示・非表示
        public void Show_tcpconnectwindow()
        {
            this.tcpconnectwindow.Show();
            this.tcpconnectwindow.Activate();
            this.Button_Side_CIPCSTCW.Visibility = System.Windows.Visibility.Collapsed;
        }
        public void Hide_tcpconnectwindow()
        {
            this.tcpconnectwindow.Hide();
            this.Button_Side_CIPCSTCW.Visibility = System.Windows.Visibility.Visible;
        }

        public void Show_cipcdiagnosticswindow()
        {
            this.CIPCDwindow.Show();
            this.CIPCDwindow.Activate();
            this.Button_Side_CIPCSDW.Visibility = System.Windows.Visibility.Collapsed;
        }
        public void Hide_cipcdiagnosticswindow()
        {
            this.CIPCDwindow.Hide();
            this.Button_Side_CIPCSDW.Visibility = System.Windows.Visibility.Visible;
        }
        #endregion

        #region CIPCSystemCnavasのUI更新
        /// <summary>
        /// CIPCSystemキャンバス内のコンテンツを更新するメソッド
        /// </summary>
        public void UpdateContents()
        {

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.Canvas_CIPCSystem.Children.Clear();
            }));
            this.CreateLocalCanvasContent_CIPCTerminal();
            this.CreateLocalCanvasContent_CIPCServer();
            this.CreateLocalCanvasContent_Client();
            this.CreateLocalCanvasContent_Lines();
        }
        /// <summary>
        /// CIPCSystemキャンバスにCIPCServerを配置する
        /// </summary>
        private void CreateLocalCanvasContent_CIPCServer()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.CanvasContent_CIPCServer = new CanvasContents.CIPCServer(this.CIPCinfo.cipcinfo);
                this.CanvasContent_CIPCServer.SetCenterPosition(this.Width / 2, this.Height / 3);
                this.CanvasContent_CIPCServer.mainwindow = this;
                this.Canvas_CIPCSystem.Children.Add(this.CanvasContent_CIPCServer);
            }));
        }
        /// <summary>
        /// CIPCSystemキャンバスにCIPCTerminalを配置する
        /// </summary>
        private void CreateLocalCanvasContent_CIPCTerminal()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.CanvasContent_CIPCTerminal = new CanvasContents.CIPCTerminal();
                this.CanvasContent_CIPCTerminal.SetCenterPosition(this.Width / 2, this.Height / 8);
                this.CanvasContent_CIPCTerminal.mainwindow = this;
                this.Canvas_CIPCSystem.Children.Add(this.CanvasContent_CIPCTerminal);
            }));
        }
        /// <summary>
        /// CIPCSystemキャンバスにCIPCClientを配置する
        /// </summary>
        private void CreateLocalCanvasContent_Client()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (this.CIPCinfo.cipcinfo.ClientList.Count != 0)
                {

                    this.List_CanvasContent_CIPCClient.Clear();
                    for (int i = 0; i < this.CIPCinfo.cipcinfo.ClientList.Count; i++)
                    {
                        this.List_CanvasContent_CIPCClient.Add(new CanvasContents.CIPCClient(this.CIPCinfo.cipcinfo.ClientList[i]));
                        this.List_CanvasContent_CIPCClient[i].SetCenterPosition((Math.Cos(Math.PI * (i + 1) / (this.CIPCinfo.cipcinfo.ClientList.Count + 1)) * 0.4 + 0.5) * this.Canvas_CIPCSystem.ActualWidth, (Math.Sin(Math.PI * (i + 1) / (this.CIPCinfo.cipcinfo.ClientList.Count + 1)) * 0.4 + 0.5) * this.Canvas_CIPCSystem.ActualHeight);
                        this.List_CanvasContent_CIPCClient[i].mainwindow = this;
                        this.Canvas_CIPCSystem.Children.Add(this.List_CanvasContent_CIPCClient[i]);
                    }
                }
            }));
        }

        /// <summary>
        /// CIPCSystemキャンバスに接続線を引く
        /// </summary>
        public void CreateLocalCanvasContent_Lines()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.Canvas_CIPCSystem_Lines.Children.Clear();
                if (this.List_CanvasContent_CIPCClient.Count != 0)
                {
                    for (int i = 0; i < this.CIPCinfo.cipcinfo.ClientList.Count; i++)
                    {
                        this.drawline(new Point(Canvas.GetLeft(this.CanvasContent_CIPCServer) + 10, Canvas.GetTop(this.CanvasContent_CIPCServer) + 10 + 20 * i), new Point(Canvas.GetLeft(this.List_CanvasContent_CIPCClient[i]) + 10, Canvas.GetTop(this.List_CanvasContent_CIPCClient[i]) + 10), this.Canvas_CIPCSystem_Lines);
                    }
                }
            }));
        }
        /// <summary>
        /// キャンバスの指定されたポイントに線を引く
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private void drawline(Point a, Point b, Canvas drawcanvas)
        {
            Line line = new Line();
            line.Stroke = new SolidColorBrush(Color.FromArgb(150, 255, 255, 255));
            line.StrokeThickness = 5;
            line.X1 = a.X;
            line.Y1 = a.Y;
            line.X2 = b.X;
            line.Y2 = b.Y;
            drawcanvas.Children.Add(line);
        }
        /// <summary>
        /// CIPCSystemキャンバスから要素を削除し、CIPCTerminalを追加する
        /// </summary>
        public void DeleteContents()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.Canvas_CIPCSystem.Children.Clear();
                CanvasContents.CIPCTerminal Terminal = new CanvasContents.CIPCTerminal();
                Terminal.SetCenterPosition(this.Width / 2, this.Height / 8);
                this.Canvas_CIPCSystem.Children.Add(Terminal);
            }));
        }
        #endregion

        #region CIPCServerCanvasのUI更新
        void UpdateCanvasServerContents()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.Canvas_CIPCServer.Children.Clear();
            }));
            this.CreateCIPCServerConnectionsCanvas_ClientHost();
            this.CreateServerCanvasContent_Lines();
        }

        private void CreateCIPCServerConnectionsCanvas_ClientHost()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (this.CIPCinfo.cipcinfo.ClientList.Count != 0)
                {
                    this.List_CanvasContent_CIPCClientHost.Clear();

                    for (int i = 0; i < this.CIPCinfo.cipcinfo.ClientList.Count; i++)
                    {
                        this.List_CanvasContent_CIPCClientHost.Add(new CanvasContents.CIPCServerClientHost(this.CIPCinfo.cipcinfo.ClientList[i]));
                        this.List_CanvasContent_CIPCClientHost[i].SetCenterPosition((Math.Cos(2 * Math.PI * i / this.CIPCinfo.cipcinfo.ClientList.Count) * 0.4 + 0.5) * this.Canvas_CIPCServer.ActualWidth, (Math.Sin(2 * Math.PI * i / this.CIPCinfo.cipcinfo.ClientList.Count) * 0.4 + 0.5) * this.Canvas_CIPCServer.ActualHeight);
                        this.List_CanvasContent_CIPCClientHost[i].mainwindow = this;
                        this.Canvas_CIPCServer.Children.Add(this.List_CanvasContent_CIPCClientHost[i]);
                    }
                }
                if (this.CIPCinfo.cipcinfo.IsSyncConnect == true)
                {
                    TextBlock TB = new TextBlock();
                    TB.Text = "SyncConnectMode : On";
                    this.Canvas_CIPCServer.Children.Add(TB);
                }
                else
                {
                    TextBlock TB = new TextBlock();
                    TB.Text = "SyncConnectMode : Off";
                    this.Canvas_CIPCServer.Children.Add(TB);
                }
            }));
        }

        private int connection_index;
        /// <summary>
        /// CIPCServerキャンバスに接続線を引く
        /// </summary>
        public void CreateServerCanvasContent_Lines()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.Canvas_CIPCServerLines.Children.Clear();
                if (this.CIPCinfo.cipcinfo.ConnectionList.Count != 0)
                {
                    for (int i = 0; i < this.CIPCinfo.cipcinfo.ConnectionList.Count; i++)
                    {
                        this.connection_index = i;
                        CanvasContents.CIPCServerClientHost sender = this.List_CanvasContent_CIPCClientHost.Find(MatchSender);
                        CanvasContents.CIPCServerClientHost receiver = this.List_CanvasContent_CIPCClientHost.Find(MatchReceiver);
                        if (sender != null && receiver != null)
                        {
                            this.drawline(new Point(Canvas.GetLeft(sender) + 10, Canvas.GetTop(sender) + 10), new Point(Canvas.GetLeft(receiver) + 10, Canvas.GetTop(receiver) + 10), Canvas_CIPCServerLines);
                        }
                    }
                }
            }));
        }

        private bool MatchReceiver(CanvasContents.CIPCServerClientHost obj)
        {
            if (obj.client.MyPort == this.CIPCinfo.cipcinfo.ConnectionList[this.connection_index].receiverport)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool MatchSender(CanvasContents.CIPCServerClientHost obj)
        {
            if (obj.client.MyPort == this.CIPCinfo.cipcinfo.ConnectionList[this.connection_index].senderport)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
        #region サイドボタンのUI応答
        private void Button_Side_CIPCSTCW_Click(object sender, RoutedEventArgs e)
        {
            this.Show_tcpconnectwindow();
        }

        private void Button_Side_CIPCSDW_Click(object sender, RoutedEventArgs e)
        {
            this.Show_cipcdiagnosticswindow();
        }
        #endregion



        private void CheckBox_DebugLog_Checked(object sender, RoutedEventArgs e)
        {
            this.debuglog.Show();
        }

        private void CheckBox_DebugLog_Unchecked(object sender, RoutedEventArgs e)
        {
            this.debuglog.Hide();
        }

        /// <summary>
        /// CIPCにアップデートの要求をだします
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Update_CIPC_Click(object sender, RoutedEventArgs e)
        {
            this.serverconnection.Udp_Send(new TerminalConnectionSettings.TerminalProtocols.DemmandInfo());
        }
        private void Update_LocalCanvas_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateContents();
            this.UpdateCanvasServerContents();
        }

        private void Button_Local_CIPCServer_AllDisConnect_Click(object sender, RoutedEventArgs e)
        {
            this.serverconnection.Udp_Send(new TerminalConnectionSettings.TerminalProtocols.AllDisConnect());
        }

        private void Button_Local_CIPCServer_SaveConnectionSettings_Click(object sender, RoutedEventArgs e)
        {
            this.serverconnection.Udp_Send(new TerminalConnectionSettings.TerminalProtocols.SaveConnectionFast());
        }

        private void Button_Local_CIPCServer_LoadConnectionSettings_Click(object sender, RoutedEventArgs e)
        {
            this.serverconnection.Udp_Send(new TerminalConnectionSettings.TerminalProtocols.LoadConnectionFast());
        }

        private void Button_Local_CIPCServer_Reboot_Click(object sender, RoutedEventArgs e)
        {
            this.serverconnection.Udp_Send(new TerminalConnectionSettings.TerminalProtocols.Restart());
        }

        private void Button_Local_CIPCServer_TurnOnSyncConnect_Click(object sender, RoutedEventArgs e)
        {
            this.serverconnection.Udp_Send(new TerminalConnectionSettings.TerminalProtocols.TurnOnSyncConnect());
        }

        private void Button_Local_CIPCServer_TurnOffSyncConnect_Click(object sender, RoutedEventArgs e)
        {
            this.serverconnection.Udp_Send(new TerminalConnectionSettings.TerminalProtocols.TurnOffSyncConnect());
        }
        #region CIPCNetWorkUIEvent

        private void Button_Network_AddFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string FolderName;
                System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
                folderBrowserDialog1.Description = "公開するフォルダを選択してください．（注意：他人に公開されます．公序良俗に反するものや，個人情報の含まれるものは指定しないようにお気を付けください．）";
                if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    FolderName = folderBrowserDialog1.SelectedPath;
                    this.AddFolderByPath(FolderName);
                }
            }
            catch (Exception ex)
            {
                myparts.myDialog dlg = new myparts.myDialog(ex.Message);
                dlg.ShowDialog();
            }
            
        }

        private void AddFolderByPath(string FolderName)
        {
            if (!this.networkinformation.settings.MyFolderPathList.Contains(FolderName))
            {
                this.networkinformation.settings.MyFolderPathList.Add(FolderName);
                this.networkinformation.settings.MyFolderPathList_prov.Refresh();
            }
            this.AddFileInFolder(FolderName);

            string[] directries = System.IO.Directory.GetDirectories(FolderName, "*", System.IO.SearchOption.AllDirectories);
            foreach (var p in directries)
            {
                if (!this.networkinformation.settings.MyFolderPathList.Contains(p))
                {
                    this.networkinformation.settings.MyFolderPathList.Add(p);
                    this.AddFileInFolder(p);
                }
            }
            this.networkinformation.settings.MyFolderPathList_prov.Refresh();
        }

        private void AddFileInFolder(string FolderName)
        {
            string[] files = System.IO.Directory.GetFiles(FolderName,"*",System.IO.SearchOption.TopDirectoryOnly);
            foreach (var p in files)
            {
                if (!this.networkinformation.settings.MyFilePathList.Contains(p))
                {
                    this.networkinformation.settings.MyFilePathList.Add(p);
                }
            }
            this.networkinformation.settings.MyFilePathList_prov.Refresh();
        }

        private void RemoveFolderByPath(string FolderName)
        {
            this.networkinformation.settings.MyFolderPathList.Remove(FolderName);
            this.networkinformation.settings.MyFolderPathList_prov.Refresh();
            this.RemoveFileInFolder(FolderName);

            string[] directries = System.IO.Directory.GetDirectories(FolderName, "*", System.IO.SearchOption.AllDirectories);
            foreach (var p in directries)
            {
                if (this.networkinformation.settings.MyFolderPathList.Contains(p))
                {
                    this.networkinformation.settings.MyFolderPathList.Remove(p);
                    this.RemoveFileInFolder(p);
                }
            }
            this.networkinformation.settings.MyFolderPathList_prov.Refresh();
        }

        private void RemoveFileInFolder(string FolderName)
        {
            string[] files = System.IO.Directory.GetFiles(FolderName, "*", System.IO.SearchOption.TopDirectoryOnly);
            foreach (var p in files)
            {
                if (this.networkinformation.settings.MyFilePathList.Contains(p))
                {
                    this.networkinformation.settings.MyFilePathList.Remove(p);
                }
            }
            this.networkinformation.settings.MyFilePathList_prov.Refresh();
        }

        private void ListBox_Network_MyFileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string str = e.AddedItems[0] as string;
                System.IO.FileInfo fi = new System.IO.FileInfo(str);
                this.TextBlock_Network_MyFileStatus.Text = "Name : "+ fi.Name + "\n"
                    + "Time : " + fi.LastWriteTime + "\n"
                    + "Byte : " + fi.Length + "byte\n"
                    + "Directory : " + fi.DirectoryName + "\n";
            }
            catch
            {

            }
        }
        private void ListBox_Network_MyFolderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string str = e.AddedItems[0] as string;
                System.IO.DirectoryInfo fi = new System.IO.DirectoryInfo(str);
                this.TextBlock_Network_MyFileStatus.Text = "Name : " + fi.Name + "\n"
                    + "Time : " + fi.LastWriteTime + "\n"
                    + "Parent : " + fi.Parent + "\n";
            }
            catch
            {

            }
        }

        private void Button_Network_DeleteFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string str = this.ListBox_Network_MyFolderList.SelectedItem as string;
                this.RemoveFolderByPath(str);
            }
            catch
            {

            }
        }
        #endregion

        private void Button_Test_Click(object sender, RoutedEventArgs e)
        {
            CIPCNetwork.Protocols.Message msg = new CIPCNetwork.Protocols.Message();
            msg.Header = CIPCNetwork.Protocols.Message.HeaderType.GetFileList;
            msg.Data = "asdasdasdasd";
            CIPCNetwork.Protocols.Message msg2 = CIPCNetwork.Protocols.Message.SetFromString(msg.ToString());
            myparts.myDialog mydialog = new myparts.myDialog(msg2.ToString());
            mydialog.ShowDialog();
            
        }

        

        

        

        
    }
}
