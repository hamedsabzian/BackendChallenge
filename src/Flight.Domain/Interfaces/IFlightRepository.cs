namespace Flight.Domain.Interfaces;

public interface IFlightRepository
{
    Task<List<Entities.Flight>> GetInterestedAgencyFlights(int agencyId, DateTime startDate, DateTime endDate);
}