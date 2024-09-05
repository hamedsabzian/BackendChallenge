namespace Flight.Application.Queries.DetectFlightChanges;

public record DetectFlightChangesQuery(int AgencyId, DateTime StartDate, DateTime EndDate);