using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using PSXDH.BLL;
using PSXDH.Model;

namespace PSXDownloadHelper
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Code.SettingHelper.InitSettings();
            var uiCulture = new CultureInfo(AppConfig.Instance().Language);
            Application.ThreadException += OnThreadException;
            Thread.CurrentThread.CurrentUICulture = uiCulture;
            Application.Run(ServerConfig.ServerInstance());
        }

        private static void OnThreadException(object sender, ThreadExceptionEventArgs args)
        {
            try
            {
                var errorMsg = "Error:\n";
                errorMsg += args.Exception.Message;
                errorMsg += "\nSource:";
                errorMsg += args.Exception.Source;
                errorMsg += "\nStackTrace:\n";
                errorMsg += args.Exception.StackTrace;
                MessageBox.Show(errorMsg, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show(@"Fatal error!", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }
    }
}