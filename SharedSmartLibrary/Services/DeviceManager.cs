﻿using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using SharedSmartLibrary.Models.Devices;
using System.Text;
using Microsoft.Azure.Devices.Shared;

namespace SharedSmartLibrary.Services;

public class DeviceManager
{
    private readonly DeviceConfiguration _config;
    private readonly DeviceClient _client;

    public DeviceManager(DeviceConfiguration config)
    {
        _config = config;
        _client = DeviceClient.CreateFromConnectionString(_config.ConnectionString);
        Task.FromResult(StartAsync());
    }

    public bool AllowSending() => _config.AllowSending;


    private async Task StartAsync()
    {
        await _client.SetMethodDefaultHandlerAsync(DirectMethodDefaultCallback, null);
    }

    private async Task<MethodResponse> DirectMethodDefaultCallback(MethodRequest req, object userContext)
    {
        var res = new DirectMethodDataResponse { Message = $"Executed method: {req.Name} successfully." };

        switch (req.Name.ToLower())
        {
            case "start":
                _config.AllowSending = true;
                break;

            case "stop":
                _config.AllowSending = false;
                break;

            default:
                res.Message = $"Direct method {req.Name} not found.";
                return new MethodResponse(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(res)), 404);
        }

        await ReportFanPowerStatusAsync();
        return new MethodResponse(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(res)), 200);
    }

    public async Task ReportFanPowerStatusAsync()
    {
        var twinProperties = new TwinCollection();
        twinProperties["power"] = _config.AllowSending ? "start" : "stop";
        await _client.UpdateReportedPropertiesAsync(twinProperties);
    }

    public async Task<string> GetFanPowerStatusAsync()
    {
        var twin = await _client.GetTwinAsync();
        return twin.Properties.Reported["power"];
    }

    public async Task SendMessageAsync(string content)
    {
        var message = new Message(Encoding.UTF8.GetBytes(content));
        await _client!.SendEventAsync(message);
    }
}
