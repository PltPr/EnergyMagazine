using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class EnergyPricePoint
    {
        public DateTime DateTime { get; set; }
        public double Price { get; set; }
    }
}