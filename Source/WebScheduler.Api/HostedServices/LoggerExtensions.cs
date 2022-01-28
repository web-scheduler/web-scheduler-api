namespace WebScheduler.Api.HostedServices;

internal static partial class LoggerExtensions
{
    [LoggerMessage(
        EventId = 7000,
        Level = LogLevel.Error,
        Message = "Failed to connect to Orleans cluster on attempt {Attempt} of {MaxAttempts}.")]
    public static partial void FailedToConnectToOrleansCluster(
        this ILogger logger,
        int attempt,
        int maxAttemps,
        Exception exception);
}
