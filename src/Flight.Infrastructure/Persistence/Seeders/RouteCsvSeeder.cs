using Flight.Domain.Entities;

namespace Flight.Infrastructure.Persistence.Seeders;

public class RouteCsvSeeder : CsvSeederBase<Route>
{
    protected override Route Map(string[] lineParts) =>
        new(int.Parse(lineParts[0]), int.Parse(lineParts[1]), int.Parse(lineParts[2]), DateOnly.Parse(lineParts[3]));
}