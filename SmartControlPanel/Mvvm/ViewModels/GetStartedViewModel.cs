using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using SharedSmartLibrary.Services;
using SmartControlPanel.Mvvm.Views;
using SmartControlPanel.Services;
using System.Diagnostics;
using System.Timers;
using System.Windows.Input;

namespace SmartControlPanel.Mvvm.ViewModels
{
    public partial class GetStartedViewModel : ObservableObject
    {

        private readonly MauiDeviceManager _mauiDeviceManager;

        public GetStartedViewModel(MauiDeviceManager mauiDeviceManager)
        {
            _mauiDeviceManager = mauiDeviceManager;
            UpdateDateTimeProperties();

            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += (s, e) => UpdateDateTimeProperties();
            _timer.Start();

            _weatherTimer = new System.Timers.Timer(TimeSpan.FromMinutes(15).TotalMilliseconds);
            _weatherTimer.Elapsed += async (s, e) => await GetWeatherAsync();
            _weatherTimer.Start();

            GetWeatherAsync().ConfigureAwait(true);
        }

        [RelayCommand]
        async Task GoToDeviceList()
        {
            await Shell.Current.GoToAsync(nameof(DeviceListPage));
        }

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        async Task GoToSettings()
        {
            await Shell.Current.GoToAsync(nameof(SettingsPage));
        }

        private System.Timers.Timer _timer;
        private string _currentTime;
        private string _currentDate;

        public string CurrentTime
        {
            get => _currentTime;
            set => SetProperty(ref _currentTime, value);
        }

        public string CurrentDate
        {
            get => _currentDate;
            set => SetProperty(ref _currentDate, value);
        }

        private void UpdateDateTimeProperties()
        {
            CurrentTime = DateTime.Now.ToString("HH:mm:ss");
            CurrentDate = DateTime.Now.ToString("dddd, d MMMM yyyy");
        }

        private string _temperature;
        private string _condition;

        public string Temperature
        {
            get => _temperature;
            set => SetProperty(ref _temperature, value);
        }

        public string Condition
        {
            get => _condition;
            set => SetProperty(ref _condition, value);
        }

        private System.Timers.Timer _weatherTimer;


        private async Task GetWeatherAsync()
        {
            using HttpClient client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://open-weather13.p.rapidapi.com/city/stockholm"),
                Headers =
        {
            { "X-RapidAPI-Key", "d1d840e348msh27e4462a1257a95p1b370djsn1a89298b312e" },
            { "X-RapidAPI-Host", "open-weather13.p.rapidapi.com" },
        },
            };

            try
            {
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();

                    var data = JsonConvert.DeserializeObject<dynamic>(body);

                    // Assuming "data" has properties for temperature and condition description.
                    Temperature = ((data!.main.temp - 32)/1.8).ToString("0");
                    var weatherDescription = data?.weather[0]?.description?.ToString();

                    switch (weatherDescription)
                    {
                        case "clear sky":
                            Condition = "\ue28f";
                            break;
                        case "few clouds":
                            Condition = "\uf6c4";
                            break;
                        case "overcast clouds":
                            Condition = "\uf744";
                            break;
                        case "scattered clouds":
                            Condition = "\uf0c2";
                            break;
                        case "broken clouds":
                            Condition = "\uf744";
                            break;
                        case "shower rain":
                            Condition = "\uf738";
                            break;
                        case "rain":
                            Condition = "\uf740";
                            break;
                        case "thunderstorm":
                            Condition = "\uf76c";
                            break;
                        case "snow":
                            Condition = "\uf742";
                            break;
                        case "mist":
                            Condition = "\uf74e";
                            break;
                        default:
                            Condition = "\ue137";
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to get weather data. Error: {ex.Message}");
                Temperature = "--";
                Condition = "\ue137";  // default icon for error scenarios
            }
        }


        private string _fanConnectionStatusText;
        public string FanConnectionStatusText
        {
            get => _fanConnectionStatusText;
            set => SetProperty(ref _fanConnectionStatusText, value);
        }

        private bool _isFanConnectionStatusVisible;
        public bool IsFanConnectionStatusVisible
        {
            get => _isFanConnectionStatusVisible;
            set => SetProperty(ref _isFanConnectionStatusVisible, value);
        }

        private bool _isFanConnected;
        public bool IsFanConnected
        {
            get => _isFanConnected;
            set => SetProperty(ref _isFanConnected, value);
        }

        public ICommand ToggleFanStateCommand { get; private set; }

        public async void ToggleFanState(ToggledEventArgs e)
        {
            bool isToggled = e.Value;
            var deviceId = "fan_device";
            string methodName = isToggled ? "start" : "stop";
            try
            {
                await _mauiDeviceManager.SendDirectMethodAsync(deviceId, methodName);
                IsFanConnected = isToggled;
            }
            catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
            {
                IsFanConnected = false;
                FanConnectionStatusText = "Device Not Connected";
                IsFanConnectionStatusVisible = true;

                await Task.Delay(3000);

                IsFanConnectionStatusVisible = false;
            }
        }
    }
}
