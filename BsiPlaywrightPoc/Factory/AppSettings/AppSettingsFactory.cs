using Microsoft.Extensions.Configuration;

namespace BsiPlaywrightPoc.Factory.AppSettings
{
    public class AppSettingsFactory
    {
        private IConfiguration Configuration { get; }
        private Model.AppSettings.AppSettings _appSettings;

        public AppSettingsFactory()
        {
            var env = Environment.GetEnvironmentVariable("EnvironmentUnderTest") ?? "local";
            Environment.SetEnvironmentVariable("EnvironmentUnderTest", env);

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appSettings.{env}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public Model.AppSettings.AppSettings LoadAppSettings()
        {
            _appSettings = new Model.AppSettings.AppSettings();

            Configuration.Bind(_appSettings);
            return _appSettings;
        }
    }
}
