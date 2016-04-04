using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PSXDnsServerLite
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;
            Application.Run(new MainFrm());
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show("程序运行时遇到问题，错误信息：" + e.Exception.Message);
        }
    }
}
