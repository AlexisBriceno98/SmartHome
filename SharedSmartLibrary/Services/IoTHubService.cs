using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using System.Diagnostics;
using System.Text;

namespace SharedSmartLibrary.Services;

public class IotHubService
{
    private readonly DeviceClient _deviceClient;
    private readonly ServiceClient _serviceClient;
    public IotHubService()
    {
        _deviceClient = DeviceClient.CreateFromConnectionString("HostName=alexis-iothub.azure-devices.net;DeviceId=fan_device;SharedAccessKey=9QMPvRdE/dRMjEDkrz2F7qGEmYEgX/+tvAIoTMbgwEs=");
        _serviceClient = ServiceClient.CreateFromConnectionString("HostName=alexis-iothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=c1zz9Sh4y567gvBU+EUvTKXV8ktuO+nfaAIoTPv14LE=");
    }
    public async Task SendMessageAsync(string messageString)
    {
        try
        {
            var message = new Microsoft.Azure.Devices.Client.Message(Encoding.UTF8.GetBytes(messageString));
            await _deviceClient.SendEventAsync(message);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    public async Task ReceiveMessageAsync()
    {
        while (true)
        {
            try
            {
                var receivedMessage = await _deviceClient.ReceiveAsync();
                if (receivedMessage != null)
                {

                    string messageData = Encoding.UTF8.GetString(receivedMessage.GetBytes());
                    Console.WriteLine("Received message: {0}", messageData);
                    await _deviceClient.CompleteAsync(receivedMessage);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }

    public async Task SendCommandAsync(string deviceId, string commandName)
    {
        var methodInvocation = new CloudToDeviceMethod(commandName) { ResponseTimeout = TimeSpan.FromSeconds(30) };
        await _serviceClient.InvokeDeviceMethodAsync(deviceId, methodInvocation);

    }
}

