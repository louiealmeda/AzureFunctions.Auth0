using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
namespace AzureFunctions.Auth0
{
    public class Authentication : IAuthentication
    {
        private readonly IConfigurationManager<OpenIdConnectConfiguration> _configurationManager;
        private readonly string ISSUER;
        private readonly string AUDIENCE;
        public Authentication(IOptions<AuthenticationOptions> options)
        {
            ISSUER = options.Value.Issuer;
            AUDIENCE = options.Value.Audience;

            var documentRetriever = new HttpDocumentRetriever { RequireHttps = ISSUER.StartsWith("https://") };
            _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            $"{ISSUER}.well-known/openid-configuration",
            new OpenIdConnectConfigurationRetriever(),
            documentRetriever
                        );
        }

        public async Task<ClaimsPrincipal> ValidateTokenAsync(HttpRequest req)
        {

            if (!req.Headers.ContainsKey("Authorization"))
                return null;

            
            string token = req.Headers["Authorization"].ToString().Replace("Bearer ", "");

            try
            {
                var header = new AuthenticationHeaderValue("Bearer", token);

                return await ValidateTokenAsync(header);
            }
            catch
            {
                return null;
            }
            
        }

        public async Task<ClaimsPrincipal> ValidateTokenAsync(AuthenticationHeaderValue value)
        {
            if (value?.Scheme != "Bearer")
                return null;

            var config = await _configurationManager.GetConfigurationAsync(CancellationToken.None);
            var validationParameter = new TokenValidationParameters
            {
                RequireSignedTokens = true,
                ValidAudience = AUDIENCE,
                ValidateAudience = true,
                ValidIssuer = ISSUER,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKeys = config.SigningKeys
            };
            ClaimsPrincipal result = null;
            var tries = 0;
            while (result == null && tries <= 1)
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    result = handler.ValidateToken(value.Parameter, validationParameter, out var token);
                }
                catch (SecurityTokenSignatureKeyNotFoundException ex1)
                {
                    // This exception is thrown if the signature key of the JWT could not be found.
                    // This could be the case when the issuer changed its signing keys, so we trigger a 
                    // refresh and retry validation.
                    _configurationManager.RequestRefresh();
                    tries++;
                }
                catch (SecurityTokenException ex2)
                {
                    return null;
                }
            }
            return result;
        }
    }
}