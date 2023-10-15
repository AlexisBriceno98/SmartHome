using CommunityToolkit.Maui;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedSmartLibrary;
using SharedSmartLibrary.Contexts;
using SharedSmartLibrary.Services;
using SmartControlPanel.Mvvm.ViewModels;
using SmartControlPanel.Mvvm.Views;
using SmartControlPanel.Services;

namespace SmartControlPanel
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("fa-brands-400.ttf", "FontAwesomeBrands");
                    fonts.AddFont("fa-duotone-900.ttf", "FontAwesomeDuoTone");
                    fonts.AddFont("fa-light-300.ttf", "FontAwesomeLight");
                    fonts.AddFont("fa-regular-400.ttf", "FontAwesomeRegular");
                    fonts.AddFont("fa-sharp-light-300.ttf", "FontAwesomeSharpLight");
                    fonts.AddFont("fa-sharp-regular-400.ttf", "FontAwesomeSharpRegular");
                    fonts.AddFont("fa-sharp-solid-900.ttf", "FontAwesome-SharpSolid");
                    fonts.AddFont("fa-solid-900.ttf", "FontAwesomeSolid");
                    fonts.AddFont("fa-thin-100.ttf", "FontAwesomeThin");
                });

            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<MainPage>();

            builder.Services.AddSingleton<DeviceItemViewModel>();

            builder.Services.AddSingleton<HomeViewModel>();
            builder.Services.AddSingleton<HomePage>();

            builder.Services.AddSingleton<GetStartedViewModel>();
            builder.Services.AddSingleton<GetStartedPage>();

            builder.Services.AddSingleton<DeviceListViewModel>();
            builder.Services.AddSingleton<DeviceListPage>();

            builder.Services.AddSingleton<SettingsViewModel>();
            builder.Services.AddSingleton<SettingsPage>();

            builder.Services.AddSingleton<DeviceManager>();
            builder.Services.AddSingleton<MauiDeviceManager>();
            builder.Services.AddDbContext<SmartHomeDbContext>(x => x.UseSqlite($"Data Source={DatabasePathFinder.GetPath()}", x => x.MigrationsAssembly(nameof(SharedSmartLibrary))));
            builder.Services.AddSingleton<IotHubService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}