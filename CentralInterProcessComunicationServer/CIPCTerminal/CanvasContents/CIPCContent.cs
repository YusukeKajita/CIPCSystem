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

namespace CIPCTerminal.CanvasContents
{
    public class CIPCConnectionPort : Grid
    {
        public Ellipse ellipse { set; get; }
        public int id { set; get; }

        public CIPCConnectionPort(int id)
        {
            this.id = id;
            this.ellipse = new Ellipse();
            this.ellipse.Height = 20;
            this.ellipse.Width = 20;
            this.ellipse.StrokeThickness = 5;
            this.ellipse.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.ellipse.Fill = new SolidColorBrush(Color.FromArgb(150, 100, 100, 0));
            this.Children.Add(this.ellipse);

            this.Height = 20;
            this.Width = 20;
            this.ToolTip = "id:" + id.ToString();
        }
    }

    public class CIPCContent : Grid
    {
        protected Rectangle rect { set; get; }
        protected TextBlock tb { set; get; }

        public List<CIPCConnectionPort> List_Port { set; get; }
        protected StackPanel stackpanel1 { set; get; }
        protected StackPanel stackpanel2 { set; get; }

        public MainWindow mainwindow { set; get; }
        public CIPCContent()
        {
            this.rect=new Rectangle();
            this.Children.Add(rect);
            this.tb = new TextBlock();
            this.tb.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            this.tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            this.Children.Add(tb);

            this.stackpanel1 = new StackPanel();
            this.stackpanel2 = new StackPanel();
            this.stackpanel2.Orientation = Orientation.Horizontal;
            this.stackpanel2.Children.Add(this.stackpanel1);
            this.Children.Add(stackpanel2);
            this.List_Port = new List<CIPCConnectionPort>();
            this.MouseDown += CIPCContent_MouseDown;
            this.MouseMove += CIPCContent_MouseMove;
            this.MouseUp += CIPCContent_MouseUp;
            this.MouseLeave += CIPCContent_MouseLeave;
            

        }

        void CIPCContent_MouseLeave(object sender, MouseEventArgs e)
        {
            this.IsDrag = false;
        }

