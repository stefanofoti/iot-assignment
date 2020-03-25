// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// This application uses the Microsoft Azure Event Hubs Client for .NET
// For samples see: https://github.com/Azure/azure-event-hubs/tree/master/samples/DotNet
// For documenation see: https://docs.microsoft.com/azure/event-hubs/
using System;
using Microsoft.Azure.EventHubs;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using IotWork.Models;

namespace IotWork.Controllers
{
    class HubController
    {
        private BackgroundWorker _worker;
        String output;

        public void run(){
            List<Object> args = new List<Object>();
            Application application = Program.getInstance();
            Console.WriteLine("Application " + application);
            SensorsData sensorsData = application.GetSensorsData();
            Console.WriteLine("SensorsData in AzureSensore: " + sensorsData);
            args.Add(sensorsData);
            _worker = new BackgroundWorker();
            _worker.DoWork += Worker_DoWork;
            _worker.RunWorkerCompleted += Work_Completed;
            _worker.RunWorkerAsync(args);
        }
        private void Work_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("e: " + e);
            Console.WriteLine("e.Result: " + e.Result);
            output = e.Result.ToString();
            //Program.getInstance().GetSensorsData().addValue(sensorName, output);
            //run();
            Console.WriteLine("output="+output);
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<object> argumentList = e.Argument as List<object>;
            SensorsData sensorsData = (SensorsData) argumentList[0];
            if(sensorsData!=null){
                Console.WriteLine("Calling readMessage...");
                StartReading(sensorsData).GetAwaiter().GetResult();
                e.Result = "END";
            }
        }

        // Event Hub-compatible endpoint
        // az iot hub show --query properties.eventHubEndpoints.events.endpoint --name {your IoT Hub name}
        private readonly static string s_eventHubsCompatibleEndpoint = "";

        // Event Hub-compatible name
        // az iot hub show --query properties.eventHubEndpoints.events.path --name {your IoT Hub name}
        private readonly static string s_eventHubsCompatiblePath = "";
        
        // az iot hub policy show --name service --query primaryKey --hub-name {your IoT Hub name}
        private readonly static string s_iotHubSasKey = "";
        private readonly static string s_iotHubSasKeyName = "service";
        private static EventHubClient s_eventHubClient;





        // Asynchronously create a PartitionReceiver for a partition and then start 
        // reading any messages sent from the simulated client.
        private async Task ReceiveMessagesFromDeviceAsync(string partition, CancellationToken ct, SensorsData sensorsData)
        {
            // Create the receiver using the default consumer group.
            // For the purposes of this sample, read only messages sent since 
            // the time the receiver is created. Typically, you don't want to skip any messages.
            var eventHubReceiver = s_eventHubClient.CreateReceiver("$Default", partition, EventPosition.FromEnqueuedTime(DateTime.Now));
            Console.WriteLine("Create receiver on partition: " + partition);
            while (true)
            {
                //if (ct.IsCancellationRequested) break;
                Console.WriteLine("Listening for messages on: " + partition);
                // Check for EventData - this methods times out if there is nothing to retrieve.
                var events = await eventHubReceiver.ReceiveAsync(100);

                // If there is data in the batch, process it.
                if (events == null) continue;

                foreach(EventData eventData in events)
                { 
                  string data = Encoding.UTF8.GetString(eventData.Body.Array);
                  Console.WriteLine("Message received on partition {0}:", partition);
                  Console.WriteLine("  {0}:", data);
                  Console.WriteLine("Application properties (set by device):");
                  foreach (var prop in eventData.Properties)
                  {
                    Console.WriteLine("  {0}: {1}", prop.Key, prop.Value);
                  }
                  Console.WriteLine("System properties (set by IoT Hub):");
                  foreach (var prop in eventData.SystemProperties)
                  {
                    Console.WriteLine("  {0}: {1}", prop.Key, prop.Value);
                  }
                  sensorsData.AddData(eventData);
                }
            }
        }

        public async Task StartReading(SensorsData sensorsData)
        {
            Console.WriteLine("Read device to cloud messages. Ctrl-C to exit.\n");

            // Create an EventHubClient instance to connect to the
            // IoT Hub Event Hubs-compatible endpoint.
            var connectionString = new EventHubsConnectionStringBuilder(new Uri(s_eventHubsCompatibleEndpoint), s_eventHubsCompatiblePath, s_iotHubSasKeyName, s_iotHubSasKey);
            s_eventHubClient = EventHubClient.CreateFromConnectionString(connectionString.ToString());

            // Create a PartitionReciever for each partition on the hub.
            var runtimeInfo = await s_eventHubClient.GetRuntimeInformationAsync();
            var d2cPartitions = runtimeInfo.PartitionIds;

            CancellationTokenSource cts = new CancellationTokenSource();

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
            };

            var tasks = new List<Task>();
            foreach (string partition in d2cPartitions)
            {
                tasks.Add(ReceiveMessagesFromDeviceAsync(partition, cts.Token, sensorsData));
            }

            // Wait for all the PartitionReceivers to finsih.
            Task.WaitAll(tasks.ToArray());
        }
    }
}