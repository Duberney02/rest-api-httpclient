using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Movie.Client.Interfaces;
using Movie.Client.Services;

var serviceCollection = new ServiceCollection();

ConfigureServices(serviceCollection);

// create a new ServiceProvider
var serviceProvider = serviceCollection.BuildServiceProvider();

try
{
    await serviceProvider.GetService<IIntegrationService>().Run();
}
catch (Exception ex)
{
    var logger = serviceProvider.GetService<ILogger<Program>>();
    logger.LogError(ex, "Ocurrio un error mientras que corria el integration service");
}

Console.ReadKey();

void ConfigureServices(ServiceCollection serviceCollection)
{
    serviceCollection.AddSingleton(LoggerFactory.Create(builder =>
    {
        builder.AddConsole();
        builder.AddDebug();
    }));


    serviceCollection.AddLogging();

    // register the integration service on our container with a 
    // scoped lifetime

    // For the CRUD demos
    serviceCollection.AddScoped<IIntegrationService, CRUDService>();
}