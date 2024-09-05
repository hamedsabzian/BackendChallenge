using Flight.Application.Queries;
using Flight.Application.Queries.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Flight.Application;

public static class DependencyExtensions
{
    public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
    {
        services.AddScoped<FlightService>();
        services.AddScoped<FlightServiceMetricProxy>();
        services.AddScoped<IFlightService, FlightServiceStoreResultDecorator>();

        return services;
    }
}