using Flight.Application;
using Flight.Application.Queries.DetectFlightChanges;
using Flight.Application.Queries.Interfaces;
using Flight.Infrastructure;
using Flight.Infrastructure.Persistence;
using Flight.Infrastructure.Persistence.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

if (args.Length != 3 ||
    !DateTime.TryParse(args[0], out var startTime) ||
    !DateTime.TryParse(args[1], out var endTime) ||
    !int.TryParse(args[2], out var agencyId) ||
    startTime > endTime)
{
    Console.WriteLine("The correct parameters (Start Date, End Date, Agency ID) is not provided.");
    return 1;
}

var configuration = ProvideConfiguration();

var services = new ServiceCollection()
    .AddSingleton(configuration)
    .AddLogging(loggingOptions =>
    {
        loggingOptions.AddConfiguration(configuration.GetSection("Logging"))
            .AddSimpleConsole(options => { options.TimestampFormat = "[HH:mm:ss] "; });
    })
    .AddApplicationDependencies()
    .AddInfrastructureDependencies(configuration);

var serviceProvider = services.BuildServiceProvider();

var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

await InitialDatabase();

// Using a seperated scope than data seeding 
using var dependencyScope = serviceProvider.CreateScope();
var flightService = dependencyScope.ServiceProvider.GetRequiredService<IFlightService>();
await flightService.DetectChangesForAgency(new DetectFlightChangesQuery(agencyId, startTime, endTime));

return 0;

IConfigurationRoot ProvideConfiguration()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false);
    return builder.Build();
}

async ValueTask InitialDatabase()
{
    using var migrationScope = serviceProvider.CreateScope();
    var dbContext = migrationScope.ServiceProvider.GetRequiredService<FlightDbContext>();
    dbContext.Database.Migrate();

    if (!dbContext.Subscriptions.Any())
    {
        var rootPath = configuration["InitialDataPath"]!;
        logger.LogInformation("The subscriptions are importing...");
        await new SubscriptionCsvSeeder().Seed(Path.Combine(rootPath, "subscriptions.csv"), dbContext);
        logger.LogInformation("The subscriptions are imported successfully");

        logger.LogInformation("The routes are importing...");
        await new RouteCsvSeeder().Seed(Path.Combine(rootPath, "routes.csv"), dbContext);
        logger.LogInformation("The routes are imported successfully");

        logger.LogInformation("The flights are importing...");
        await new FlightCsvSeeder().Seed(Path.Combine(rootPath, "flights.csv"), dbContext);
        logger.LogInformation("The flights are imported successfully");
    }
    else
    {
        logger.LogInformation("Data has already been imported");
    }
}