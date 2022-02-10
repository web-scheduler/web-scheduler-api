namespace WebScheduler.Api.ViewModels;

using System.Text.Json.Serialization;

/// <summary>
/// Enables faster serialization and de-serialization with fewer allocations by generating source code.
/// </summary>
[JsonSerializable(typeof(Car[]))]
[JsonSerializable(typeof(PagedCollection<Car>[]))]
[JsonSerializable(typeof(SaveCar[]))]
[JsonSerializable(typeof(ScheduledTask[]))]
[JsonSerializable(typeof(PagedCollection<ScheduledTask>[]))]
[JsonSerializable(typeof(SaveScheduledTask[]))]
internal partial class CustomJsonSerializerContext : JsonSerializerContext
{
}
