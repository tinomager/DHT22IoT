using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TinoMager.IoTCreatorsConnectorFunction
{
    public class IoTCreatorsDTO
    {
        [JsonPropertyName("reports")]
        public List<Report> Reports { get; set; }

        [JsonPropertyName("registrations")]
        public List<object> Registrations { get; set; }

        [JsonPropertyName("deregistrations")]
        public List<object> Deregistrations { get; set; }

        [JsonPropertyName("updates")]
        public List<object> Updates { get; set; }

        [JsonPropertyName("expirations")]
        public List<object> Expirations { get; set; }

        [JsonPropertyName("responses")]
        public List<object> Responses { get; set; }
    }

    public class Report
    {
        [JsonPropertyName("serialNumber")]
        public string SerialNumber { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("subscriptionId")]
        public string SubscriptionId { get; set; }

        [JsonPropertyName("resourcePath")]
        public string ResourcePath { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}