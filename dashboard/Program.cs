using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using IotWork.Controllers;

namespace IotWork
{
    public class Program
    {
        private static Application _instance;

        public static Application getInstance(){
            return _instance;
        } 

        public static void Main(string[] args)
        {
            _instance = new Application();
            _instance.run();
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();
            host.Run();

        }
    }
}
