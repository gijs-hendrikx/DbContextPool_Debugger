using System.Diagnostics.Metrics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Hosting;

namespace DbContextPoolDebugger;

public sealed class MetricsLoggingService(IDbContextPool<AppDbContext> pool) : BackgroundService
{
    /// <summary>
    /// The EF Core meter name.
    /// </summary>
    private const string EfCoreMeterName = "Microsoft.EntityFrameworkCore";

    /// <summary>
    /// The name of the EF Core active DbContexts instrument.
    /// </summary>
    private const string ActiveDbContextsInstrumentName = "microsoft.entityframeworkcore.active_dbcontexts";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var listener = new MeterListener();

        listener.InstrumentPublished = (instrument, l) =>
        {
            if (instrument.Meter.Name.Equals(EfCoreMeterName, StringComparison.OrdinalIgnoreCase) && instrument.Name.Equals(ActiveDbContextsInstrumentName, StringComparison.OrdinalIgnoreCase))
                listener.EnableMeasurementEvents(instrument);
        };

        listener.SetMeasurementEventCallback<int>((instrument, measurement, _, _) =>
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] active_dbcontexts = {measurement}"));

        listener.SetMeasurementEventCallback<long>((instrument, measurement, _, _) =>
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] active_dbcontexts = {measurement}"));

        listener.Start();

        var countField = pool.GetType()
            .GetField("_count", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            listener.RecordObservableInstruments();
            var poolCount = countField?.GetValue(pool);
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] number of available db contexts in pool = {poolCount}");
        }
    }
}
