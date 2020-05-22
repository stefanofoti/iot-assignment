using System;

namespace ActivityApp.Models
{
    public class DataItem
    {
        public string Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public double Magnitude {get; set;}
        public double Delta {get; set;}

    }
}