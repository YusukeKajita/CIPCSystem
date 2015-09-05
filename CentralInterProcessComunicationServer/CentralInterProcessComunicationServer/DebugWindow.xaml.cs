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

namespace CentralInterProcessCommunicationServer
{
    /// <summary>
    /// DebugWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class DebugWindow : Window
    {
        public string DebugLog
        {
            set
            {
                this.Dispatcher.BeginInvoke(new Action(() => 
                {
                    this.TextBlock_log.Text += value + "\n";
                    if (this.TextBlock_log.Text.Length > 5000)
                    {
                        this.TextBlock_log.Text = this.TextBlock_log.Text.Remove(0, this.TextBlock_log.Text.Length - 5000);
                    }
                }));
                
            }

            get
            {
                return this.TextBlock_log.Text;
            }
        }
        public DebugWindow()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += (sender, e) => this.DragMove();
        }

        private void Button_logdelete_Click(object sender, RoutedEventArgs e)
        {
            this.TextBlock_log.Text = "";
        }
    }
}
