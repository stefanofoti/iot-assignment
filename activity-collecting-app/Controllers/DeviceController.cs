// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// This application uses the Azure IoT Hub device SDK for .NET
// For samples see: https://github.com/Azure/azure-iot-sdk-csharp/tree/master/iothub/device/samples

using System;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace ActivityApp.Controllers
{
    public class DeviceController
    {
        private DeviceClient s_deviceClient;
        private DeviceClient s_deviceClientEdge;

        private readonly string s_connectionString = "";
        private readonly string s_connectionStringEdge = "";
        

        // Async method to send simulated telemetry
        public async void SendDeviceToCloudMessagesAsync(String txt, String device)
        {
            var message = new Message(Encoding.ASCII.GetBytes(txt));
            if(device.Equals("phone1")){
                await s_deviceClient.SendEventAsync(message);

            } else if (device.Equals("phone2")){
                await s_deviceClientEdge.SendEventAsync(message);
            }
            Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, txt);

        }

        public void init(){
            s_deviceClient = DeviceClient.CreateFromConnectionString(s_connectionString, TransportType.Mqtt);
            s_deviceClientEdge = DeviceClient.CreateFromConnectionString(s_connectionStringEdge, TransportType.Mqtt);
        }
    }
}