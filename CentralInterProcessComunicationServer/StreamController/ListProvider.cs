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

    public class StreamWindowListProvider
    {
        public List<StreamWindow> itemsorce { get; protected set; }
        public List<string> windowstates;
        public ListBox listbox { set; get; }
        public StreamWindowListProvider(List<StreamWindow> items, ListBox listbox)
        {
            this.itemsorce = items;
            this.windowstates = new List<string>();
            foreach (var p in this.itemsorce)
            {
                windowstates.Add(p.SC.name + " " + p.SC.mode + " " + p.SC.myport);
            }
            this.listbox = listbox;
            this.listbox.ItemsSource = this.windowstates;
            this.listbox.Items.Refresh();
        }
        public void Refresh()
        {
            windowstates.Clear();
            foreach (var p in this.itemsorce)
            {
                windowstates.Add(p.SC.name + " " + p.SC.mode + " " + p.SC.myport);
            }
            this.listbox.Items.Refresh();
        }
    }
}
