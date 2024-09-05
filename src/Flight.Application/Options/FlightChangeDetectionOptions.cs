namespace Flight.Application.Options;

public class FlightOptions
{
    public int PeriodDays { get; set; }
    public int PeriodToleranceMinutes { get; set; }
    public string DetectedChangesResultFilePath { get; set; } = default!;
    public string InitialDataPath { get; set; } = default!;
}