using System.Collections.Concurrent;
using Flight.Application.Options;
using Flight.Application.Queries.DetectFlightChanges;
using Flight.Application.Queries.Interfaces;
using Flight.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Flight.Application.Queries;

public class FlightService(IFlightRepository flightRepository, IOptions<FlightOptions> flightOptions, ILogger<FlightService> logger) : IFlightService
{
    private readonly FlightOptions _options = flightOptions.Value;
    private const int MaxDegreeOfParallelism = 4;

    public async Task<List<DetectFlightChangesResult>> DetectChangesForAgency(DetectFlightChangesQuery query)
    {
        var detectedFlights = new ConcurrentBag<DetectFlightChangesResult>();

        var startDateIncludeHistory = query.StartDate.AddDays(-_options.PeriodDays).AddMinutes(-_options.PeriodToleranceMinutes);
        var endDateIncludeNextPlan = query.EndDate.AddDays(_options.PeriodDays).AddMinutes(_options.PeriodToleranceMinutes);

        logger.LogInformation(
            "The interested flights of {Agency} between {StartDate:s} and {EndDate:s} (with history and next plan) should be loaded",
            query.AgencyId,
            query.StartDate, query.EndDate);

        var flights = await flightRepository.GetInterestedAgencyFlights(query.AgencyId, startDateIncludeHistory, endDateIncludeNextPlan);

        logger.LogInformation("{Count} flights are fetched to detecting the changes", flights.Count);

        var indexedFlights = flights.GroupBy(f => $"{f.AirlineId}_{f.Route.OriginCityId}_{f.Route.DestinationCityId}")
            .ToDictionary(g => g.Key, g => g.ToList());
        var inspectedFlights = flights.Where(f => query.StartDate <= f.DepartureTime && f.DepartureTime <= query.EndDate).ToList();

        Parallel.ForEach(inspectedFlights, new ParallelOptions { MaxDegreeOfParallelism = MaxDegreeOfParallelism }, inspected =>
        {
            var sameFlights = indexedFlights[$"{inspected.AirlineId}_{inspected.Route.OriginCityId}_{inspected.Route.DestinationCityId}"];

            if (IsNewFlight(inspected, sameFlights))
            {
                detectedFlights.Add(MapToResult(inspected, FlightChangeStatus.New));
            }

            if (IsDiscontinuedFlight(inspected, sameFlights))
            {
                detectedFlights.Add(MapToResult(inspected, FlightChangeStatus.Discontinued));
            }
        });

        return detectedFlights.ToList();
    }

    private static DetectFlightChangesResult MapToResult(Domain.Entities.Flight flight, FlightChangeStatus status)
    {
        return new DetectFlightChangesResult(
            flight.Id,
            flight.Route.OriginCityId,
            flight.Route.DestinationCityId,
            flight.DepartureTime,
            flight.ArrivalTime,
            flight.AirlineId,
            status);
    }

    private bool IsNewFlight(Domain.Entities.Flight inspected, List<Domain.Entities.Flight> airlineFlights)
    {
        var (start, end) = ToleranceRange(inspected.DepartureTime, -_options.PeriodDays);

        return !airlineFlights.Any(f => start <= f.DepartureTime && f.DepartureTime <= end);
    }

    private bool IsDiscontinuedFlight(Domain.Entities.Flight inspected, List<Domain.Entities.Flight> airlineFlights)
    {
        var (start, end) = ToleranceRange(inspected.DepartureTime, _options.PeriodDays);

        return !airlineFlights.Any(f => start <= f.DepartureTime && f.DepartureTime <= end);
    }

    private (DateTime Start, DateTime End) ToleranceRange(DateTime date, int days) =>
        (date.AddDays(days).AddMinutes(-_options.PeriodToleranceMinutes),
            date.AddDays(days).AddMinutes(_options.PeriodToleranceMinutes));
}