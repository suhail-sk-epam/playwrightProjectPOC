using Microsoft.Extensions.Configuration;

namespace BsiPlaywrightPoc;

public class Config
{
    public static IConfigurationRoot Configuration { get; private set; }

    static Config()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        Configuration = builder.Build();
    }

    public static string GetSetting(string key)
    {
        return Configuration[$"Settings:{Environment.GetEnvironmentVariable("ENV") ?? "Main"}:{key}"];
    }
}