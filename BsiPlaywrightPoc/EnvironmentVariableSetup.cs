namespace BsiPlaywrightPoc;

public class EnvironmentVariableSetup
{
    public void SetEnvironmentVariables()
    {
        var settings = new[]
        {
            "BaseUrl",
            "MiddlewareUrl",
            "IngestionUrl",
            "EntitlementsApiBaseUrl",
            "CustomerProfilesApiBaseUrl",
            "ApiUsername",
            "ApiPassword",
            "DatabaseHost",
            "DatabaseUser",
            "DatabasePassword"
        };

        foreach (var setting in settings)
        {
            Environment.SetEnvironmentVariable(setting, Config.GetSetting(setting));
        }
    }
}