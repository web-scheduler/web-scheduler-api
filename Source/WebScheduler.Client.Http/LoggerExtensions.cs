namespace WebScheduler.Client.Http;

using Microsoft.Extensions.Logging;

/// <summary>
/// <see cref="ILogger"/> extension methods. Helps log messages using strongly typing and source generators.
/// </summary>
internal static partial class LoggerExtensions
{
    [LoggerMessage(
        EventId = 5413,
        Level = LogLevel.Error,
        Message = "{message}")]
    public static partial void Exception(
        this ILogger logger,
        Exception exception,
        string message);
}
