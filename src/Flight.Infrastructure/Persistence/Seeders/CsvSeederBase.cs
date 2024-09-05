using EFCore.BulkExtensions;
using Microsoft.Extensions.Logging;

namespace Flight.Infrastructure.Persistence.Seeders;

public abstract class CsvSeederBase<T> where T : class
{
    const int PersistenceThreshold = 100000;

    public async Task Seed(string file, FlightDbContext dbContext, ILogger logger)
    {
        logger.LogInformation("The \"{File}\" is importing...", file);
        if (!File.Exists(file))
        {
            logger.LogInformation("The \"{File}\" not found", file);
            return;
        }

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

        logger.LogInformation("The \"{File}\" is imported successfully", file);
    }

    protected abstract T Map(string[] lineParts);
}