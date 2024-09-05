using Flight.Application.Queries.DetectFlightChanges;

namespace Flight.Application.Queries.Interfaces;

public interface IFlightService
{
    Task<List<DetectFlightChangesResult>> DetectChangesForAgency(DetectFlightChangesQuery query);
}