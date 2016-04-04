using ARSoft.Tools.Net.Dns;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace PSXDnsServerLite
{
    public partial class MainFrm : Form
    {
        public MainFrm()
        {
            InitializeComponent();
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {

        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            StartDnsServer();
        }

        static void StartDnsServer()
        {
            using (DnsServer dnsServer = new DnsServer(IPAddress.Parse("127.0.0.1"), 50, 50, ProcessQuery))
            {
                dnsServer.Start();
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
                //如果为IPV6请求，则交给上级DNS服务器处理
                var answer = DnsClient.Default.Resolve(query.Questions[0].Name);
                foreach (var record in answer.AnswerRecords)
                    query.AnswerRecords.Add(record);
                foreach (var record in answer.AdditionalRecords)
                    query.AnswerRecords.Add(record);
            }

            return message;
        }

        static string Resolve(string clientAddress, string domianName)
        {
            return "127.0.0.1";
        }
    }
}
