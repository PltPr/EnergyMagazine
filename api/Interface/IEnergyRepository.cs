using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Interface
{
    public interface IEnergyRepository
    {
        Task<string>GetEnergyDataForTodayAsync();
    }
}