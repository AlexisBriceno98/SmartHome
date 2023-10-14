using SmartControlPanel.Mvvm.Views;

namespace SmartControlPanel
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(GetStartedPage), typeof(GetStartedPage));
            Routing.RegisterRoute(nameof(DeviceListPage), typeof(DeviceListPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        }
    }
}