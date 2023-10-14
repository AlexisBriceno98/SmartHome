using CommunityToolkit.Mvvm.ComponentModel;
using SharedSmartLibrary;
using SmartControlPanel.Services;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace SmartControlPanel.Mvvm.ViewModels;

public partial class DeviceListViewModel : ObservableObject
{
    private readonly MauiDeviceManager _mauiDeviceManager;
    private readonly DataContext _context;

    [ObservableProperty]
    bool isConfigured;

    [ObservableProperty]
    ObservableCollection<DeviceItemViewModel> devices;

    [RelayCommand]
    public async Task SendDirectMethod(DeviceItemViewModel deviceItem)
    {
        try
        {
            var methodName = string.Empty;

            if (!deviceItem.IsActive)
            {
                deviceItem.IsActive = true;
                methodName = "start";
            }
            else
            {
                deviceItem.IsActive = false;
                methodName = "stop";
            }

            await _mauiDeviceManager.SendDirectMethodAsync(deviceItem.DeviceId, methodName);
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); deviceItem.IsActive = false; }
    }



    public DeviceListViewModel(MauiDeviceManager deviceManager, DataContext context)
    {
        _mauiDeviceManager = deviceManager;
        _context = context;

        IsConfigured = false;

        //		var result = Task.FromResult(GetConnectionStringAsync()).Result;
        //		var connectionstring = result.Result;
        //		if (connectionstring != null)
        IsConfigured = true;

        if (!IsConfigured)
        {
            //Task.Run(() => Shell.Current.GoToAsync(nameof(GetStartedPage)));
            //			Task.Run(() => AddConnectionStringAsync("HostName=kyh-iothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=M/vLVpxoLM7Blwqdsc8YxXaW2A7rQRLjzAIoTFa78jI="));

            IsConfigured = true;
        }



        Devices = new ObservableCollection<DeviceItemViewModel>(_mauiDeviceManager.Devices
            .Select(device => new DeviceItemViewModel(device)).ToList());

        _mauiDeviceManager.DevicesUpdated += UpdateDeviceList;



    }

    private void UpdateDeviceList()
    {
        Devices = new ObservableCollection<DeviceItemViewModel>(_mauiDeviceManager.Devices
            .Select(device => new DeviceItemViewModel(device)).ToList());
    }

    private async Task AddConnectionStringAsync(string connectionString)
    {
        _context.Settings.Add(new SharedSmartLibrary.Entities.SettingsEntity { ConnectionString = connectionString });
        await _context.SaveChangesAsync();
    }

    private async Task<string> GetConnectionStringAsync()
    {
        var result = await _context.Settings.FirstOrDefaultAsync();
        if (result != null)
            return result.ConnectionString;

        return null!;
    }
}
