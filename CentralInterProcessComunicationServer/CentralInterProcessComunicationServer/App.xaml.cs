using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;


namespace CentralInterProcessCommunicationServer
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private App()
        {
            if (System.Diagnostics.Process.GetProcessesByName(System.Diagnostics.Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                //すでに起動していると判断する
                this.Shutdown();
            }
        }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            }
            catch (Exception ex)
            {
                while (true)
                {
                    myDialog dialog = new myDialog(ex.Message);
                    if (dialog.ShowDialog() == true)
                    {
                        break;
                    }
                }
            }
        }
        
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            MessageBox.Show(ex.Message + ex.Source, "例外発生(UI スレッド外)",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
            this.Shutdown();
        }
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "例外発生",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
