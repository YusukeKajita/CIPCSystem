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
using System.Threading;
using System.Windows.Threading;
using CIPC_CS;

namespace StreamController
{
    using fm = System.Windows.Forms;
    using System.Windows.Forms;
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        #region fields

        public List<StreamController.StreamWindow> List_SW;
        public StreamWindowListProvider ListProvider_SW;
        private DispatcherTimer dt;
        private int AutoFileNameNumber;
        private System.Diagnostics.Stopwatch stopwatch;
        public System.Diagnostics.Stopwatch Stopwatch
        {
            get
            {
                return this.stopwatch;
            }
        }

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            this.AutoFileNameNumber = 0;
            this.MouseLeftButtonDown += (sender, e) => this.DragMove();
            this.Closing += MainWindow_Closing;
            this.List_SW = new List<StreamWindow>();
            this.ListProvider_SW = new StreamWindowListProvider(this.List_SW, this.ListBox_SW);
            this.ProcessName.Text = "行程名：" + System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            this.stopwatch = new System.Diagnostics.Stopwatch();
            this.stopwatch.Start();
            this.dt = new DispatcherTimer(DispatcherPriority.Normal);
            this.dt.Interval = new TimeSpan(0, 0, 1);
            this.dt.Tick += new EventHandler(dispatcherTimer_Tick);
            this.dt.Start();

