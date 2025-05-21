using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class HourlyData
    {
        public List<string> time { get; set; }
        public List<float> temperature_2m { get; set; }
        public List<float> wind_speed_10m { get; set; }
        //public List<float> solar_radiation { get; set; }
        public List<float> cloudcover { get; set; }
        public List<float> precipitation { get; set; }
        public List<float> relative_humidity_2m { get; set; }
        public List<float> dew_point_2m { get; set; }
    }

    public class WeatherResponse
    {
        public HourlyData hourly { get; set; }
    }
}