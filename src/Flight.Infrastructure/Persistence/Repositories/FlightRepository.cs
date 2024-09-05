using Flight.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Flight.Infrastructure.Persistence.Repositories;

public class FlightRepository(FlightDbContext dbContext) : IFlightRepository
{
    public Task<List<Domain.Entities.Flight>> GetInterestedAgencyFlights(int agencyId, DateTime startDate, DateTime endDate)
    {
        var query = from subscription in dbContext.Subscriptions
            join route in dbContext.Routes
                on new { subscription.OriginCityId, subscription.DestinationCityId } equals new { route.OriginCityId, route.DestinationCityId }
            join flight in dbContext.Flights
                on route.Id equals flight.RouteId
            where subscription.AgencyId == agencyId && startDate <= flight.DepartureTime && flight.DepartureTime <= endDate
            select flight;

        return query.Include(f => f.Route).AsNoTracking().ToListAsync();
    }
}