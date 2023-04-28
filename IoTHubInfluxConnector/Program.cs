using System;  
using System.Text;  
using System.Threading.Tasks;  
using Microsoft.Azure.EventHubs;  
using Microsoft.Azure.EventHubs.Processor;  
using InfluxDB.Client;
  
namespace IoTHubInfluxConnector  
{  
    class Program  
    {  
        private static string? EventHubConnectionString = Environment.GetEnvironmentVariable("EventHubConnectionString");  
        private static string? EventHubName = Environment.GetEnvironmentVariable("EventHubName");  
        private static string? InfluxUrl = Environment.GetEnvironmentVariable("InfluxUrl");  
        private static string? InfluxPort = Environment.GetEnvironmentVariable("InfluxPort");  
        private static string? StorageAccountKey = Environment.GetEnvironmentVariable("StorageAccountKey");  
        private static string? StorageContainerName = Environment.GetEnvironmentVariable("StorageAccountContainerName");  
        private static string? StorageAccountName = Environment.GetEnvironmentVariable("StorageAccountName");  
        static async Task Main(string[] args)  
        {  
            Console.WriteLine("Starting Event Processor Host...");  
    
            var eventProcessorHost = new EventProcessorHost(  
                EventHubName,  
                PartitionReceiver.DefaultConsumerGroupName,  
                EventHubConnectionString,  
                $"DefaultEndpointsProtocol=https;AccountName={StorageAccountName};AccountKey={StorageAccountKey};EndpointSuffix=core.windows.net",  
                StorageContainerName);  
  
            await eventProcessorHost.RegisterEventProcessorAsync<IoTHubInfluxConnector>();  
  
            Console.WriteLine("Event Processor Host started. Press any key to exit...");  
            Console.ReadKey();  
  
            await eventProcessorHost.UnregisterEventProcessorAsync();  
        }  
    }  
  
    public class IoTHubInfluxConnector : IEventProcessor  
    {  
        public Task CloseAsync(PartitionContext context, CloseReason reason)  
        {  
            Console.WriteLine($"Processor Shutting Down. Partition '{context.PartitionId}', Reason: '{reason}'.");  
            return Task.CompletedTask;  
        }  
  
        public Task OpenAsync(PartitionContext context)  
        {  
            Console.WriteLine($"Event Processor initialized. Partition: '{context.PartitionId}'");  
            return Task.CompletedTask;  
        }  
  
        public Task ProcessErrorAsync(PartitionContext context, Exception error)  
        {  
            Console.WriteLine($"Error on Partition: {context.PartitionId}, Error: {error.Message}");  
            return Task.CompletedTask;  
        }  
  
        public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)  
        {  
            foreach (var eventData in messages)  
            {  
                var data = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);  
                Console.WriteLine($"Message received on Partition '{context.PartitionId}': {data}");  
            }  
  
            return context.CheckpointAsync();  
        }  
    }  
}  