        protected void CIPCContent_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.IsDrag)
            {
                this.SetPosition(this.Left + e.GetPosition(this).X - this.MousePosFront.X, this.Top + e.GetPosition(this).Y - this.MousePosFront.Y);
                if (this.mainwindow != null)
                {
                    this.mainwindow.CreateLocalCanvasContent_Lines();
                }
            }
        }

        void CIPCContent_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                this.IsDrag = false;
            }
        }

        public bool IsDrag { set; get; }
        public Point MousePosFront { set; get; }
        void CIPCContent_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                this.MousePosFront = e.GetPosition(this);
                this.IsDrag = true;
            }
        }

        public double Top { set; get; }
        public double Left { set; get; }
        public double CenterLeft
        {
            set
            {
                this.Left = value - this.Width / 2;
            }
            get
            {
                return Left + this.Width / 2;
            }
        }
        public double CenterTop
        {
            set
            {
                this.Top = value - this.Height / 2;
            }
            get
            {
                return Top + this.Height / 2;
            }
        }

        public void SetPosition(double x, double y)
        {
            this.Top = y;
            this.Left = x;
            this.SetCanvasPos();
        }
        public void SetCenterPosition(double x, double y)
        {
            this.CenterTop = y;
            this.CenterLeft = x;
            this.SetCanvasPos();
        }

        private void SetCanvasPos()
        {
            Canvas.SetLeft(this, this.Left);
            Canvas.SetTop(this, this.Top);
        }
    }

    public class CIPCServer : CIPCContent
    {
        public TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo cipcinfo { set; get; }
        public CIPCServer(TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo cipcinfo)
        {
            this.tb.Text = "CIPCServer" + "\n"
                + cipcinfo.ClientList.Count;

            this.Width = 200;
            this.Height = 150;
            this.cipcinfo = cipcinfo;

            

            this.rect.RadiusX = 5;
            this.rect.RadiusY = 5;
            this.rect.StrokeThickness = 5;
            this.rect.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.rect.Fill = new SolidColorBrush(Color.FromArgb(150,0,100,0));

            for (int i = 0; i < cipcinfo.ClientList.Count; i++)
            {
                this.List_Port.Add(new CIPCConnectionPort(this.cipcinfo.ClientList[i].remotePort));
            }

            for (int i = 0; i < cipcinfo.ClientList.Count; i++)
            {
                this.stackpanel1.Children.Add(this.List_Port[i]);
            }

            Button button = new Button();
            button.Content = " 詳細 ";
            button.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            button.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            button.Margin = new Thickness(10);
            button.Click += button_Click;
            this.Children.Add(button);

            this.ToolTip = "CIPCServer";
            this.ToolTip += "\n中ボタンをドラッグで\n移動します";
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            this.mainwindow.TabControl_Main.SelectedIndex = 1;
        }
    }

    public class CIPCTerminal : CIPCContent
    {
        public CIPCTerminal()
        {
            this.tb.Text = "CIPCTerminal";

            this.Width = 100;
            this.Height = 50;

            this.rect.RadiusX = 5;
            this.rect.RadiusY = 5;
            this.rect.StrokeThickness = 5;
            this.rect.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.rect.Fill = new SolidColorBrush(Color.FromArgb(150, 80, 80, 80));
            this.ToolTip = "CIPCTerminal";
            this.ToolTip += "\n中ボタンをドラッグで\n移動します";
        }

    }

    public class CIPCClient : CIPCContent
    {
        public TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Client client { set; get; }
        public CIPCClient(TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Client client)
        {
            this.tb.Text = client.name + "\n"
                + client.remoteIP + "\n"
                + client.remotePort + "\n"
                + client.FPS + "\n"
                + client.mode;

            this.Width = 150;
            this.Height = 100;
            this.client = client;

            this.rect.RadiusX = 5;
            this.rect.RadiusY = 5;
            this.rect.StrokeThickness = 5;
            this.rect.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.rect.Fill = new SolidColorBrush(Color.FromArgb(150, 100, 0, 0));

            this.List_Port.Add(new CIPCConnectionPort(this.client.remotePort));
            this.stackpanel1.Children.Add(this.List_Port[0]);
            this.ToolTip = client.name;
            this.ToolTip += "\n中ボタンをドラッグで\n移動します";
        }

    }

    public class CIPCServerClientHost : CIPCContent
    {
        public TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Client client { set; get; }
        public CIPCServerClientHost(TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Client client)
        {
            this.tb.Text = client.MyPort.ToString()+" "+ client.mode.ToString()+"\n"
                + client.name+"\n" 
                + client.remotePort.ToString();

            this.Width = 180;
            this.Height = 80;
            this.client = client;

            this.rect.RadiusX = 5;
            this.rect.RadiusY = 5;
            this.rect.StrokeThickness = 5;
            this.rect.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            if (this.client.mode == TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Client.Mode.Sender)
            {
                this.rect.Fill = new SolidColorBrush(Color.FromArgb(150, 0, 0, 100));
            }
            else if (this.client.mode == TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Client.Mode.Receiver)
            {
                this.rect.Fill = new SolidColorBrush(Color.FromArgb(150, 100, 0, 0));
            }

            this.List_Port.Add(new CIPCConnectionPort(this.client.remotePort));
            this.stackpanel1.Children.Add(this.List_Port[0]);
            this.ToolTip = client.name;
            this.ToolTip += "\n中ボタンをドラッグで移動\n右ボタンをドラッグで新しい接続を開始";
            this.MouseDown += CIPCServerClientHost_MouseRightButtonDown;
            this.MouseRightButtonUp += CIPCServerClientHost_MouseUp;
            this.MouseMove -= this.CIPCContent_MouseMove;
            this.MouseMove += this.CIPCServerClientHost_MouseMove;
        }

        void CIPCServerClientHost_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.IsDrag)
            {
                this.SetPosition(this.Left + e.GetPosition(this).X - this.MousePosFront.X, this.Top + e.GetPosition(this).Y - this.MousePosFront.Y);
                if (this.mainwindow != null)
                {
                    this.mainwindow.CreateServerCanvasContent_Lines();
                }
            }
        }

        void CIPCServerClientHost_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.mainwindow.SenderClientHost != this && this.mainwindow.SenderClientHost != null)
                {
                    if (this.client.mode == TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Client.Mode.Receiver)
                    {
                        this.mainwindow.serverconnection.Tcp_Send(new TerminalConnectionSettings.TerminalProtocols.Connect(this.mainwindow.SenderClientHost.client.MyPort, this.client.MyPort));
                    }
                }
            }
            catch(Exception ex)
            {
                
            }
        }

        private void CIPCServerClientHost_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (this.client.mode == TerminalConnectionSettings.ServerProtocols.ReportInfo.CIPCInfo.Client.Mode.Sender)
                {
                    this.mainwindow.SenderClientHost = this;
                    this.mainwindow.IsConnectStartedinUI = true;
                }
            }
        }

    }
}
