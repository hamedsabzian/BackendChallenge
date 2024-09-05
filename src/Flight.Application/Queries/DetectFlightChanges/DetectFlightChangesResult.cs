namespace Flight.Application.Queries.DetectFlightChanges;

public record DetectFlightChangesResult(
    int FlightId,
    int OriginCityId,
    int DestinationCityId,
    DateTime DepartureTime,
    DateTime ArrivalTime,
    int AirlineId,
    FlightChangeStatus Status);