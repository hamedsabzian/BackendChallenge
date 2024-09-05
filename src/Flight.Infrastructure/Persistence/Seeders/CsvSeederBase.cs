using EFCore.BulkExtensions;

namespace Flight.Infrastructure.Persistence.Seeders;

public abstract class CsvSeederBase<T> where T : class
{
    const int PersistenceThreshold = 100000;

    public async Task Seed(string file, FlightDbContext dbContext)
    {
        using var reader = new StreamReader(file);
        int counter = 0;
        var bucket = new List<T>();
        var line = await reader.ReadLineAsync();
        while ((line = await reader.ReadLineAsync()) is not null)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var lineParts = line.Split(',');
            bucket.Add(Map(lineParts));

            counter++;
            if (counter % PersistenceThreshold == 0)
            {
                await dbContext.BulkInsertAsync(bucket);
                bucket.Clear();
            }
        }

        await dbContext.BulkInsertAsync(bucket);
        bucket.Clear();
    }

    protected abstract T Map(string[] lineParts);
}