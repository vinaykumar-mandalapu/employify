using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using employify.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["AppHost:BrowserToken"] = "",
    });
}

string envFilePath = Path.Combine(builder.Environment.ContentRootPath, ".env");
builder.Environment.LoadEnvironmentVariablesFromEnvFile(envFilePath);

/*
#region SQL Server Container
IResourceBuilder<ParameterResource> sqlPassword = builder.AddParameter("employify-sqlserver-password", true);
IResourceBuilder<SqlServerServerResource> sqlserver = builder.AddSqlServer("employify-sqlserver", sqlPassword, 1433)
    .WithDataVolume("employify-sqlserver-volume")
    .WithDockerfile(".", "LocalDb.docker")
    .WithContainerRuntimeArgs("--platform", "linux/amd64", "--restart", "unless-stopped")
    .WithLifetime(ContainerLifetime.Persistent);
IResourceBuilder<SqlServerDatabaseResource> _ = sqlserver.AddDatabase("employify");
#endregion
*/

var apiService = builder.AddProject<Projects.employify_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.employify_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
