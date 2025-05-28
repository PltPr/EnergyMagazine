using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using api.Interface;
using System.Net.Http.Json;
using Newtonsoft.Json;
using System.Xml.Linq;
using api.Models;
using System.Globalization;

namespace api.Repository
{
    public class EnergyRepository : IEnergyRepository
    {
        private readonly IConfiguration _configuration;
        public EnergyRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<EnergyPricePoint>> GetEnergyDataForTodayAsync(string? startDate=null, string? endDate=null)
        {

            string apiKey = _configuration["EntsoE:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                throw new Exception("Brak klucza API w konfiguracji");


            startDate ??= DateTime.UtcNow.ToString("yyyyMMdd") + "0000";
            endDate ??= DateTime.UtcNow.ToString("yyyyMMdd") + "2300";


            string url = $"https://web-api.tp.entsoe.eu/api?" +
                         $"documentType=A44" +
                         $"&In_Domain=10YPL-AREA-----S" +
                         $"&Out_Domain=10YPL-AREA-----S" +
                         $"&periodStart={startDate}" +
                         $"&periodEnd={endDate}" +
                         $"&securityToken={apiKey}";

            using var client = new HttpClient();
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Błąd podczas pobierania danych: {(int)response.StatusCode}");

            var xmlString = await response.Content.ReadAsStringAsync();

            return ParseXml(xmlString);
        }

        public List<EnergyPricePoint> ParseXml(string xml)
        {
            var warsawTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            var culture = CultureInfo.InvariantCulture;
            var ns = (XNamespace)"urn:iec62325.351:tc57wg16:451-3:publicationdocument:7:3";
            var doc = XDocument.Parse(xml);
            var result = new List<EnergyPricePoint>();

            foreach (var timeSeries in doc.Descendants(ns + "TimeSeries"))
            {
                var period = timeSeries.Element(ns + "Period");
                if (period == null) continue;

                var startUtcStr = period.Element(ns + "timeInterval")?.Element(ns + "start")?.Value;
                if (!DateTime.TryParse(startUtcStr, out DateTime periodStartUtc))
                    continue;

                foreach (var point in period.Elements(ns + "Point"))
                {
                    var posStr = point.Element(ns + "position")?.Value;
                    var priceStr = point.Element(ns + "price.amount")?.Value;

                    if (int.TryParse(posStr,NumberStyles.Any, culture, out int position) && float.TryParse(priceStr,NumberStyles.Any, culture, out float price))
                    {
                        var utcHour = DateTime.SpecifyKind(periodStartUtc.AddHours(position - 3), DateTimeKind.Utc);
                        var localHour = TimeZoneInfo.ConvertTimeFromUtc(utcHour, warsawTimeZone);


                        result.Add(new EnergyPricePoint
                        {
                            DateTime = localHour,
                            Price = price
                        });
                    }
                }
            }

            return result;
        }

    }
}
