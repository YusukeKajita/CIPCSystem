using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CIPCTerminal.CIPCDiagnostics
{
    public class OwnCIPCProcess
    {
        public System.Diagnostics.Process process { set; get; }
        public CIPCDiagnosticsWindow window { set; get; }

        public OwnCIPCProcess()
        {

        }
        public OwnCIPCProcess(CIPCDiagnosticsWindow window)
        {
            this.window = window;
        }

        private void Init_Value()
        {
        }

        public void Update()
        {
            System.Diagnostics.Process[] CIPC = System.Diagnostics.Process.GetProcessesByName("CentralInterProcessCommunicationServer");
            if (CIPC.Length != 0)
            {
                this.process = CIPC[0];
                this.window.TextBlock_Local_CIPCState.Text = "動作中";
                this.window.TextBlock_Local_CIPCTime.Text = (DateTime.Now - CIPC[0].StartTime).ToString();
                this.window.TextBlock_Local_CIPCRespond.Text = CIPC[0].Responding.ToString();
                this.window.TextBlock_Local_CIPCID.Text = CIPC[0].Id.ToString();
            }
            else
            {
                this.window.TextBlock_Local_CIPCState.Text = "未起動";
                this.window.TextBlock_Local_CIPCTime.Text = "";
                this.window.TextBlock_Local_CIPCRespond.Text = "";
                this.window.TextBlock_Local_CIPCID.Text = "";
            }
            if (this.window.CheckBox_AutoRestart.IsChecked == true)
            {
                if (this.process == null)
                {
                    this.start();
                }
                else if (!this.process.Responding)
                {
                    this.restart();
                }
            }
            else
            {
                switch (this.window.handle)
                {
                    case CIPCDiagnosticsWindow.Handle.Start:
                        this.start();
                        break;
                    case CIPCDiagnosticsWindow.Handle.Close:
                        this.exit();
                        break;
                    case CIPCDiagnosticsWindow.Handle.Restart:
                        this.restart();
                        break;
                    default:
                        break;
                }
            }
            CIPC = null;
        }

        public void restart()
        {
            this.exit();
            Thread.Sleep(300);
            this.start();
        }
        public void exit()
        {
            if (this.process != null)
            {
                if (!this.process.HasExited)
                {
                    this.process.Kill();
                }
            }
        }
        public void start()
        {
            try
            {
                System.Diagnostics.Process[] CIPC = System.Diagnostics.Process.GetProcessesByName("CentralInterProcessCommunicationServer");
                if (CIPC.Length == 0)
                {
                    System.Diagnostics.Process.Start(this.window.TextBox_Local_CIPCServerPath.Text);
                }
            }
            catch (Exception ex)
            {
                while (true)
                {
                    myparts.myDialog mydialog = new myparts.myDialog(ex.Message);
                    if (mydialog.ShowDialog() == true)
                    {
                        break;
                    }
                }
            }
        }
    }
}
