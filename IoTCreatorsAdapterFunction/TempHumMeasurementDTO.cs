//using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace TinoMager.IoTCreatorsConnectorFunction
{
    public class TempHumMeasurementDTO{
        //[JsonPropertyName("temp")]
        [JsonProperty(PropertyName ="temp")]
        public float Temperature {get;set;}

        
        //[JsonPropertyName("hum")]
        [JsonProperty(PropertyName ="hum")]
        public float Humidity {get;set;}
    }
}