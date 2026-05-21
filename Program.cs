using DbContextPoolDebugger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddPooledDbContextFactory<AppDbContext>(
            options => options.UseInMemoryDatabase("DebugDb"),
            poolSize: 20);

        services.AddHostedService<MetricsLoggingService>();
        services.AddHostedService<ContextLeakService>();
    })
    .Build();

await host.RunAsync();
