using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using ActivityApp.Controllers;
using System;

namespace ActivityApp
{
    public class Program
    {
        
        private static Program instance;

        private DeviceController device {get; set;}
        public static Program GetInstance(){
            return instance;
        }

        public DeviceController GetDevice(){
            if(device==null){
                Console.WriteLine("+-- Devace initialization failed.\n+--- trying to re-init");
                device.init();
            }
            return device;
        }

        private void init(){
            device = new DeviceController();
            device.init();
        }

        public static void Main(string[] args)
        {
            Program.instance = new Program();
            instance.init();
            CreateHostBuilder(args).Build().Run();   
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("https://*:8080", "http://*:80");
                });
    }
}
