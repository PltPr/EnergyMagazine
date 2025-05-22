using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class TrainResponse
    {
        public string message { get; set; }
        public Dictionary<string, object> best_params { get; set; }
        public double best_mse { get; set; }
        public double best_rmse { get; set; }
    }
}