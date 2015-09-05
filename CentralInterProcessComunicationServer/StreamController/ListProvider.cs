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

namespace StreamController
{
    public class ListProvider<T>
    {
        public List<T> itemsorce { get; protected set; }
        public ListBox listbox { set; get; }
        public ListProvider(List<T> items, ListBox listbox)
        {
            this.itemsorce = items;
            this.listbox = listbox;
            this.listbox.ItemsSource = this.itemsorce;
            this.listbox.Items.Refresh();
        }
        public void Refresh()
        {
            this.listbox.Items.Refresh();
        }
    }
}
