namespace WebScheduler.Api.ConfigureOptions;

using WebScheduler.Api.Options;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;

/// <summary>
/// Configures Redis based distributed caching for the application.
/// </summary>
public class ConfigureRedisCacheOptions : IConfigureOptions<RedisCacheOptions>
{
    private readonly RedisOptions redisOptions;

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="redisOptions"></param>
    public ConfigureRedisCacheOptions(RedisOptions redisOptions) =>
        this.redisOptions = redisOptions;

    /// <summary>
    /// Invoked to configure a  instance.
    /// </summary>
    /// <param name="options">The options instance to configure.</param>
    public void Configure(RedisCacheOptions options) =>
        options.ConfigurationOptions = this.redisOptions.ConfigurationOptions;
}
