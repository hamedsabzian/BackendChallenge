using System.Diagnostics;
using Flight.Application.Queries.DetectFlightChanges;
using Flight.Application.Queries.Interfaces;
using Microsoft.Extensions.Logging;

namespace Flight.Application.Queries;

public class FlightServiceMetricProxy(FlightService service, ILogger<FlightServiceMetricProxy> logger) : IFlightService
{
    public async Task<List<DetectFlightChangesResult>> DetectChangesForAgency(DetectFlightChangesQuery query)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var result = await service.DetectChangesForAgency(query);

        stopwatch.Stop();
        logger.LogInformation(
            "{Count} changes are found in {ElapsedTime} seconds. Unique flights: {UniqueCount}, New: {NewCount}, Discontinued: {DiscontinuedCount}",
            result.Count,
            stopwatch.Elapsed.TotalSeconds,
            result.Select(a => a.FlightId).Distinct().Count(),
            result.Count(a => a.Status == FlightChangeStatus.New),
            result.Count(a => a.Status == FlightChangeStatus.Discontinued));

        return result;
    }
}