namespace WebScheduler.Api.Http.ViewModels;

using System.Text.Json.Serialization;
using WebScheduler.Abstractions.Grains.Scheduler;
using WebScheduler.Api.Models.ViewModels;

/// <summary>
/// Enables faster serialization and de-serialization with fewer allocations by generating source code.
/// </summary>
[JsonSerializable(typeof(ScheduledTask))]
[JsonSerializable(typeof(PageResults<ScheduledTask>[]))]
[JsonSerializable(typeof(SaveScheduledTask[]))]
[JsonSerializable(typeof(PageOptions[]))]
[JsonSerializable(typeof(PageOptions))]
[JsonSerializable(typeof(PageResults<ScheduledTask>))]
[JsonSerializable(typeof(List<Header>))]
[JsonSerializable(typeof(Header))]
internal partial class CustomJsonSerializerContext : JsonSerializerContext
{
}
