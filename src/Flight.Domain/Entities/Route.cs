namespace Flight.Domain.Entities;

public class Route
{
    private Route()
    {
    }

    public Route(int id, int originCityId, int destinationCityId, DateOnly departureDate)
    {
        Id = id;
        OriginCityId = originCityId;
        DestinationCityId = destinationCityId;
        DepartureDate = departureDate;
    }

    public int Id { get; private set; }
    public int OriginCityId { get; private set; }
    public int DestinationCityId { get; private set; }
    public DateOnly DepartureDate { get; private set; }
}