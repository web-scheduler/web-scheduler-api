// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Method is an Controller action.", Scope = "member", Target = "~M:WebScheduler.Client.Http.Controllers.ScheduledTasksController.DeleteAsync(WebScheduler.Client.Http.Commands.ScheduledTask.DeleteScheduledTaskCommand,System.Guid,System.Threading.CancellationToken)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.IActionResult}")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Method is an Controller action.", Scope = "member", Target = "~M:WebScheduler.Client.Http.Controllers.ScheduledTasksController.GetAsync(WebScheduler.Client.Http.Commands.ScheduledTask.GetScheduledTaskCommand,System.Guid,System.Threading.CancellationToken)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.IActionResult}")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Method is an Controller action.", Scope = "member", Target = "~M:WebScheduler.Client.Http.Controllers.ScheduledTasksController.GetPageAsync(WebScheduler.Client.Http.Commands.ScheduledTask.GetScheduledTaskPageCommand,WebScheduler.Client.Http.Models.ViewModels.PageOptions,System.Threading.CancellationToken)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.IActionResult}")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Method is an Controller action.", Scope = "member", Target = "~M:WebScheduler.Client.Http.Controllers.ScheduledTasksController.PostAsync(WebScheduler.Api.Commands.ScheduledTask.PostScheduledTaskCommand,WebScheduler.Client.Http.Models.ViewModels.SaveScheduledTask,System.Threading.CancellationToken)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.IActionResult}")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Method is an Controller action.", Scope = "member", Target = "~M:WebScheduler.Client.Http.Controllers.ScheduledTasksController.PutAsync(WebScheduler.Client.Http.Commands.ScheduledTask.PutScheduledTaskCommand,System.Guid,WebScheduler.Client.Http.Models.ViewModels.SaveScheduledTask,System.Threading.CancellationToken)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.IActionResult}")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Method is an Controller action.", Scope = "member", Target = "~M:WebScheduler.Client.Http.Controllers.ScheduledTasksController.PatchAsync(WebScheduler.Client.Http.Commands.ScheduledTask.PatchScheduledTaskCommand,System.Guid,Microsoft.AspNetCore.JsonPatch.JsonPatchDocument{WebScheduler.Client.Http.Models.ViewModels.SaveScheduledTask},System.Threading.CancellationToken)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.IActionResult}")]
