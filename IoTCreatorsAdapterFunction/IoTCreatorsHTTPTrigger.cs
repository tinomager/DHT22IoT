using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Azure.Devices.Client;
using System.Text;

namespace TinoMager.IoTCreatorsConnectorFunction
{
    public static class IoTCreatorsHTTPTrigger
    {
        private static string ConvertHexToString(string hexString){
            byte[] raw = new byte[hexString.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return Encoding.ASCII.GetString(raw);
        }

        [FunctionName("IoTCreatorsHTTPTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"IoTCreatorsHTTPTrigger function processed a request with body: {req.Body}");

            string requestBody = String.Empty;
            using (StreamReader streamReader =  new  StreamReader(req.Body))
            {
                requestBody = await streamReader.ReadToEndAsync();
            }
            var data = JsonConvert.DeserializeObject<IoTCreatorsDTO>(requestBody);

            String responseMessage = String.Empty;
            var ioTHubConnectionString = Environment.GetEnvironmentVariable("IoTHubConnectionString");
            if(string.IsNullOrEmpty(ioTHubConnectionString)){
                    var logConfigMessage = "Cannot get IoT Hub connectionstring from appsettings. Is IoTHubConnectionString configured?";
                    log.LogError(logConfigMessage);
                    responseMessage = logConfigMessage;
            }
            else{
                var deviceClient = DeviceClient.CreateFromConnectionString(ioTHubConnectionString);
                responseMessage = "Processed values: ";
                if(data.Reports.Count == 0){
                    responseMessage += "none";
                }
                else{
                    foreach(var res in data.Reports){
                        //TODO: Do IoT Hub Magic here
                        var stringValue = ConvertHexToString(res.Value);
                        var jsonValue = JsonConvert.DeserializeObject<TempHumMeasurementDTO>(stringValue);
                        responseMessage += $"Device Id {res.SerialNumber} message value {stringValue}; ";

                        var message = new Message(Encoding.ASCII.GetBytes(stringValue));
                        deviceClient.SendEventAsync(message).Wait();
                        log.LogInformation($"Successfully sent {stringValue} to IoT Hub");
                    }
                }
            }
                      
            log.LogInformation($"Response message: {responseMessage}");
            return new OkObjectResult(responseMessage);
        }
    }
}
