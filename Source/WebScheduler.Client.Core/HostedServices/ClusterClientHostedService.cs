namespace WebScheduler.Client.Core.HostedServices;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using WebScheduler.Client.Core.Options;

/// <summary>
/// Manages the <see cref="IClusterClient"/>.
/// </summary>
public class ClusterClientHostedService : IHostedService, IAsyncDisposable, IDisposable
{
    private readonly ILogger<ClusterClientHostedService> logger;
    private readonly WebSchedulerClientConfigurationOptions options;

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="loggerFactory">logger factory</param>
    /// <param name="options">options</param>
    /// <param name="clientBuilder">client builder</param>
    public ClusterClientHostedService(ILoggerFactory loggerFactory, IOptions<WebSchedulerClientConfigurationOptions> options, IClientBuilder clientBuilder)
    {
        this.logger = loggerFactory.CreateLogger<ClusterClientHostedService>();
        this.options = options.Value;

        this.Client = clientBuilder
            .ConfigureServices(services =>
            {
                // Add logging from the host's container.
                _ = services.AddSingleton(loggerFactory);
                _ = services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            })
            // .AddClusterConnectionLostHandler(async (object sender, EventArgs e) => await this.ConnectionHandler().)
            .UseAdoNetClustering(options =>
            {
                options.Invariant = this.options.Storage.Invariant;
                options.ConnectionString = this.options.Storage.ConnectionString;
            })
           .Configure<ClusterOptions>(options =>
           {
               options.ClusterId = this.options.Cluster.ClusterId;
               options.ServiceId = this.options.Cluster.ServiceId;
           })
           .ConfigureApplicationParts(
               parts => parts
                   .AddApplicationPart(typeof(Abstractions.Grains.Scheduler.IScheduledTaskGrain).Assembly)
                   .WithReferences())
            .Configure<StatisticsOptions>(options => options.LogWriteInterval = TimeSpan.FromMilliseconds(-1)) // Disable Statistics output to logging from Orleans

            .Build();
    }

    /// <summary>
    /// The client.
    /// </summary>
    public IClusterClient Client { get; }

    /// <inheritdoc/>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var attempt = 0;
        const int maxAttempts = 100;
        var delay = TimeSpan.FromSeconds(1);
        return this.Client.Connect(async error =>
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            if (++attempt < maxAttempts)
            {
                this.logger.FailedToConnectToOrleansCluster(error, attempt, maxAttempts);

                try
                {
                    await Task.Delay(delay, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return false;
                }

                return true;
            }

            this.logger.FailedToConnectToOrleansCluster(error, attempt, maxAttempts);

            return false;
        });
    }

    /// <inheritdoc/>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        this.logger.ShuttingDownSiloGracefully();
        await this.Client.Close();
        var cancellation = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        _ = cancellationToken.Register(() => cancellation.TrySetCanceled(cancellationToken));
        _ = await Task.WhenAny(this.Client.Close(), cancellation.Task);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        this.Client?.Dispose();
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return this.Client?.DisposeAsync() ??
            default;
    }
}
