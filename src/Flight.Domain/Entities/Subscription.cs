namespace Flight.Domain.Entities;

public class Subscription
{
    private Subscription()
    {
    }

    public Subscription(byte agencyId, int originCityId, int destinationCityId)
    {
        AgencyId = agencyId;
        OriginCityId = originCityId;
        DestinationCityId = destinationCityId;
    }

    public byte AgencyId { get; private set; }
    public int OriginCityId { get; private set; }
    public int DestinationCityId { get; private set; }
}