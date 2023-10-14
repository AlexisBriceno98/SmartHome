using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartControlPanel.Mvvm.Views;
using System.Timers;

namespace SmartControlPanel.Mvvm.ViewModels
{
    public partial class GetStartedViewModel : ObservableObject
    {
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

        public GetStartedViewModel()
        {
            UpdateDateTimeProperties();

            _timer = new System.Timers.Timer(1000); 
            _timer.Elapsed += (s, e) => UpdateDateTimeProperties();
            _timer.Start();
        }

        private void UpdateDateTimeProperties()
        {
            CurrentTime = DateTime.Now.ToString("HH:mm:ss");
            CurrentDate = DateTime.Now.ToString("dddd, d MMMM yyyy");
        }
    }
}
