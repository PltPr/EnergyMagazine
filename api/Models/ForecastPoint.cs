using System;
using System.Text.Json.Serialization;

namespace api.Models
{
    public class ForecastPoint
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("temperature")]
        public float Temperature { get; set; }

        [JsonPropertyName("windSpeed")]
        public float WindSpeed { get; set; }

        [JsonPropertyName("cloudCover")]
        public float CloudCover { get; set; }

        [JsonPropertyName("precipitation")]
        public float Precipitation { get; set; }

        [JsonPropertyName("relativeHumidity")]
        public float RelativeHumidity { get; set; }

        [JsonPropertyName("dewPoint")]
        public float DewPoint { get; set; }
    }
}
