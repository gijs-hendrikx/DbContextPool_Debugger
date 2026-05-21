using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace DbContextPoolDebugger;

public sealed class ContextLeakService(IDbContextFactory<AppDbContext> factory) : BackgroundService
{
    private const int ContextsToRequest = 25; // deliberately exceeds the pool size of 20

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Requesting {ContextsToRequest} DbContext instances...");

            var contexts = new AppDbContext[ContextsToRequest];
            for (int i = 0; i < ContextsToRequest; i++)
                contexts[i] = factory.CreateDbContext();

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] All {ContextsToRequest} contexts acquired. Disposing...");

            for (int i = 0;i < contexts.Length;i++)
            {
                // ##############
                // ENABLE THESE LINES TO SIMULATE A LEAK
                // THIS DOES NOT EXHAUST THE POOL, EVEN THOUGH THE NUMBER OF NON-DISPOSED CONTEXTS EXCEEDS THE POOL SIZE AFTER A WHILE
                // ##############
                //if (i < 10)
                //    continue; // skip disposing the first context to simulate an actual leak, slowly exhausting the pool over time

                contexts[i].Dispose();
            }
                

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] All contexts disposed.");

            await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
        }
    }
}
