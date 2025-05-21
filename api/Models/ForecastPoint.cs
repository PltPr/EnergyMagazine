using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class ForecastPoint
    {
        public DateTime Date { get; set; }
        public float Temperature { get; set; }
        public float WindSpeed { get; set; }   
        //public float SolarRadiation  { get; set; }
        public float CloudCover { get; set; }
        public float Precipitation { get; set; }
        public float RelativeHumidity { get; set; }
        public float DewPoint { get; set; }
    }
}