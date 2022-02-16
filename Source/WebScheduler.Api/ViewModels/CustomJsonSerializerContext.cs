namespace WebScheduler.Api.ViewModels;

using System.Text.Json.Serialization;
using WebScheduler.Api.Models.ViewModels;

/// <summary>
/// Enables faster serialization and de-serialization with fewer allocations by generating source code.
/// </summary>
[JsonSerializable(typeof(ScheduledTask[]))]
[JsonSerializable(typeof(PageResults<ScheduledTask>[]))]
[JsonSerializable(typeof(SaveScheduledTask[]))]
internal partial class CustomJsonSerializerContext : JsonSerializerContext
{
}
