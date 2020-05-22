using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ActivityApp.Models;

namespace ActivityApp.Controllers
{
    [Route("api/[controller]")] 
    [ApiController]
    public class EndpointController : Controller
    {

        private readonly DataContext _context;

        public EndpointController(DataContext context)
        {
            _context = context;
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<List<DataItem>> PostDataItem(List<DataItem> dataItemList)
        {
            Console.WriteLine("+-- Got new DataItem");
            _context.DataItems.AddRange(dataItemList);
            Console.WriteLine("+-- Got " + dataItemList.Count + " items");
            await _context.SaveChangesAsync();
            DeviceController device = Program.GetInstance().GetDevice();
            if(device==null){
                Console.WriteLine("+-- Devace initialization failed. Unable to send.");
            }
            else {
                var messageString = JsonConvert.SerializeObject(dataItemList);
                Program.GetInstance().GetDevice().SendDeviceToCloudMessagesAsync(messageString, "phone1");
            }
            return dataItemList;
        }

        [HttpPost("edge")]
        public List<string> PostEdgeItems(List<string> result)
        {
            Console.WriteLine("+-- Got new item");
            Console.WriteLine("+-- Got " + result.Count + " items");
            DeviceController device = Program.GetInstance().GetDevice();
            if(device==null){
                Console.WriteLine("+-- Devace initialization failed. Unable to send.");
            }
            else {
                var messageString = JsonConvert.SerializeObject(result);
                Program.GetInstance().GetDevice().SendDeviceToCloudMessagesAsync(messageString, "phone2");
            }
            return result;
        }

    }
}
