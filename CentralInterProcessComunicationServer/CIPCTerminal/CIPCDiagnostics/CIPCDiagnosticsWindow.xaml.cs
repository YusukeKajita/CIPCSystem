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
using System.Windows.Shapes;
using System.Diagnostics;

namespace CIPCTerminal.CIPCDiagnostics
{
    /// <summary>
    /// CIPCDiagnosticsWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class CIPCDiagnosticsWindow : Window
    {
        public enum Handle
        {
            Start,
            Close,
            Restart,
            Non
        }
        private Handle _handle;

        public MainWindow mainwindow { set; get; }

        public Handle handle
        {
            private set
            {
                this._handle = value;
            }
            get
            {
                Handle _handle =this._handle;
                this._handle = Handle.Non;
                return _handle;
            }
        }

        public CIPCDiagnosticsWindow()
        {
            InitializeComponent();
            this.Init_Events();
            this.Init_Field();
            this.TabItem_local.Header = System.Net.Dns.GetHostName();
            //this.TabItem_remote.Header = System.Net.Dns.GetHostEntry("127.0.0.1").HostName;
        }

        private void Init_Field()
        {
            this.handle = Handle.Non;
        }

        private void Init_Events()
        {
            this.MouseLeftButtonDown += (sender, e) => this.DragMove();
            this.Closing += CIPCDiagnosticsWindow_Closing;
        }

        void CIPCDiagnosticsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.CIPCServer_filepath = this.TextBox_Local_CIPCServerPath.Text;
            Properties.Settings.Default.Save();
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.mainwindow.Hide_cipcdiagnosticswindow();
        }

        private void Button_Local_CIPCServerPathSetting_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "CIPCServer (CentralInterProcessCommunicationServer.exe)|CentralInterProcessCommunicationServer.exe";
            dialog.Title = "CIPCServerの場所を選択してください";
            if (dialog.ShowDialog() == true)
            {
                this.TextBox_Local_CIPCServerPath.Text = dialog.FileName;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.TextBox_Local_CIPCServerPath.Text = Properties.Settings.Default.CIPCServer_filepath;
        }

        private void Widnow_Unloaded(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.CIPCServer_filepath = this.TextBox_Local_CIPCServerPath.Text;
            Properties.Settings.Default.Save();
        }

        private void Button_Local_Start_Click(object sender, RoutedEventArgs e)
        {
            this.handle = Handle.Start;
        }

        private void Button_Local_Close_Click(object sender, RoutedEventArgs e)
        {
            this.handle = Handle.Close;
        }

        private void Button_Local_Restart_Click(object sender, RoutedEventArgs e)
        {
            this.handle = Handle.Restart;
        }

        private void Button_Remote_GetProcess_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.TabItem_remote.Header = System.Net.Dns.GetHostEntry(this.TextBox_Remote_RemoteIP.Text).HostName;
                //System.Diagnostics.Process[] CIPCS = System.Diagnostics.Process.GetProcessesByName("CentralInterProcessCommunicationServer", this.TextBox_Remote_RemoteIP.Text);
                //if (CIPCS.Length != 0)
                //{
                //    this.TextBlock_Remote_CIPCState.Text = "動作中";
                //    this.TextBlock_Remote_CIPCTime.Text = (DateTime.Now - CIPCS[0].StartTime).ToString();
                //    this.TextBlock_Remote_CIPCRespond.Text = CIPCS[0].Responding.ToString();
                //    this.TextBlock_Remote_CIPCID.Text = CIPCS[0].Id.ToString();
                //}
                //else
                //{
                //    this.TextBlock_Remote_CIPCState.Text = "未起動";
                //    this.TextBlock_Remote_CIPCTime.Text = "";
                //    this.TextBlock_Remote_CIPCRespond.Text = "";
                //    this.TextBlock_Remote_CIPCID.Text = "";
                //}
                //System.Diagnostics.Process[] CIPCT = System.Diagnostics.Process.GetProcessesByName("CIPCTerminal", this.TextBox_Remote_RemoteIP.Text);
                //if (CIPCT.Length != 0)
                //{
                //    this.TextBlock_Remote_CIPCTerminalState.Text = "動作中";
                //    this.TextBlock_Remote_CIPCTerminalTime.Text = (DateTime.Now - CIPCT[0].StartTime).ToString();
                //    this.TextBlock_Remote_CIPCTerminalRespond.Text = CIPCT[0].Responding.ToString();
                //    this.TextBlock_Remote_CIPCTerminalID.Text = CIPCT[0].Id.ToString();
                //}
                //else
                //{
                //    this.TextBlock_Remote_CIPCTerminalState.Text = "未起動";
                //    this.TextBlock_Remote_CIPCTerminalTime.Text = "";
                //    this.TextBlock_Remote_CIPCTerminalRespond.Text = "";
                //    this.TextBlock_Remote_CIPCTerminalID.Text = "";
                //}
            }
            catch (Exception ex)
            {
                while (true)
                {
                    myparts.myDialog mydialog = new myparts.myDialog(ex.Message);
                    if (mydialog.ShowDialog() == true)
                    {
                        break;
                    }
                }
            }
        }
    }
}
