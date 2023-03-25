# Home Azure IoT platform

## 1. Components

### IoTCreatorsAdapterFunction

This function acts as a receiver (cloud gateway) for forwarded NB IoT messages sent via IoT Creators (Deutsche Telekom). 
The Azure Function contains a HTTP trigger which is able to parse the IoT Creators application server messaging format. 
Assumed input format for HTTP request:
```
{
    "reports":[{
        "serialNumber":"IMEI:XXX",
        "timestamp":1598887180734,
        "subscriptionId":"XXX",
        "resourcePath":"uplinkMsg/0/data",
        "value":"XXX"
    }],
    "registrations":[],
    "deregistrations":[],
    "updates":[],
    "expirations":[],
    "responses":[]
}
```
Assumed value format:
```
{
    temp:10.2,
    hum:24.3
}
```
Output format to IoT Hub:
```
{
    temp:10.2,
    hum:24.3,
    imei:XXX
}
```
Required application settings for Azure Function:
- IoTHubConnectionString -> The device connection string to the IoTHub