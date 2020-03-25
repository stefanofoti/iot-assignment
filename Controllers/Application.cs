using System;
using Microsoft.Azure;
using System.Net;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Http.Headers;
using IotWork.Controllers;
using System.ComponentModel;
using IotWork.Models;
//using uPLibrary.Networking.M2Mqtt;
//using uPLibrary.Networking.M2Mqtt.Messages;

//https://m2mqtt.wordpress.com/using-mqttclient/

namespace IotWork.Controllers
{
    public class Application : Controller
    {

        private HubController hubController;

        private SensorsData sensorsData = null;

        public void run(){
            sensorsData = new SensorsData();
            startCollectingData();
        }

        private void startCollectingData(){
            try
            {
                hubController = new HubController();
                hubController.run();                
            }
            catch (System.Exception)
            {
                Console.WriteLine("Restarting data collecting... ");
                startCollectingData();
            }

        }

        public SensorsData GetSensorsData(){
            return sensorsData;
        }

    }
}

