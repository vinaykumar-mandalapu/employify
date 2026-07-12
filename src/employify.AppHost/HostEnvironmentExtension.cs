using Microsoft.Extensions.Hosting;

namespace employify.AppHost;

internal static class HostEnvironmentExtension
{
    public static IHostEnvironment LoadEnvironmentVariablesFromEnvFile(this IHostEnvironment environment, string envFilePath)
    {
        if (File.Exists(envFilePath))
        {
            var lines = File.ReadAllLines(envFilePath);
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (!string.IsNullOrWhiteSpace(trimmedLine) && !trimmedLine.StartsWith("#"))
                {
                    var keyValue = trimmedLine.Split('=', 2, StringSplitOptions.RemoveEmptyEntries);
                    if (keyValue.Length == 2)
                    {
                        var key = keyValue[0].Trim();
                        if (key.Contains("${"))
                            key = Environment.ExpandEnvironmentVariables(key.Replace("${", "%").Replace("}", "%"));
                        var value = keyValue[1].Trim();
                        Environment.SetEnvironmentVariable(key, value);
                    }
                }
            }
        }
        environment.EnvironmentName = Environment.GetEnvironmentVariable("AspNetCore_Environment") ?? environment.EnvironmentName;
        //environment.EnvironmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? environment.EnvironmentName;
        return environment;
    }

    public static string? GetDevDatabaseBaseImage(this IHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            return Environment.GetEnvironmentVariable("DEV_DATABASE_BASE_IMAGE");
        }
        return null;
    }
}