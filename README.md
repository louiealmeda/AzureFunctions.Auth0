# AzureFunctions.Auth0
Auth0 authentication for AzureFunctions


## Usage

```shell
Install-Package AzureFunctions.Auth0 
```

or search for `AzureFunctions.Auth0`



### 1. Register the service in your Startup.cs
```csharp
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using AzureFunctions.Auth0;

[assembly: FunctionsStartup(typeof(MyNamespace.Startup))]

namespace MyNamespace
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //... 
            //your other registrations here
            
            builder.Services.AddAuth0Authentication();


            //add this if you need to debug your token
            //IdentityModelEventSource.ShowPII = true; 
        }
    }

```
> Create a Startup.cs if you don't have one.

### 2. Add your connection strings in the local.settings.json


```json
{

  "Values": {

    "Issuer": "https://yourapp.auth0.com/",
    "Audience": "your.apiendpoint.com"
  }
}
```

### 3. Inject and use it in your function

Inject the client in the constructor. Save it to a static variable, and use it normally.

```csharp
namespace MyNamespace
{
    public class MyFunctionName
    {

        private static IAuthentication auth;

        public Courses(IAuthentication authorization)
        {
            auth = authorization;
        }

        [FunctionName("TestClient")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "courses")] HttpRequest req,
            ILogger log
            )
        {

            var principal = await auth.ValidateTokenAsync(req);

            
            if( principal == null )
                return new UnauthorizedResult();

            string userId = principal.FindFirst(ClaimTypes.NameIdentifier).Value;

            //Do what you need to do here

            return new OkObjectResult(userId);

        }
    }
}
```
