using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using SmartControlPanel.Mvvm.Views;
using System.Diagnostics;
using System.Timers;

namespace SmartControlPanel.Mvvm.ViewModels
{
    public partial class GetStartedViewModel : ObservableObject
    {

        public GetStartedViewModel()
        {
            UpdateDateTimeProperties();

            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += (s, e) => UpdateDateTimeProperties();
            _timer.Start();

            _weatherTimer = new System.Timers.Timer(TimeSpan.FromMinutes(15).TotalMilliseconds);
            _weatherTimer.Elapsed += async (s, e) => await GetWeatherAsync();
            _weatherTimer.Start();

            GetWeatherAsync();
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
            using HttpClient http = new HttpClient();
            try
            {
                var data = JsonConvert.DeserializeObject<dynamic>(await http.GetStringAsync("https://api.openweathermap.org/data/2.5/weather?lat=59.1875&lon=18.1232&appid=b4a3119e986341f8f3a4d159c5787679"));
                Temperature = (data!.main.temp - 273.15).ToString("0");

                switch (data!.weather[0].description.ToString())
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
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to get weather data. Error: {ex.Message}");
                Temperature = "--";
            }
        }
    }
}
