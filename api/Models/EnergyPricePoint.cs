using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace api.Models
{
    public class EnergyPricePoint
    {
        public DateTime DateTime { get; set; }
        public float Price { get; set; }
    }
}