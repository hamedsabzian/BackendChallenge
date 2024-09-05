namespace Flight.Infrastructure.Persistence.Seeders;

public class FlightCsvSeeder : CsvSeederBase<Domain.Entities.Flight>
{
    protected override Domain.Entities.Flight Map(string[] lineParts) =>
        new(int.Parse(lineParts[0]), int.Parse(lineParts[1]), DateTime.Parse(lineParts[2]), DateTime.Parse(lineParts[3]), int.Parse(lineParts[4]));
}