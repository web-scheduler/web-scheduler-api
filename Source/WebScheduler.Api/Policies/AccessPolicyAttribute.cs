namespace WebScheduler.Api.Policies;

using Microsoft.AspNetCore.Authorization;

/// <summary>
/// TODO
/// </summary>
public class AccessPolicyAttribute : AuthorizeAttribute
{
    private const string Name = nameof(AccessPolicyAttribute);

    /// <summary>
    /// Initializes a new instance of the class with the specified policy.
    /// </summary>
    public AccessPolicyAttribute() : base(Name) { }

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="options"></param>
    public static void AddPolicy(AuthorizationOptions options) => options.AddPolicy(Name, configurePolicy => configurePolicy.RequireClaim("sub"));
}
