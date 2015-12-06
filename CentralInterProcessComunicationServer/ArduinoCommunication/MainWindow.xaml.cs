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

namespace ArduinoCommunication
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields
        public System.IO.Ports.SerialPort serialport { set; get; }
        public Task maintask;
        public CancellationTokenSource CTS;
        private FPSAdjuster.FPSAdjuster fps;

        #region PPMS
        public string readstring;
        public CIPC_CS.CLIENT.CLIENT CIPCClient;
        #endregion

        #endregion

        public MainWindow()
        {
            this.InitializeComponent();

            this.Init_Events();

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


        private void Change_StatusBar_TextBlock(string text)
        {
            try
            {
                this.TextBlock_StatusCaption.Text = text;
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

        #region othersevents
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
                    this.CloseAll();
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
        #endregion

        #region Maintask
        private void MainTask()
        {
            try
            {

                while (!this.CTS.IsCancellationRequested)
                {
                    fps.Adjust();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        
        #region PPMS
        private void Button_PPMS_Serial_Connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                myparts.myDialog dlg = new myparts.myDialog("シリアルポートに接続します");
                if (dlg.ShowDialog() != true)
                {
                    return;
                }

                this.serialport = new System.IO.Ports.SerialPort("COM" + this.TextBox_PPMS_COMPORT.Text, int.Parse(this.TextBox_PPMS_BaudRate.Text), System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
                this.serialport.Encoding = Encoding.ASCII;
               
                this.serialport.Open();

                this.fps = new FPSAdjuster.FPSAdjuster();
                fps.Fps = int.Parse( this.TextBox_PPMS_FPS.Text);
                fps.Start();

                this.CTS = new CancellationTokenSource();
                this.maintask = new Task(new Action(this.MainTask_PPMS), this.CTS.Token);
                this.maintask.Start();
            }
            catch (Exception ex)
            {
                myparts.myDialog dlg = new myparts.myDialog(ex.Message);
                dlg.ShowDialog();
            }
        }

        private void Button_PPMS_Serial_Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                myparts.myDialog dlg = new myparts.myDialog("シリアルポートを切断します");
                if (dlg.ShowDialog() != true)
                {
                    return;
                }
                this.CloseAll();
            }
            catch (Exception ex)
            {
                myparts.myDialog dlg = new myparts.myDialog(ex.Message);
                dlg.ShowDialog();
            }
        }

        private void MainTask_PPMS()
        {
            try
            {

                while (!this.CTS.IsCancellationRequested)
                {
                    if (this.serialport.IsOpen)
                    {
                        try
                        {
                            this.readstring = this.serialport.ReadLine();
                            byte[] data = Encoding.ASCII.GetBytes(this.readstring);
                            if (this.CIPCClient != null)
                            {
                                this.CIPCClient.Update(ref data);
                            }
                        }
                        catch (Exception ex)
                        {
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                this.TextBlock_StatusCaption.Text = ex.Message;
                            }));
                        }
                    }
                    fps.Adjust();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region Close Parts
        private void CloseAll()
        {
            this.CloseCTS();
            this.CloseTask();
            this.CloseComport();
        }

        private void CloseCTS()
        {
            if (this.CTS != null)
            {
                this.CTS.Cancel();
                Thread.Sleep(100);
                this.CTS.Dispose();
                this.CTS = null;

            }
        }

        private void CloseTask()
        {
            if (this.maintask != null)
            {
                this.maintask.Dispose();
                this.maintask = null;
            }
        }

        private void CloseComport()
        {
            if (this.serialport != null)
            {
                this.serialport.Close();
                this.serialport = null;
            }
        }

        private void CIPCClose()
        {
            if (this.CIPCClient != null)
            {
                this.CIPCClient.Close();
                this.CIPCClient = null;
            }
        }

        private void CIPC_lunchbutton_Click(object sender, RoutedEventArgs e)
        {
            this.CIPCClose();
            this.CIPCClient = new CIPC_CS.CLIENT.CLIENT(int.Parse(this.TextBox_myPort.Text), this.TextBox_remoteIP.Text, int.Parse(this.TextBox_remotePort.Text), "PPMS", int.Parse(this.TextBox_fps.Text));
            this.CIPCClient.Setup(CIPC_CS.CLIENT.MODE.Sender);
        }

        private void CIPC_closebutton_Click(object sender, RoutedEventArgs e)
        {
            this.CIPCClient.Close();
        }
        #endregion

        #region Servo Control
        private void Button_SC_Serial_Connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                myparts.myDialog dlg = new myparts.myDialog("シリアルポートに接続します");
                if (dlg.ShowDialog() != true)
                {
                    return;
                }

                this.serialport = new System.IO.Ports.SerialPort("COM" + this.TextBox_SC_COMPORT.Text, int.Parse(this.TextBox_SC_BaudRate.Text), System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
                this.serialport.Encoding = Encoding.ASCII;
                this.serialport.Open();

                this.fps = new FPSAdjuster.FPSAdjuster();
                fps.Fps = 30;
                fps.Start();
            }
            catch (Exception ex)
            {
                myparts.myDialog dlg = new myparts.myDialog(ex.Message);
                dlg.ShowDialog();
            }
        }

        private void Button_SC_Serial_Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                myparts.myDialog dlg = new myparts.myDialog("シリアルポートを切断します");
                if (dlg.ShowDialog() != true)
                {
                    return;
                }
                this.CloseAll();
            }
            catch (Exception ex)
            {
                myparts.myDialog dlg = new myparts.myDialog(ex.Message);
                dlg.ShowDialog();
            }
        }

        private void Slider_Deg1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                byte[] data;
                data = new byte[2];
                data[0] = (byte)this.Slider_Deg1.Value;
                data[1] = (byte)this.Slider_Deg2.Value;
                this.serialport.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region MKCL
        private void Button_MKCL_Serial_Connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                myparts.myDialog dlg = new myparts.myDialog("シリアルポートに接続します");
                if (dlg.ShowDialog() != true)
                {
                    return;
                }
                this.serialport = new System.IO.Ports.SerialPort("COM" + this.TextBox_MKCL_COMPORT.Text, int.Parse(this.TextBox_MKCL_BaudRate.Text), System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
                this.serialport.Encoding = Encoding.ASCII;
                this.serialport.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Button_MKCL_Serial_Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                myparts.myDialog dlg = new myparts.myDialog("シリアルポートを切断します");
                if (dlg.ShowDialog() != true)
                {
                    return;
                }
                this.CloseComport();
            }
            catch (Exception ex)
            {
                myparts.myDialog dlg = new myparts.myDialog(ex.Message);
                dlg.ShowDialog();
            }
        }

        
        #endregion

        private void RadioButton_MKCL_LED_Checkedchange(object sender, RoutedEventArgs e)
        {
            byte[] senddata = new byte[3];
            senddata[0] = (this.RadioButton_MKCL_LED1.IsChecked == true) ? (byte)1 : (byte)0;
            senddata[1] = (this.RadioButton_MKCL_LED2.IsChecked == true) ? (byte)1 : (byte)0;
            senddata[2] = (this.RadioButton_MKCL_LED3.IsChecked == true) ? (byte)1 : (byte)0;
            this.serialport.Write(senddata,0,senddata.Length);
        }
    }
}
