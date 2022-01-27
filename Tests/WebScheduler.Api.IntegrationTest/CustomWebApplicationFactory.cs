namespace WebScheduler.Api.IntegrationTest;

using WebScheduler.Api.Options;
using WebScheduler.Api.Repositories;
using WebScheduler.Api.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using WebScheduler.Api.HostedServices;
using Orleans;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using FakeItEasy;

public class CustomWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
{
    public CustomWebApplicationFactory() => this.ClientOptions.AllowAutoRedirect = false;

    public ApplicationOptions ApplicationOptions { get; private set; } = default!;

    public Mock<ICarRepository> CarRepositoryMock { get; } = new Mock<ICarRepository>(MockBehavior.Strict);

    public Mock<IClockService> ClockServiceMock { get; } = new Mock<IClockService>(MockBehavior.Strict);

    public void VerifyAllMocks() => Mock.VerifyAll(this.CarRepositoryMock, this.ClockServiceMock);

    protected override void ConfigureClient(HttpClient client)
    {
        using (var serviceScope = this.Services.CreateScope())
        {
            var serviceProvider = serviceScope.ServiceProvider;
            this.ApplicationOptions = serviceProvider.GetRequiredService<ApplicationOptions>();
        }

        base.ConfigureClient(client);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder
            .UseEnvironment(Constants.EnvironmentName.Test)
            .ConfigureServices(this.ConfigureServices);

    protected virtual void ConfigureServices(IServiceCollection services)
    {


        services.AddDistributedMemoryCache();
        services
            .AddSingleton(this.CarRepositoryMock.Object)
            .AddSingleton(this.ClockServiceMock.Object)
            .AddTransient(_ => A.Fake<IClientBuilder>());
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.VerifyAllMocks();
        }

        base.Dispose(disposing);
    }
}
