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
        public System.IO.Ports.SerialPort serialport { set; get; }
        public Task maintask;
        public CancellationTokenSource CTS;

        public MainWindow()
        {
            this.InitializeComponent();

            this.Init_Classes();

            this.Init_ButtonClickEvents();

            this.Init_Events();

            this.CreateChildWindows();
        }

        private void Init_Classes()
        {
            this.serialport = new System.IO.Ports.SerialPort("COM10", 9600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
            this.serialport.Encoding = Encoding.ASCII;
            this.serialport.Open();

            this.CTS = new CancellationTokenSource();
            this.maintask = new Task( new Action(MainTask),this.CTS.Token);
            this.maintask.Start();
        }

        private void MainTask()
        {
            try
            {
                FPSAdjuster.FPSAdjuster fps = new FPSAdjuster.FPSAdjuster();
                fps.Fps = 30;
                fps.Start();
                int DT = DateTime.Now.Millisecond;
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
        /// <summary>
        /// 子ウィンドウの生成
        /// </summary>
        private void CreateChildWindows()
        {

        }
        #region buttonclickevents
        /// <summary>
        /// ボタンをクリックした時のイベントの登録
        /// </summary>
        private void Init_ButtonClickEvents()
        {
            this.Button_Close.Click += Button_Close_Click;
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
                    this.serialport.Close();
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

        private void Slider_Deg1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            byte[] data;
            data = new byte[1];
            data[0] = (byte)this.Slider_Deg1.Value;
            this.serialport.Write(data,0,1);
        }
        
    }
}
