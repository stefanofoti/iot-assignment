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
using ActivityApp.Controllers;
using System.ComponentModel;
using Microsoft.Azure.EventHubs;

namespace ActivityApp.Models
{
    public class SensorsData : Controller
    {

        const string sensor = "phone1";

        const string edgeSensor = "phone2";
        const string RUNNING = "Running";
        const string CAR = "Car";
        const string WALKING = "Walking";
        const string STANDING = "Standing";


        public IDictionary<String, Object> mainDictionary;

        public IList<StatusData> statusResult;
        public IList<StatusData> statusResultEdge;
        public SensorsData(){
            mainDictionary = new Dictionary<String, Object>();
            statusResult = new List<StatusData>();
            statusResultEdge = new List<StatusData>();
        }

        public void AddData(EventData ed){
            Console.WriteLine("+-- Aggiungo evento: " + ed);
            String deviceId = (String) ed.SystemProperties["iothub-connection-device-id"].ToString(); 
            Console.WriteLine("+-- DeviceId " + deviceId);
            if(deviceId!=null){
                Console.WriteLine("+-- DeviceId not null");
                Object currentList = null;
                if(!mainDictionary.ContainsKey(deviceId)){
                    Console.WriteLine("+-- creo la lista");
                    if(deviceId.Equals(sensor)){
                        currentList = new List<DataItem>();
                        mainDictionary.Add(deviceId, currentList);
                    }
                    else if(deviceId.Equals(edgeSensor)){
                        currentList = new List<string>();
                        mainDictionary.Add(deviceId, currentList);
                    }
                } else {
                    currentList=mainDictionary[deviceId];
                }
                DataItem lastDataItem = null;
                if(deviceId.Equals(sensor)){
                    List<DataItem> curr = (List<DataItem>)currentList;
                    if(curr.Count > 0){
                        lastDataItem = curr[curr.Count-1];
                    }
                    IList<DataItem> newValues = GenerateDataItem(ed);
                    foreach (var item in newValues)
                    {
                        double Magnitude = Math.Sqrt(item.X*item.X + item.Y*item.Y + item.Z*item.Z);
                        item.Magnitude = Magnitude;
                        if(lastDataItem!=null){
                            item.Delta = Magnitude-lastDataItem.Magnitude;
                        }
                        curr.Add(item);
                        if(curr.Count>200){
                            curr.RemoveAt(0);
                        }
                    }
                    computeStatus(newValues);
                }
                if(deviceId.Equals(edgeSensor)){
                    List<string> allItems = (List<string>)currentList;
                    List<string> newItems = JsonConvert.DeserializeObject<List<string>>(Encoding.UTF8.GetString(ed.Body.Array));
                    allItems.AddRange(newItems);
                    int i = statusResultEdge.Count;
                    foreach (var item in newItems)
                    {
                        StatusData sd = new StatusData();
                        sd.Id=i.ToString();
                        sd.Status=item;
                        statusResultEdge.Add(sd);
                        i++;
                    }
                    Console.WriteLine("+-- added: " + Encoding.UTF8.GetString(ed.Body.Array));
                }
            }
            Console.WriteLine("+-- AddData end");
        }

        private void computeStatus(IList<DataItem> list){
            int freq = 20;
            for(int i = 0; i<list.Count; i=i+freq){
                string result = GetActivityForRange(list, i, i+freq);
                StatusData sd = new StatusData();
                sd.Id=statusResult.Count.ToString();
                sd.Status = result;
                statusResult.Add(sd);
                Console.WriteLine("+--- Computed status: " + result);               
            }
        }

        public string GetActivityForRange(IList<DataItem> lista, int start, int end)
        {
            IDictionary<string,int> result = initMap();
            for (int i = start; i < end; i++){
                DataItem di = lista[i];
                if(di.Delta < 0.5){
                    result[STANDING]++;           
                } else if(di.Delta < 1.5){
                    result[CAR]++;
                } else if(di.Delta < 4.5){
                    result[WALKING]++;
                } else {
                    result[RUNNING]++;
                }
            }
            return GetResult(result);
        }

        private IDictionary<string, int> initMap(){
            IDictionary<string,int> result = new Dictionary<string, int>();
            result[RUNNING] = 0;
            result[CAR] = 0;
            result[STANDING] = 0;
            result[WALKING] = 0;
            return result;
        }

        private string GetResult(IDictionary<string, int> map){
            string max = "NOT_FOUND";
            int freqMax = 0; 
            foreach (var pair in map){
                if(pair.Value > freqMax){
                    freqMax=pair.Value;
                    max=pair.Key;
                }
            }
            return max;
        }


        public IList<DataItem> GenerateDataItem(EventData ed){
            IList<DataItem> values = new List<DataItem>();
            Console.WriteLine("+-- genero dataItem");
            if(ed.Body.Array!=null){
                Console.WriteLine("Received message: " + Encoding.UTF8.GetString(ed.Body.Array));
                values = JsonConvert.DeserializeObject<List<DataItem>>(Encoding.UTF8.GetString(ed.Body.Array));
                Console.WriteLine("+------------- VALUES contains: " + values.Count + "values");
                if(values[0]!=null){
                    Console.WriteLine("+------------- NOT NULL VALUE");
                    Console.WriteLine("+------------- VALUES[0].x: " + values[0].X);
                }
                //Console.WriteLine("+------------- VALUES[humidity]: " + values["humidity"]);
                //Console.WriteLine("+------------- VALUES[windIntensity]: " + values["windIntensity"]);
                //Console.WriteLine("+------------- VALUES[windDirection]: " + values["windDirection"]);
                //Console.WriteLine("+------------- VALUES[rain]: " + values["rain"]);
                //if(values.ContainsKey("messageId")) dataItem.setMessageId(values["messageId"]);
                //if(values.ContainsKey("value")) dataItem.setValue(values["value"]);
                //if(values.ContainsKey("temperature")) dataItem.setTemperature(float.Parse(values["temperature"]));
                //if(values.ContainsKey("humidity")) dataItem.setHumidity(float.Parse(values["humidity"]));
                //if(values.ContainsKey("windIntensity")) dataItem.setWindIntensity(float.Parse(values["windIntensity"]));
                //if(values.ContainsKey("windDirection")) dataItem.setWindDirection(float.Parse(values["windDirection"]));
                //if(values.ContainsKey("rain")) dataItem.setRain(float.Parse(values["rain"]));
            }
            Console.WriteLine("+-- returning data item");
            return values;
        }

/*        public String GetOnlyValuesForSensor(String sensor){
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

        public String GetAllValues(String sensor){
            IList<IList<float>> output = new List<IList<float>>();
            IList<float> temperatures = new List<float>();
            IList<float> humidities = new List<float>();
            IList<float> windDirections = new List<float>();
            IList<float> windIntensities = new List<float>();
            IList<float> rains = new List<float>();
            if(mainDictionary.ContainsKey(sensor)){
                IList<DataItem> list = mainDictionary[sensor];
                foreach (DataItem item in list){
                    temperatures.Add(item.getTemperature());
                    humidities.Add(item.getHumidity());
                    windDirections.Add(item.getWindDirection());
                    windIntensities.Add(item.getWindIntensity());
                    rains.Add(item.getRain());
                }
                output.Add(temperatures);
                output.Add(humidities);
                output.Add(windDirections);
                output.Add(windIntensities);
                output.Add(rains);
                return JsonConvert.SerializeObject(output);
            }
            return "[]";
        }
*/

    }
}
