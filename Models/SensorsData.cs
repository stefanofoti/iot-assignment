using System;
using Microsoft.Azure;
using System.Net;
using System.Text;
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
using Microsoft.Azure.EventHubs;

namespace IotWork.Models
{
    public class SensorsData : Controller
    {
        private IDictionary<String, IList<DataItem>> mainDictionary;

        public SensorsData(){
            mainDictionary = new Dictionary<String, IList<DataItem>>();
        }

        public void AddData(EventData ed){
            Console.WriteLine("+-- Aggiungo evento: " + ed);
            String deviceId = (String) ed.SystemProperties["iothub-connection-device-id"].ToString(); 
            Console.WriteLine("+-- DeviceId " + deviceId);
            if(deviceId!=null){
                Console.WriteLine("+-- DeviceId not null");
                IList<DataItem> currentList = null;// = mainDictionary[deviceId.Trim()];
                if(!mainDictionary.ContainsKey(deviceId)){
                    Console.WriteLine("+-- creo la lista");
                    currentList = new List<DataItem>();
                    mainDictionary.Add(deviceId, currentList);
                } else {
                    currentList=mainDictionary[deviceId];
                }
                currentList.Add(GenerateDataItem(ed));
                if(currentList.Count>120){
                    currentList.RemoveAt(0);
                }
            }
            Console.WriteLine("+-- AddData end");
        }

        public String GetLastValueForSensor(String deviceId){
            IList<DataItem> currentList = mainDictionary[deviceId];
            if(currentList==null || currentList.Count==0){
                return "";
            }
            DataItem di = currentList[currentList.Count-1];
            String json = JsonConvert.SerializeObject(di);
            return json;
        }

        public String GetLastHourValuesForSensor(String deviceId){
            IList<DataItem> currentList = mainDictionary[deviceId];
            if(currentList==null || currentList.Count==0){
                return "";
            }
            String json = JsonConvert.SerializeObject(currentList);
            return json;
        }
        public DataItem GenerateDataItem(EventData ed){
            DataItem dataItem = new DataItem();
            Console.WriteLine("+-- genero dataItem");
            if(ed.Body.Array!=null){
                Console.WriteLine("Received message: " + Encoding.UTF8.GetString(ed.Body.Array));
                IDictionary<String, String> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(Encoding.UTF8.GetString(ed.Body.Array));
                if(values.ContainsKey("messageId")) dataItem.setMessageId(values["messageId"]);
                if(values.ContainsKey("value")) dataItem.setValue(values["value"]);
            }
            Console.WriteLine("+-- returning data item");
            return dataItem;
        }

        public String GetOnlyValuesForSensor(String sensor){
            IList<float> output = new List<float>();
            if(mainDictionary.ContainsKey(sensor)){
                IList<DataItem> list = mainDictionary[sensor];
                foreach (DataItem item in list){  
                    output.Add(float.Parse(item.getValue()));
                }
                return JsonConvert.SerializeObject(output);
            }
            return "[]";
        }

        public String GetOnlyIdsForSensors(String sensor){
            IList<String> output = new List<String>();
            if(mainDictionary.ContainsKey(sensor)){
                IList<DataItem> list = mainDictionary[sensor];
                foreach (DataItem item in list){  
                    output.Add(item.getMessageId());
                }
                return JsonConvert.SerializeObject(output);
            }
            return "[]";
        }

    }
}

