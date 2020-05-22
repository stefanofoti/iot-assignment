using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ActivityApp.Models;
using ActivityApp;

namespace ActivityApp.Controllers
{
    [Route("api/[controller]")] 
    [ApiController]
    public class ResultController : Controller
    {

        // GET: api/Result
        //Returns last result available
        [HttpGet]
        public string GetLastActivity()
        {
            IList<StatusData> result = Program.GetInstance().GetSensorsData().statusResult;
            if(result==null||result.Count==0){
                return "Err";
            }
            return result[result.Count-1].Status;
        }

        [HttpGet("/all")]
        public string GetLastActivities()
        {
            IList<StatusData> result = Program.GetInstance().GetSensorsData().statusResult;
            var json = JsonSerializer.Serialize(result);
            return json;
        }

        [HttpGet("/edge")]
        public string GetLastActivitiesEdge()
        {
            IList<StatusData> result = Program.GetInstance().GetSensorsData().statusResultEdge;
            var json = JsonSerializer.Serialize(result);
            return json;
        }


    }
}
