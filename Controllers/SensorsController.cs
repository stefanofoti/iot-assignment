using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using IotWork.Models;

namespace IotWork.Controllers
{
    [Route("api/[controller]")] 
    public class SensorsController : Controller
    {

        // GET api/sensors/sensorID
        [HttpGet("{sensorID}/values")]
        public string GetValuesOnly(string sensorID)
        {
            return Program.getInstance().GetSensorsData().GetOnlyValuesForSensor(sensorID);
        }


        // GET api/sensors/sensorID
        [HttpGet("{sensorID}/labels")]
        public string GetIdsOnly(string sensorID)
        {
            return Program.getInstance().GetSensorsData().GetOnlyIdsForSensors(sensorID);
        }

        // GET api/sensors/sensorID
        [HttpGet("{sensorID}/last")]
        public string GetLastValue(string sensorID)
        {
            //DataItem di = Program.getInstance().GetSensorsData().getLastItem("thermo");
            //if(di==null){
            //    return "Not found";
            //}
            //return di.getValue();
            string m = Program.getInstance().GetSensorsData().GetLastValueForSensor(sensorID);
            return m;
        }


        // GET api/sensors/sensorID
        [HttpGet("{sensorID}/all")]
        public string GetLastHour(string sensorID)
        {
            string m = Program.getInstance().GetSensorsData().GetLastHourValuesForSensor(sensorID);
            return m;

        }
    }
}
