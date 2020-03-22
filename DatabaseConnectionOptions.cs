using System;
namespace AzureFunctions.Auth0
{
    public class DatabaseConnectionOptions
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }
}
