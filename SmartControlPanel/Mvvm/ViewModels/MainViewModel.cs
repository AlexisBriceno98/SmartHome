using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SharedSmartLibrary.Services;
using SharedSmartLibrary;
using System.Collections.ObjectModel;
using SmartControlPanel.Services;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using SmartControlPanel.Mvvm.Views;
using SharedSmartLibrary.Contexts;

namespace SmartControlPanel.Mvvm.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly MauiDeviceManager _mauiDeviceManager;
    private readonly SmartHomeDbContext _context;

    [ObservableProperty]
    bool isConfigured;

    [ObservableProperty]
    ObservableCollection<DeviceItemViewModel> devices;


    public MainViewModel(MauiDeviceManager deviceManager, SmartHomeDbContext context)
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


    private void UpdateDeviceList()
    {
        Devices = new ObservableCollection<DeviceItemViewModel>(_mauiDeviceManager.Devices
            .Select(device => new DeviceItemViewModel(device)).ToList());
    }

    private async Task AddConnectionStringAsync(string connectionString)
    {
        _context.Settings.Add(new SharedSmartLibrary.Entities.SmartAppSettings{ ConnectionString = connectionString });
        await _context.SaveChangesAsync();
    }

    private async Task<string> GetConnectionStringAsync()
    {
        var result = await _context.Settings.FirstOrDefaultAsync();
        if (result != null)
            return result.ConnectionString;

        return null!;
    }

    //private async Task CheckConfigurationAsync()
    //{
    //    try
    //    {
    //        if (await _context.Settings.AnyAsync())
    //        {
    //            await _context.InitializeAsync();
    //            await Shell.Current.GoToAsync(nameof(OverviewPage));
    //        }

    //    }
    //    catch (Exception ex) { Debug.WriteLine(ex.Message); }
    //}

    [RelayCommand]
    async Task GoToGetStarted()
    {
        await Shell.Current.GoToAsync(nameof(GetStartedPage));
    }

    [RelayCommand]
    async Task GoToSettings()
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }
}