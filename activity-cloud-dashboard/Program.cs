using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using ActivityApp.Controllers;
using ActivityApp.Models;

namespace ActivityApp
{
    public class Program
    {
        
        private static Program instance{get; set;}

        public static Program GetInstance(){
            return instance;
        }
        private ReceiveController receiver;
        private SensorsData sensorsData;

        public SensorsData GetSensorsData(){
            return sensorsData;
        }
        public static void Main(string[] args)
        {
            instance = new Program();
            instance.Init();
            CreateHostBuilder(args).Build().Run();
        }

        private void Init(){
            sensorsData = new SensorsData();
            receiver = new ReceiveController();
            receiver.Init();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("https://*:5001", "http://*:5002");
                    webBuilder.UseStartup<Startup>();
                });
    }
}
