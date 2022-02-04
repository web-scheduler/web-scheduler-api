namespace WebScheduler.Api.Policies;

using ITfoxtec.Identity;
using Microsoft.AspNetCore.Authorization;

    public class AccessPolicyAttribute : AuthorizeAttribute
    {
        private static string _name = nameof(AccessPolicyAttribute);

        public AccessPolicyAttribute() : base(_name)
        { }

        public static void AddPolicy(AuthorizationOptions options)
        {
            options.AddPolicy(_name, configurePolicy =>
            {
                //TODO: Require scoep
                configurePolicy.RequireScope("sub");
            });
        }
    }
