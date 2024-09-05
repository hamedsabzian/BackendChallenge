using Flight.Domain.Entities;

namespace Flight.Infrastructure.Persistence.Seeders;

public class SubscriptionCsvSeeder : CsvSeederBase<Subscription>
{
    protected override Subscription Map(string[] lineParts) => new(byte.Parse(lineParts[0]), int.Parse(lineParts[1]), int.Parse(lineParts[2]));
}