            this.LoadApplicationState();
            this.Button_NameUpdate_Click(this, new RoutedEventArgs());
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                this.CheckStreamWindows();
                this.LIST_UPDATE();
                this.FolderListUpdate();
                this.TextBlock_TimeStanp.Text = this.stopwatch.ElapsedMilliseconds.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FolderListUpdate()
        {
            if (this.textbox_DirectoryName.Text == "")
            {
                return;
            }
            System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(this.textbox_DirectoryName.Text);
            if (!directory.Exists)
            {
                return;
            }
            this.ListBox_FileList.ItemsSource = directory.GetFiles();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                #region 終了操作 すべてのストリームウィンドウを終了させます
                foreach (var p in this.List_SW)
                {

                    p.Close();
                }
                #endregion
                this.SaveApplicationState();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + sender.ToString());
            }
        }
        private void SaveApplicationState()
        {
            Properties.Settings.Default.FileName = this.textbox_filename.Text;
            Properties.Settings.Default.DirectoryName = this.textbox_DirectoryName.Text;
            Properties.Settings.Default.fps = this.textbox_fps.Text;
            Properties.Settings.Default.remotePort = int.Parse(this.textbox_serverport.Text);
            Properties.Settings.Default.remoteIP = this.remoteIP.Text;
            Properties.Settings.Default.myName = this.textbox_clientname.Text;
            Properties.Settings.Default.myPort = int.Parse(this.textbox_clientmyport.Text);
            Properties.Settings.Default.Save();

        }
        private void LoadApplicationState()
        {
            this.textbox_filename.Text = Properties.Settings.Default.FileName;
            this.textbox_DirectoryName.Text = Properties.Settings.Default.DirectoryName;
            this.textbox_fps.Text = Properties.Settings.Default.fps;
            this.textbox_serverport.Text = Properties.Settings.Default.remotePort.ToString();
            this.remoteIP.Text = Properties.Settings.Default.remoteIP;
            this.textbox_clientname.Text = Properties.Settings.Default.myName;
            this.textbox_clientmyport.Text = Properties.Settings.Default.myPort.ToString();

        }

        #region UI EVENT
        private void restart_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Thread.Sleep(300);
            System.Diagnostics.Process.Start(System.Windows.Application.ResourceAssembly.Location);
        }

        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void StreamWindowLunchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.CheckStreamWindows();
                if (this.TextBox_FinalFileName.Text == "")
                {
                    this.Button_NameUpdate_Click(sender, e);
                }
                #region StreamClientSetting

                StreamClient SC = new StreamClient();
                SC.name = this.textbox_clientname.Text;
                SC.myport = int.Parse(this.textbox_clientmyport.Text);
                SC.serverIP = this.remoteIP.Text;
                SC.serverport = int.Parse(this.textbox_serverport.Text);
                if (this.radiobutton_sender.IsChecked == true)
                {
                    SC.mode = MODE.Sender;
                }
                else if (this.radiobutton_receiver.IsChecked == true)
                {
                    SC.mode = MODE.Receiver;
                }
                else
                {
                    MessageBox.Show("error." + this.ToString());
                }
                SC.fps = int.Parse(this.textbox_fps.Text);
                SC.filename = this.TextBox_FinalFileName.Text;
                #endregion

                StreamWindow sw = new StreamWindow(this, SC);
                this.List_SW.Add(sw);
                sw.Show();
                this.ListProvider_SW.Refresh();
                this.textbox_clientmyport.Text = (int.Parse(this.textbox_clientmyport.Text) + 1).ToString();
                this.AutoFileNameNumber++;
                this.TextBlock_AutoNumber.Text = this.AutoFileNameNumber.ToString();
                this.Button_NameUpdate_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        private void savefiledialog()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = "default.scd"; // Default file name
            dialog.DefaultExt = ".scd"; // Default file extension
            dialog.Filter = "StreamControllerData (.scd)|*.scd"; // Filter files by extension
            dialog.CheckFileExists = false;
            dialog.CheckPathExists = false;
            var result = dialog.ShowDialog();
            if (result == true)
            {
                this.textbox_filename.Text = dialog.FileName;
            }
        }

        private void openfiledialog()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "default.scd"; // Default file name
            dialog.DefaultExt = ".scd"; // Default file extension
            dialog.Filter = "StreamControllerData (.scd)|*.scd"; // Filter files by extension
            dialog.CheckFileExists = false;
            dialog.CheckPathExists = false;
            var result = dialog.ShowDialog();
            if (result == true)
            {
                this.textbox_DirectoryName.Text = System.IO.Path.GetDirectoryName(dialog.FileName);
                this.textbox_filename.Text = System.IO.Path.GetFileName(dialog.FileName).Replace(System.IO.Path.GetExtension(dialog.FileName), "");
                this.CheckBox_AutoSetting_Unchecked(this, new RoutedEventArgs());
                this.CheckBox_AutoSetting.IsChecked = false;
            }
        }

        private void CheckStreamWindows()
        {
            this.List_SW.RemoveAll(p => p.IsClosed);
        }

        private void LIST_UPDATE()
        {
            try
            {
                this.ListProvider_SW.Refresh();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Button_ALL_REC_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.UpdateStatusCaption(this.List_SW.FindAll(k => k.SC.mode == MODE.Receiver && !k.IsRecStarted).Count.ToString() + "個のクライアントの録画を開始します");
                foreach (var p in this.List_SW)
                {
                    if (p.SC.mode == MODE.Receiver)
                    {
                        p.Button_RecStart_Click(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_ALL_STOP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.UpdateStatusCaption(this.List_SW.FindAll(k => k.SC.mode == MODE.Receiver && (k.IsRecStarted || k.IsSendStarted)).Count.ToString() + "個のクライアントの録画・再生を停止します");
                foreach (var p in this.List_SW)
                {
                    if (p.IsRecStarted || p.IsSendStarted)
                    {
                        p.Button_Stop_Click(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_ALL_CLOSE_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.UpdateStatusCaption(this.List_SW.FindAll(k => (!k.IsRecStarted || !k.IsSendStarted)).Count.ToString() + "個のクライアントを終了します");
                foreach (var p in this.List_SW)
                {
                    if (!p.IsRecStarted || !p.IsSendStarted)
                    {
                        p.Button_Click(sender, e);
                    }
                }
                this.LIST_UPDATE();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_ALL_START_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.UpdateStatusCaption(this.List_SW.FindAll(k => k.SC.mode == MODE.Sender && !k.IsSendStarted).Count.ToString() + "個のクライアントの再生を開始します");
                foreach (var p in this.List_SW)
                {
                    if (p.SC.mode == MODE.Receiver)
                    {
                        p.Button_RecStart_Click(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_SelectDirectory_Click(object sender, RoutedEventArgs e)
        {
            if (this.radiobutton_receiver.IsChecked == true)
            {
                FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
                folderBrowserDialog1.Description = "ここに説明を書いてください";
                folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.MyComputer;
                folderBrowserDialog1.SelectedPath = this.textbox_DirectoryName.Text;
                folderBrowserDialog1.ShowNewFolderButton = true;
                if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.textbox_DirectoryName.Text = folderBrowserDialog1.SelectedPath;
                    this.UpdateStatusCaption(folderBrowserDialog1.SelectedPath);
                }
                folderBrowserDialog1.Dispose();
            }
            else
            {
                this.openfiledialog();
            }
        }

        private void CheckBox_AutoSetting_Checked(object sender, RoutedEventArgs e)
        {
            this.TextBlock_AutoNumber.Text = this.AutoFileNameNumber.ToString();
            this.UpdateStatusCaption("Change Auto Mode check");
        }

        private void CheckBox_AutoSetting_Unchecked(object sender, RoutedEventArgs e)
        {
            this.UpdateStatusCaption("Change Auto Mode uncheck");
            this.TextBlock_AutoNumber.Text = "";
        }

        private void Button_NameUpdate_Click(object sender, RoutedEventArgs e)
        {
            this.TextBox_FinalFileName.Text = this.textbox_DirectoryName.Text + @"\" + this.textbox_filename.Text + this.TextBlock_AutoNumber.Text + ".scd";
            this.UpdateStatusCaption(this.TextBox_FinalFileName.Text);
        }

        private void UpdateStatusCaption(string str)
        {
            if (this.TextBlock_StatusCaption != null)
            {
                this.TextBlock_StatusCaption.Text = str;
            }
        }

        private void Button_ResetAutoNum_Click(object sender, RoutedEventArgs e)
        {
            this.AutoFileNameNumber = 0;
            this.TextBlock_AutoNumber.Text = this.AutoFileNameNumber.ToString();
            this.UpdateStatusCaption("Reset Auto Number");
        }

        private void Button_OpenExproler_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("EXPLORER.EXE", this.textbox_DirectoryName.Text);
        }

        private void Button_Reset_Stamp_Click(object sender, RoutedEventArgs e)
        {
            this.stopwatch.Restart();
            try
            {
                this.UpdateStatusCaption(this.List_SW.FindAll(k => k.SC.mode == MODE.Receiver && (k.IsRecStarted || k.IsSendStarted)).Count.ToString() + "個のクライアントの録画・再生を停止し，再接続をします．");
                foreach (var p in this.List_SW)
                {
                    if (p.IsRecStarted || p.IsSendStarted)
                    {
                        p.Button_Stop_Click(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private CIPC_CS.CLIENT.CLIENT CipcSyncClient;
        private void Button_CIPC_Connect_Click(object sender, RoutedEventArgs e)
        {
            if (this.CipcSyncClient != null)
            {
                return;
            }
            this.CipcSyncClient = new CIPC_CS.CLIENT.CLIENT(int.Parse(this.TextBox_ControlCIPC_myPort.Text), this.TextBox_ControlCIPC_remoteIP.Text, int.Parse(this.TextBox_ControlCIPC_remotePort.Text), "StreamController_SYNCREC", 30);
            this.CipcSyncClient.Setup(CIPC_CS.CLIENT.MODE.Receiver);
            this.CipcSyncClient.DataReceived += CipcSyncClient_DataReceived;
        }


        private void Button_ALL_RESET_Click(object sender, RoutedEventArgs e)
        {
            this.Button_ALL_STOP_Click(sender, e);
            this.Button_ALL_STOP_Click(sender, e);
        }


        void CipcSyncClient_DataReceived(object sender, byte[] e)
        {
            UDP_PACKETS_CODER.UDP_PACKETS_DECODER dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
            dec.Source = e;
            var str = dec.get_string();
            switch(str){
                case "START":
                    this.Button_ALL_REC.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.Button_ALL_REC_Click(this,new RoutedEventArgs());
                    }));
                    break;
                case "STOP":
                    this.Button_ALL_STOP.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.Button_ALL_STOP_Click(this, new RoutedEventArgs());
                    }));
                    break;
                default:
                    Console.WriteLine("Invalid operation.");
                    break;
            }
            
        }

        private void Button_CIPC_Close_Click(object sender, RoutedEventArgs e)
        {
            this.CipcSyncClient.Close();
            this.CipcSyncClient = null;
        }


    }


}
