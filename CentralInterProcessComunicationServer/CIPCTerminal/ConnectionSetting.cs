using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace CIPCTerminal
{
    public class ConnectionSetting : INotifyPropertyChanged
    {
        public System.Net.IPEndPoint ConnectionIPEndPoiint
        {
            get
            {
                try { 
                    if(this.IsConnectionLocal)
                    {
                        return new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, int.Parse(this.localport));
                    }
                    else
                    {
                        string[] ipaddress = this.remoteIP.Split('.');
                        if (ipaddress.Length == 4)
                        {
                            byte[] ipaddressByte = new byte[4];
                            for (int i = 0; i < 4; i++)
                            {
                                ipaddressByte[i] = byte.Parse(ipaddress[i]);
                            }
                            return new System.Net.IPEndPoint(new System.Net.IPAddress(ipaddressByte), int.Parse(this.localport));
                        }
                        else
                        {
                            throw new Exception("IPアドレスの書式が間違っています。");
                        }
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                
            }
        }

        public bool IsConnectionRemote { set; get; }
        public bool IsConnectionLocal 
        {
            set 
            {
                this.IsConnectionRemote = !value; 
            }
            get 
            {
                return !this.IsConnectionRemote; 
            }
        }
        public bool IsConnectionAuto { set; get; }
        public bool IsConnectionHand
        {
            set
            {
                this.IsConnectionAuto = !value;
            }
            get
            {
                return !this.IsConnectionAuto;
            }
        }
        public string localport { set; get; }
        public string remoteIP { set; get; }
        public string remoteport { set; get; }

        public void readsetting()
        {
            this.localport = Properties.Settings.Default.connect_local_Port;
            this.remoteIP = Properties.Settings.Default.connect_remote_IPadress;
            this.remoteport = Properties.Settings.Default.connect_remote_Port;
            this.IsConnectionAuto = Properties.Settings.Default.connect_mode_auto;
            this.IsConnectionRemote = Properties.Settings.Default.connect_mode_host;
            this.OnPropertyChanged("localport");
            this.OnPropertyChanged("remoteIP");
            this.OnPropertyChanged("remoteport");
            this.OnPropertyChanged("IsConnectionAuto");
            this.OnPropertyChanged("IsConnectionRemote");
            this.OnPropertyChanged("IsConnectionLocal");
            this.OnPropertyChanged("IsConnectionHand");
        }
        public void savesetting()
        {
            Properties.Settings.Default.connect_local_Port = this.localport;
            Properties.Settings.Default.connect_remote_IPadress = this.remoteIP;
            Properties.Settings.Default.connect_remote_Port = this.remoteport;
            Properties.Settings.Default.connect_mode_auto = this.IsConnectionAuto;
            Properties.Settings.Default.connect_mode_host = this.IsConnectionRemote;
            Properties.Settings.Default.Save();
        }

        public void initsetting()
        {
            this.localport = "12500";
            this.remoteIP = "192.168.0.1";
            this.remoteport = "12500";
            this.IsConnectionAuto = true;
            this.IsConnectionRemote = false;
            this.OnPropertyChanged("localport");
            this.OnPropertyChanged("remoteIP");
            this.OnPropertyChanged("remoteport");
            this.OnPropertyChanged("IsConnectionAuto");
            this.OnPropertyChanged("IsConnectionRemote");
            this.OnPropertyChanged("IsConnectionLocal");
            this.OnPropertyChanged("IsConnectionHand");
        }

        #region INotifyPropertyChanged メンバ

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
