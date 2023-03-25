//using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace TinoMager.IoTCreatorsConnectorFunction
{
    public class TempHumImeiMeasurementDTO : TempHumMeasurementDTO{
        //[JsonPropertyName("imei")]
        [JsonProperty(PropertyName ="imei")]
        public string Imei {get;set;}


    }
}