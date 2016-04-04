using ARSoft.Tools.Net.Dns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PSXDnsServerLiteConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("输入监听IP地址后，按回车启动服务。");
            var hostIP = Console.ReadLine();
            StartDnsServer(hostIP);
        }
        static void StartDnsServer(string hostIP)
        {
            using (DnsServer dnsServer = new DnsServer(IPAddress.Parse("168.160.98.162"), 10, 10, ProcessQuery))
            {
                dnsServer.Start();
                Console.WriteLine("监听服务启动……");
                Console.ReadLine();
            }
        }

        static DnsMessageBase ProcessQuery(DnsMessageBase message, IPAddress clientAddress, ProtocolType protocol)
        {
            message.IsQuery = false;
            DnsMessage query = message as DnsMessage;
            if (query == null || query.Questions.Count <= 0)
            {
                message.ReturnCode = ReturnCode.ServerFailure;
                return message;
            }

            if (query.Questions[0].RecordType == RecordType.A)
            {
                //自定义解析规则，clientAddress即客户端的IP，dnsQuestion.Name即客户端请求的域名，Resolve为自定义的方法（代码不再贴出），返回解析后的ip，将其加入AnswerRecords中
                foreach (DnsQuestion dnsQuestion in query.Questions)
                {
                    string resolvedIp = Resolve(clientAddress.ToString(), dnsQuestion.Name);
                    ARecord aRecord = new ARecord(query.Questions[0].Name, 36000, IPAddress.Parse(resolvedIp));
                    query.AnswerRecords.Add(aRecord);
                }
            }
            else
            {
                Console.WriteLine("监听到非A记录请求，请求名：" + query.Questions[0].Name);
                //如果为IPV6请求，则交给上级DNS服务器处理
                //var answer = DnsClient.Default.Resolve(query.Questions[0].Name);
                //foreach (var record in answer.AnswerRecords)
                //    query.AnswerRecords.Add(record);
                //foreach (var record in answer.AdditionalRecords)
                //    query.AnswerRecords.Add(record);
            }

            return message;
        }

        static string Resolve(string clientAddress, string domianName)
        {
            string ipresult = string.Empty;
            if (domianName.Contains("youtube"))
            {
                ipresult = "203.208.37.0";
            }
            else
            {
                var iphostentry = Dns.GetHostEntry(domianName);
                ipresult = "168.160.98.162"; //iphostentry.AddressList[0].AddressFamily.ToString();
            }
            Console.WriteLine("请求域名：" + domianName + ",解析地址："+ipresult);
            return ipresult;
        }
    }
}
