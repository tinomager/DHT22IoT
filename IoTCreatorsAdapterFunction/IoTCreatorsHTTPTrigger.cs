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
            string requestBody = String.Empty;
            using (StreamReader streamReader =  new  StreamReader(req.Body))
            {
                requestBody = await streamReader.ReadToEndAsync();
            }
            log.LogInformation($"IoTCreatorsHTTPTrigger function processed a request with body: {requestBody}");

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
                        var stringValue = ConvertHexToString(res.Value);
                        var jsonValue = JsonConvert.DeserializeObject<TempHumMeasurementDTO>(stringValue);
                        var messageJson = new TempHumImeiMeasurementDTO();
                        messageJson.Temperature = jsonValue.Temperature;
                        messageJson.Humidity = jsonValue.Humidity;
                        if(res.SerialNumber.Contains(':')){
                            messageJson.Imei = res.SerialNumber.Split(':')[1];
                        }
                        else{
                            messageJson.Imei = res.SerialNumber;
                        }
                        var messageStringValue = JsonConvert.SerializeObject(messageJson);

                        responseMessage += $"Device Id {res.SerialNumber} message value {messageStringValue}; ";

                        var message = new Message(Encoding.ASCII.GetBytes(messageStringValue));
                        deviceClient.SendEventAsync(message).Wait();
                        log.LogInformation($"Successfully sent {messageStringValue} to IoT Hub");
                    }
                }
            }
                      
            log.LogInformation($"Response message: {responseMessage}");
            return new OkObjectResult(responseMessage);
        }
        
    }
}
