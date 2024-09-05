using Flight.Application.Options;
using Flight.Application.Queries.Interfaces;
using Flight.Domain.Interfaces;
using Flight.Infrastructure.FileSystem;
using Flight.Infrastructure.Persistence;
using Flight.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flight.Infrastructure;

public static class DependencyExtensions
{
    public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FlightOptions>(configuration.GetSection("Flight"));

        services.AddDbContext<FlightDbContext>(builder =>
        {
            builder.UseSqlite(configuration.GetConnectionString("Sqlite"));
        });
        services.AddScoped<IFlightRepository, FlightRepository>();
        services.AddScoped<IFlightChangeDetectionResultStorage, FlightChangeDetectionResultFileStorage>();
        return services;
    }
}