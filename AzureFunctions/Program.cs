using AzureFunctions.Contexts;
using Microsoft.Azure.Devices.Client.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedSmartLibrary.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddSingleton<IotHubManager>();
    })
     .ConfigureServices((config, services) =>
     {
         services.AddDbContext<CosmosContext>(x => x.UseCosmos(config.Configuration.GetConnectionString("CosmosDb")!, "AB"));
     })
    .Build();

host.Run();
