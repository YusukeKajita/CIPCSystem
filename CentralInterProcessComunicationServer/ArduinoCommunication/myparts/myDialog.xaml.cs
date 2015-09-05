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

namespace myparts
{
    /// <summary>
    /// myDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class myDialog : Window
    {
        public myDialog()
        {
            InitializeComponent();
            this.Init_Events();
        }

        public myDialog(string str)
        {
            InitializeComponent();
            this.TEXTBLOCK.Text = str;
            this.Init_Events();
        }

        private void Init_Events()
        {
            this.MouseLeftButtonDown += (sender, e) => this.DragMove();
        }

        private void button_yes_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void button_no_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
