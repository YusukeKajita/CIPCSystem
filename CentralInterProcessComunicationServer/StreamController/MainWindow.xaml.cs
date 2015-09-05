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
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        #region fields

        public List<StreamController.StreamWindow> List_SW;
        public ListProvider<StreamController.StreamWindow> ListProvider_SW;
        private DispatcherTimer dt;

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            
            this.MouseLeftButtonDown += (sender, e) => this.DragMove();
            this.Closing += MainWindow_Closing;
            this.List_SW = new List<StreamWindow>();
            this.ListProvider_SW = new ListProvider<StreamWindow>(this.List_SW, this.ListBox_SW);
            this.ProcessName.Text = "行程名：" + System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            this.dt = new DispatcherTimer(DispatcherPriority.Normal);
            this.dt.Interval = new TimeSpan(0, 0, 1);
            this.dt.Tick += new EventHandler(dispatcherTimer_Tick);
            this.dt.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                this.CheckStreamWindows();
                this.LIST_UPDATE();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + sender.ToString());
            }
        }


        #region UI EVENT
        private void restart_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Thread.Sleep(300);
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
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

                #region StreamClientSetting

                StreamClient SC = new StreamClient();
                SC.name = this.textbox_clientname.Text;
                SC.myport = int.Parse(this.textbox_clientmyport.Text);
                SC.serverIP = this.remoteIP_1.Text + "." + this.remoteIP_2.Text + "." + this.remoteIP_3.Text + "." + this.remoteIP_4.Text;
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
                SC.filename = this.textbox_filename.Text;
                #endregion

                StreamWindow sw = new StreamWindow(this, SC);
                sw.Show();
                this.List_SW.Add(sw);
                this.textbox_clientmyport.Text = (int.Parse(this.textbox_clientmyport.Text) + 1).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private bool CheckSWClosed(StreamWindow obj)
        {
            if (obj.IsClosed)
            {
                return true;
            }
            return false;
        }
        #endregion

        private void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (radiobutton_sender.IsChecked == true)
                {
                    this.openfiledialog();
                }
                else if (radiobutton_receiver.IsChecked == true)
                {
                    this.savefiledialog();
                }
                else
                {
                    throw new Exception("接続モードを選択してください．");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

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
                this.textbox_filename.Text = dialog.FileName;
            }
        }

        private void CheckStreamWindows()
        {
            while (this.List_SW.Exists(CheckSWClosed))
            {
                for (int i = 0; i < this.List_SW.Count; i++)
                {
                    if (this.List_SW[i].IsClosed)
                    {
                        this.List_SW.Remove(this.List_SW[i]);
                        break;
                    }
                }
            }
        }

        private void LIST_UPDATE()
        {
            try
            {
                //this.ListProvider_SW.Refresh();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        
    }


}
