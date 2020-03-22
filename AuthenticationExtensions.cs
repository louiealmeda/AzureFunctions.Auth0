using System;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFunctions.Auth0
{
    public static class AuthenticationExtensions
    {
        

        public static IServiceCollection AddAuth0Authentication(this IServiceCollection services)
        {
            services.AddSingleton<IAuthentication, Authentication>();

            services.AddOptions<AuthenticationOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.Bind(settings);
                });
            return services;
        }

        

        public static string GetUserId(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

    }
}
