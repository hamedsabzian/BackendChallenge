using Flight.Application.Queries.DetectFlightChanges;

namespace Flight.Application.Queries.Interfaces;

public interface IFlightChangeDetectionResultStorage
{
    Task Store(List<DetectFlightChangesResult> items);
}