using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DnsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            DnsProxy.DefaultV4.Start();
            DnsProxy.DefaultV6.Start();
            new ManualResetEvent(false).WaitOne();
        }
    }
}
