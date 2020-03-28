using System;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace sender_cs
{
    class Program
    {
        private static DeviceClient deviceClient_1;
        private static DeviceClient deviceClient_2;

        public const int MAX_TEMPERATURE = 50;
        public const int MIN_TEMPERATURE = -50;
        public const int MAX_HUMIDITY= 100;
        public const int MIN_HUMIDITY = 0;
        public const int MAX_WIND_DIRECTION = 360;
        public const int MIN_WIND_DIRECTION = 0;
        public const int MAX_WIND_INTENSITY = 100;
        public const int MIN_WIND_INTENSITY = 0;
        public const int MAX_RAIN = 50;
        public const int MIN_RAIN = 0;

        private static string s_deviceConnectionString_1 = "";
        private static string s_deviceConnectionString_2 = "";

        private static TransportType s_transportType = TransportType.Mqtt;

        public static void Main(string[] args)
        {
            deviceClient_1 = DeviceClient.CreateFromConnectionString(s_deviceConnectionString_1, s_transportType);
            deviceClient_2 = DeviceClient.CreateFromConnectionString(s_deviceConnectionString_2, s_transportType);
            var msg_1 = new MessageSample(deviceClient_1);
            var msg_2 = new MessageSample(deviceClient_2);
            while(true){
                msg_1.RunSampleAsync().GetAwaiter().GetResult();
                msg_2.RunSampleAsync().GetAwaiter().GetResult();
                Thread.Sleep(30000);
            }
        }
    }
}