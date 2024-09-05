using Flight.Application.Options;
using Flight.Application.Queries.DetectFlightChanges;
using Flight.Application.Queries.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Flight.Infrastructure.FileSystem;

public class FlightChangeDetectionResultFileStorage(IOptions<FlightOptions> flightOptions, ILogger<FlightChangeDetectionResultFileStorage> logger)
    : IFlightChangeDetectionResultStorage
{
    public async Task Store(List<DetectFlightChangesResult> items)
    {
        // Create a new file if not exist and overwrite it if exists
        await using var writer = new StreamWriter(flightOptions.Value.DetectedChangesResultFilePath);
        await writer.WriteLineAsync("flight_id,origin_city_id,destination_city_id,departure_time,arrival_time,airline_id,status");

        foreach (var item in items)
        {
            await writer.WriteLineAsync(
                $"{item.FlightId},{item.OriginCityId},{item.DestinationCityId},{item.DepartureTime:yyyy-MM-dd HH:mm:ss},{item.ArrivalTime:yyyy-MM-dd HH:mm:ss},{item.AirlineId},{item.Status}");
        }

        logger.LogInformation("Result of the change detection algorithm is saved to {File}", flightOptions.Value.DetectedChangesResultFilePath);
    }
}