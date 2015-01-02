using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewRelic.ResufulApi.CSharp.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new NewRelicClient("your_newrelic_apikey");
            client.NotifyDeployAsync("App1",
                "test deploy 1",
                "rev1",
                "change1",
                "from newrelic restful api client").Wait();
        }
    }
}
