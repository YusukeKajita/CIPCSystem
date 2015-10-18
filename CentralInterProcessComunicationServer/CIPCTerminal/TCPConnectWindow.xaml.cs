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

namespace CIPCTerminal
{
    /// <summary>
    /// TCPConnectWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class TCPConnectWindow : Window
    {
        public ConnectionSetting connectionsetting { set; get; }
        public MainWindow mainwindow { set; get; }

        public bool IsConnectSetuped { set; get; }
        public string pushedtime { set; get; }
        public TCPConnectWindow()
        {
            InitializeComponent();
            this.InitProperty();
            this.Init_Classes();
            this.Init_Events();
            this.Init_DataContext();
        }

        private void InitProperty()
        {
            this.IsConnectSetuped = false;
        }

        private void Init_DataContext()
        {
            this.TabItem_ConnectionSetting.DataContext = this.connectionsetting;
            this.connectionsetting.readsetting();
        }

        private void Init_Classes()
        {
            this.connectionsetting = new ConnectionSetting();
        }

        private void Init_Events()
        {
            this.MouseLeftButtonDown += (s, e) => this.DragMove();
            this.Closing += TCPConnectWindow_Closing;
        }

        void TCPConnectWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.mainwindow.Hide_tcpconnectwindow();
        }

        private void Button_SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            this.connectionsetting.savesetting();
        }

        private void Button_LoadSettings_Click(object sender, RoutedEventArgs e)
        {
            this.connectionsetting.readsetting();
        }

        private void Button_InitSettings_Click(object sender, RoutedEventArgs e)
        {
            this.connectionsetting.initsetting();
        }

        private void Button_Connect_Click(object sender, RoutedEventArgs e)
        {
            myparts.myDialog mydialog = new myparts.myDialog("接続を開始します");
            if (mydialog.ShowDialog() != true)
            {
                return;
            }
            this.StackPanel_ConnectSetting.IsEnabled = false;
            this.IsConnectSetuped = true;
            this.pushedtime = System.DateTime.Now.ToString();
            this.TabControl_CIPCConnection.SelectedIndex = 1;
            this.mainwindow.serverconnection.SetupUDP();
        }

        private void Button_StopConnect_Click(object sender, RoutedEventArgs e)
        {
            myparts.myDialog mydialog = new myparts.myDialog("接続を停止します");
            if (mydialog.ShowDialog() != true)
            {
                return;
            }
            this.StackPanel_ConnectSetting.IsEnabled = true;
            this.IsConnectSetuped = false;
            this.TabControl_CIPCConnection.SelectedIndex = 0;
            this.mainwindow.serverconnection.closeUDP();
        }
    }
}
