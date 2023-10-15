using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Devices;
using SharedSmartLibrary.Models.Devices;
using System.Diagnostics;
using SharedSmartLibrary.Contexts;
using Microsoft.EntityFrameworkCore;

namespace SharedSmartLibrary.Services
{
    public class IotHubManager
    {
        private string _connectionString = string.Empty;
        private System.Timers.Timer timer;
        private bool isConfigured;
        private readonly SmartHomeDbContext _context;
        private RegistryManager? _registryManager;
        private ServiceClient? _serviceClient;

        public IotHubManager(SmartHomeDbContext context)
        {
            _context = context;
            DeviceItemList = new List<DeviceItem>();

            timer = new System.Timers.Timer(5000);
            timer.Elapsed += async (s, e) => await GetAllDevicesAsync();
            timer.Start();
        }

        public List<DeviceItem> DeviceItemList { get; private set; }
        public event Action? DeviceItemListUpdated;


        public void Initialize(string connectionString = null!)
        {
            try
            {
                _connectionString = connectionString;

                if (!isConfigured)
                {
                    if (!string.IsNullOrEmpty(_connectionString))
                    {
                        _registryManager = RegistryManager.CreateFromConnectionString(_connectionString);
                        _serviceClient = ServiceClient.CreateFromConnectionString(_connectionString);
                        isConfigured = true;
                    }
                }
            }
            catch (Exception ex) { Debug.Write(ex.Message); }
        }

        public async Task InitializeAsync(string connectionString = null!)
        {
            try
            {
                if (isConfigured) return;

                if (string.IsNullOrEmpty(connectionString))
                {
                    var settings = await _context.Settings.FirstOrDefaultAsync();
                    if (settings == null) return;

                    connectionString = settings.ConnectionString;
                }

                _registryManager = RegistryManager.CreateFromConnectionString(connectionString);
                _serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
                isConfigured = true;

                await EnsureDeviceIsRegisteredAsync("fan_device");
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
        }


        //public async Task InitializeAsync(string connectionString = null!)
        //{
        //    try
        //    {
        //        if (!isConfigured)
        //        {
        //            if (string.IsNullOrEmpty(connectionString))
        //            {
        //                var settings = await _context.Settings.FirstOrDefaultAsync();
        //                if (settings != null)
        //                {
        //                    _registryManager = RegistryManager.CreateFromConnectionString(settings.ConnectionString);
        //                    _serviceClient = ServiceClient.CreateFromConnectionString(settings.ConnectionString);
        //                    isConfigured = true;
        //                }
        //            }
        //            else
        //            {
        //                _registryManager = RegistryManager.CreateFromConnectionString(connectionString);
        //                _serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
        //                isConfigured = true;
        //            }
        //        }
        //    }
        //    catch (Exception ex) { Debug.Write(ex.Message); }
        //}

        private async Task GetAllDevicesAsync()
        {
            try
            {
                if (isConfigured)
                {
                    var list_updated = false;
                    var twins = new List<Twin>();
                    var result = _registryManager!.CreateQuery("select * from devices");

                    foreach (var item in await result.GetNextAsTwinAsync())
                        twins.Add(item);

                    foreach (var device in twins)
                        if (!DeviceItemList.Any(x => x.DeviceId == device.DeviceId))
                        {
                            DeviceItemList.Add(new DeviceItem { DeviceId = device.DeviceId });
                            list_updated = true;
                        }

                    for (int i = DeviceItemList.Count - 1; i >= 0; i--)
                    {
                        if (!twins.Any(x => x.DeviceId == DeviceItemList[i].DeviceId))
                        {
                            DeviceItemList.RemoveAt(i);
                            list_updated = true;
                        }
                    }

                    if (list_updated)
                        DeviceItemListUpdated!.Invoke();

                }
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
        }

        public async Task<Device> GetDeviceAsync(string deviceId)
        {
            try
            {
                var device = await _registryManager!.GetDeviceAsync(deviceId);
                if (device != null)
                    return device;
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }

            return null!;
        }

        public async Task<Device> RegisterDeviceAsync(string deviceId)
        {
            try
            {
                var device = await _registryManager!.AddDeviceAsync(new Device(deviceId));
                if (device != null)
                    return device;
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }

            return null!;
        }

        public string GenerateConnectionString(Device device)
        {
            try
            {
                return $"{_connectionString.Split(";")[0]};DeviceId={device.Id};SharedAccessKey={device.Authentication.SymmetricKey.PrimaryKey}";
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }

            return null!;
        }

        public async Task EnsureDeviceIsRegisteredAsync(string deviceId)
        {
            var device = await GetDeviceAsync(deviceId);
            if (device == null)
            {
                await RegisterDeviceAsync(deviceId);
            }
        }

    }
}
