using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace api.Models
{
    public class EnergyPricePrediction
    {
        [JsonPropertyName("date")]
        public DateTime date { get; set; }

        [JsonPropertyName("predicted_price")]
        public float predictedPrice { get; set; }
    }
}