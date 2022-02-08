namespace WebScheduler.Api.Policies;

using ITfoxtec.Identity;
using Microsoft.AspNetCore.Authorization;

public class AccessPolicyAttribute : AuthorizeAttribute
{
    private const string Name = nameof(AccessPolicyAttribute);

    public AccessPolicyAttribute() : base(Name) { }

    public static void AddPolicy(AuthorizationOptions options) => options.AddPolicy(Name, configurePolicy => configurePolicy.RequireScope("sub"));
}
