using BoDi;
using BsiPlaywrightPoc.Web;
using TechTalk.SpecFlow;
using Microsoft.Playwright;
using TechTalk.SpecFlow.Infrastructure;
using BrowserType = BsiPlaywrightPoc.Model.Enums.BrowserType;

namespace BsiPlaywrightPoc.Hooks
{
    [Binding]
    public class PlaywrightHooks
    {
        private readonly ScenarioContext _scenarioContext;
        private PlaywrightDriver _driver;
        private readonly IObjectContainer _container;
        private readonly ISpecFlowOutputHelper _specFlowOutputHelper;

        public PlaywrightHooks(ScenarioContext scenarioContext, IObjectContainer container, ISpecFlowOutputHelper specFlowOutputHelper)
        {
            _scenarioContext = scenarioContext;
            _container = container;
            _specFlowOutputHelper = specFlowOutputHelper;
        }

        [BeforeScenario("@webScenario", Order = HookOrdering.WebSetting)]
        public void BeforeScenarioRegisterPlaywrightDriver()
        {
            _driver = new PlaywrightDriver(_scenarioContext, BrowserType.Chromium);
            _container.RegisterInstanceAs(_driver);
            _container.RegisterInstanceAs(_driver.Page);
        }

        [AfterStep("@webScenario", Order = HookOrdering.ScreenShot)]
        public async Task AfterWebStep()
        {
            if (_scenarioContext.TestError != null)
            {
                var page = _container.Resolve<IPage>();
                var screenShotPath = Path.Combine("Screenshots", $"{_scenarioContext.ScenarioInfo.Title.Replace(" ", "_")}.png");
                Directory.CreateDirectory("Screenshots"); // Ensure the directory exists
                await page.ScreenshotAsync(new PageScreenshotOptions { Path = screenShotPath });

                // Log the screenShot path to SpecFlow's output
                _specFlowOutputHelper.AddAttachment(screenShotPath);
                _specFlowOutputHelper.WriteLine($"ScreenShot saved to: {Path.GetFullPath(screenShotPath)}");
            }
        }

        [AfterScenario("webScenario")]
        public async Task AfterTestScenario()
        {
            await _driver.DisposeAsync();
        }
    }
}
