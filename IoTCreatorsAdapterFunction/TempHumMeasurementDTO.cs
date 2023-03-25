using System.Text.Json.Serialization;

namespace TinoMager.IoTCreatorsConnectorFunction
{
    public class TempHumMeasurementDTO{
        [JsonPropertyName("temp")]
        public float Temperature {get;set;}

        
        [JsonPropertyName("hum")]
        public float Humidity {get;set;}
    }
}