// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Azure.Devices.Client;

namespace sender_cs
{
    public class MessageSample
    {
        private static Random s_randomGenerator;
        private DeviceClient _deviceClient;
        private int _messageId = 0;
        private int _humidity;
        private int _temperature;
        private int _windDirection;
        private int _windIntensity;
        private int _rain;


        public MessageSample(DeviceClient deviceClient)
        {
            _deviceClient = deviceClient ?? throw new ArgumentNullException(nameof(deviceClient));
            s_randomGenerator = new Random();
        }

        public async Task RunSampleAsync(){
            await SendEvent().ConfigureAwait(false);
        }

        private async Task SendEvent(){
            string dataBuffer;
            _temperature = s_randomGenerator.Next(Program.MIN_TEMPERATURE, Program.MAX_TEMPERATURE);
            _humidity = s_randomGenerator.Next(Program.MIN_HUMIDITY, Program.MAX_HUMIDITY);
            _windDirection = s_randomGenerator.Next(Program.MIN_WIND_DIRECTION, Program.MAX_WIND_DIRECTION);
            _windIntensity = s_randomGenerator.Next(Program.MIN_WIND_INTENSITY, Program.MAX_WIND_INTENSITY);
            _rain = s_randomGenerator.Next(Program.MIN_RAIN, Program.MAX_RAIN);
            //_messageId=DateTime.Now.ToString("HHmmss");
            _messageId++;
            dataBuffer = $"{{\"messageId\":{_messageId},\"temperature\":{_temperature},\"humidity\":{_humidity},\"windDirection\":{_windDirection},\"windIntensity\":{_windIntensity},\"rain\":{_rain}}}";
            Message eventMessage = new Message(Encoding.UTF8.GetBytes(dataBuffer));
            Console.WriteLine("Sending message " + dataBuffer);
            await _deviceClient.SendEventAsync(eventMessage).ConfigureAwait(false);
        }

    }
}