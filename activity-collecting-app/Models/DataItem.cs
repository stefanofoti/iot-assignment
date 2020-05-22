using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ActivityApp.Models
{
    public class DataItem
    {
        [Key]
        public long Id { get; set; }

        public double X {get; set;}
        public double Y {get; set;}
        public double Z {get; set;}
          
    }
}