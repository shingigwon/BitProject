using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MCBChat_Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Address
            Uri uri = new Uri(ConfigurationManager.AppSettings["addr"]);
            ServiceHost host = new ServiceHost(typeof(MCBChat_Server.ChatService), uri);

            //오픈
            host.Open();
            Console.WriteLine("맴버 관리를 시작합니다. {0}", uri.ToString());
            Console.WriteLine("http://10.101.33.242:9000/Service");
            Console.WriteLine("멈추시려면 엔터를 눌러주세요..");
            Console.ReadLine();
            //서비스
            host.Abort();
            host.Close();
        }
    }
}
