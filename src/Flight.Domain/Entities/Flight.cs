namespace Flight.Domain.Entities;

public class Flight
{
    private Flight()
    {
    }

    public Flight(int id, int routeId, DateTime departureTime, DateTime arrivalTime, int airlineId)
    {
        Id = id;
        RouteId = routeId;
        DepartureTime = departureTime;
        ArrivalTime = arrivalTime;
        AirlineId = airlineId;
    }

    public int Id { get; private set; }
    public int RouteId { get; private set; }
    public int AirlineId { get; private set; }
    public DateTime DepartureTime { get; private set; }
    public DateTime ArrivalTime { get; private set; }

    public Route Route { get; private set; } = default!;
}