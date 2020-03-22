using System;
namespace AzureFunctions.Auth0
{
    public class AuthenticationOptions
    {
        public String Issuer { get; set; }
        public String Audience { get; set; }

    }
}
