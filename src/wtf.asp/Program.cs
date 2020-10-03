using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Elmah.Io.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace wtf.asp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .ConfigureLogging(factory =>
                {
                    // You will need to replace these parameters with your own available on https://elmah.io
                    factory.AddElmahIo("168e8ebc7e4d4755bf9f1038ab8b5ecd", Guid.Parse("4edc9492-8c3c-44bf-a727-53f708f2fba5"));
                })
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
