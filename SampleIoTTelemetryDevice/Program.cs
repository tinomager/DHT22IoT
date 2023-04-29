using System;  
using System.IO;  
using System.Text;  
using System.Threading.Tasks;  
using Microsoft.Azure.Devices.Client;  
using Microsoft.Extensions.Configuration;  
using Microsoft.Extensions.Configuration.Json;  


namespace AzureIoTDeviceSample  
{  
    class Program  
    {  
        private static DeviceClient _deviceClient;  
        private static string _iotHubConnectionString;  
  
        static async Task Main(string[] args)  
        {  
            Console.WriteLine("Starting Azure IoT Device Sample...");  
  
            var configuration = LoadConfiguration();  
            _iotHubConnectionString = configuration["IoTHub:DeviceConnectionString"];  
  
            _deviceClient = DeviceClient.CreateFromConnectionString(_iotHubConnectionString, TransportType.Mqtt);  
            await SendDeviceToCloudMessagesAsync();  
  
            Console.WriteLine("Exiting...");  
        }  
  
        private static IConfiguration LoadConfiguration()  
        {  
            var builder = new ConfigurationBuilder()  
                .SetBasePath(Directory.GetCurrentDirectory())  
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);  
  
            return builder.Build();  
        }  
  
        private static async Task SendDeviceToCloudMessagesAsync()  
        {  
            int messageCount = 0;  
  
            while (true)  
            {  
                messageCount++;  
                var telemetryData = new  
                {  
                    deviceId = "SampleDevice",  
                    temperature = 25.0 + (0.1 * messageCount),  
                    humidity = 70.0 + (0.1 * messageCount)  
                };  
  
                var messageString = Newtonsoft.Json.JsonConvert.SerializeObject(telemetryData);  
                var message = new Message(Encoding.ASCII.GetBytes(messageString));  
  
                await _deviceClient.SendEventAsync(message);  
                Console.WriteLine($"Message sent: {messageString}");  
  
                await Task.Delay(10000);  
            }  
        }  
    }  
}  
