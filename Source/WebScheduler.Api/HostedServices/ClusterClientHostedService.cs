namespace WebScheduler.Api.HostedServices;

using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using WebScheduler.Abstractions.Constants;
using WebScheduler.Api.Options;

public sealed class ClusterClientHostedService : IHostedService, IAsyncDisposable, IDisposable
{
    private readonly ILogger<ClusterClientHostedService> logger;
    private readonly ApplicationOptions options;

    public ClusterClientHostedService(ILoggerFactory loggerFactory, IOptions<ApplicationOptions> options)
    {
        this.logger = loggerFactory.CreateLogger<ClusterClientHostedService>();
        this.options = options.Value;

        this.Client = new ClientBuilder()
            .ConfigureServices(services =>
            {
                // Add logging from the host's container.
                services.AddSingleton(loggerFactory);
                services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            })
            //.AddClusterConnectionLostHandler(async (object sender, EventArgs e) => await this.ConnectionHandler().ConfigureAwait(false))
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
                   .AddApplicationPart(typeof(Abstractions.Grains.HealthChecks.ILocalHealthCheckGrain).Assembly)
                   .WithReferences())
           .AddSimpleMessageStreamProvider(StreamProviderName.Default)
            //.UseTls(
            //    options =>
            //    {
            //        // TODO: Configure a certificate.
            //        options.LocalCertificate = null;

            //        // TODO: Do not allow any remote certificates in production.
            //        options.AllowAnyRemoteCertificate();
            //    })
            .Build();
    }

    public IClusterClient Client { get; }

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
                this.logger.LogWarning(error, "Failed to connect to Orleans cluster on attempt {Attempt} of {MaxAttempts}.", attempt, maxAttempts);

                try
                {
                    await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    return false;
                }

                return true;
            }

            this.logger.LogError(error, "Failed to connect to Orleans cluster on attempt {Attempt} of {MaxAttempts}.", attempt, maxAttempts);

            return false;
        });
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Shutting down silo connection.");

        var cancellation = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        _ = cancellationToken.Register(() => cancellation.TrySetCanceled(cancellationToken));
        _ = await Task.WhenAny(this.Client.Close(), cancellation.Task).ConfigureAwait(false);
    }

    public void Dispose() => this.Client?.Dispose();

#pragma warning disable VSTHRD110 // Observe result of async calls
    public ValueTask DisposeAsync() => this.Client?.DisposeAsync() ?? default;
#pragma warning restore VSTHRD110 // Observe result of async calls
}
