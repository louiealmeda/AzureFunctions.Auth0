using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AzureFunctions.Auth0
{
    public interface IAuthentication
    {
        Task<ClaimsPrincipal> ValidateTokenAsync(HttpRequest req);
    }
}
