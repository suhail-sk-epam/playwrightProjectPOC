using TechTalk.SpecFlow;
using BsiPlaywrightPoc.Factory.AppSettings;
using BsiPlaywrightPoc.Model.AppSettings;

namespace BsiPlaywrightPoc.Hooks
{
    [Binding]
    public class ScenarioHooks
    {
        private readonly FeatureContext _featureContext;
        private readonly ScenarioContext _scenarioContext;
        private static AppSettings? _appSettings;

        public ScenarioHooks(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            _featureContext = featureContext ?? throw new ArgumentNullException(nameof(featureContext));
            _scenarioContext = scenarioContext;
        }

        [BeforeTestRun(Order = HookOrdering.BeforeAndAfterSetting)]
        public static void BeforeTestRun()
        {
            var appSettingsFactory = new AppSettingsFactory();
            _appSettings = appSettingsFactory.LoadAppSettings();
        }

        [BeforeFeature(Order = HookOrdering.BeforeAndAfterSetting)]
        public static void BeforeFeature(FeatureContext featureContext)
        {
            if (featureContext == null) throw new ArgumentNullException(nameof(featureContext));

            featureContext.Set(_appSettings);
        }

        [BeforeScenario(Order = HookOrdering.BeforeAndAfterSetting)]
        public void BeforeScenario()
        {
            _scenarioContext.Set(_appSettings);
        }

        [AfterTestRun(Order = HookOrdering.Maximum)]
        public static void ResetEnvironmentVariables()
        {
            Environment.SetEnvironmentVariable("EnvironmentUnderTest", null);
            Environment.SetEnvironmentVariable("LunchBrowserInHeadlessMode", null);
        }
    }
}