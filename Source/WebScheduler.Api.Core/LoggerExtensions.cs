namespace WebScheduler.Api;

using Microsoft.Extensions.Logging;

/// <summary>
/// <see cref="ILogger"/> extension methods. Helps log messages using strongly typing and source generators.
/// </summary>
internal static partial class LoggerExtensions
{
    [LoggerMessage(
        EventId = 7000,
        Level = LogLevel.Error,
        Message = "Failed to connect to Orleans cluster on attempt {Attempt} of {MaxAttempts}.")]
    public static partial void FailedToConnectToOrleansCluster(
        this ILogger logger,
        Exception exception,
        int attempt,
        int maxAttempts);

    [LoggerMessage(
        EventId = 7001,
        Level = LogLevel.Information,
        Message = "Silo shutting down gracefully.")]
    public static partial void ShuttingDownSiloGracefully(this ILogger logger);

    [LoggerMessage(
    EventId = 7002,
    Level = LogLevel.Error,
    Message = "Error getting scheduled tasks.")]
    public static partial void GettingScheduledTasks(this ILogger logger, Exception exception);
}
