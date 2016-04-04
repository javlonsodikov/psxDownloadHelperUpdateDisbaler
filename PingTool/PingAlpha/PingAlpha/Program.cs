using System;
using System.IO;
using System.Net.NetworkInformation;

namespace PingAlpha
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (!File.Exists("address.txt"))
            {
                Console.WriteLine("请在程序根目录建立address.txt文件，并将要测试的地址填入保存后再运行本程序！\n按任意键退出。");
                Console.ReadKey();
                return;
            }
            var sr = new StreamReader("address.txt");
            int i = 0;
            while (true)
            {
                i++;
                try
                {
                    string lineStr = sr.ReadLine();
                    if (String.IsNullOrEmpty(lineStr))
                        break;

                    string ip = lineStr.Split(' ')[0];
                    var pingThread = new Ping();
                    PingReply reply = pingThread.Send(ip);
                    OutPutStream(reply, lineStr);
                }
                catch
                {
                    continue;
                }
            }
            sr.Close();
            Console.WriteLine("本次共测试{0}个IP地址，其中链接成功延迟在200以下的有{1}个，链接成功延迟在200以上的{2}个，链接超时{3}个，其他未链接成功{4}个。\n按任意键退出", i, s,d, t, o);
            Console.ReadKey();
        }

        private static int s = 0;
        private static int t = 0;
        private static int d = 0;
        private static int o = 0;
        private static void OutPutStream(PingReply reply, string info)
        {
            string path = "Report_other.txt";
            if (reply.Status == IPStatus.Success)
            {
                if (reply.RoundtripTime > 200)
                {
                    d++;
                    path = "Report_Delay.txt";
                }
                else
                {
                    s++;
                    path = "Report_Success.txt";
                }
            }
            else if (reply.Status == IPStatus.TimedOut)
            {
                t++;
                path = "Report_Timeout.txt";
            }
            else o++;

            string str = info + "    [Reply：" + reply.Status + "   " + reply.RoundtripTime + "ms]";
            Console.WriteLine(str);
            var sw = new StreamWriter(path,true);
            sw.WriteLine(str);
            sw.Close();
        }
    }
}