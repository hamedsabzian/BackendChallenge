using Flight.Application.Queries.DetectFlightChanges;
using Flight.Application.Queries.Interfaces;

namespace Flight.Application.Queries;

public class FlightServiceStoreResultDecorator(FlightServiceMetricProxy serviceMetric, IFlightChangeDetectionResultStorage resultStorage)
    : IFlightService
{
    public async Task<List<DetectFlightChangesResult>> DetectChangesForAgency(DetectFlightChangesQuery query)
    {
        var result = await serviceMetric.DetectChangesForAgency(query);
        await resultStorage.Store(result);
        return result;
    }
}