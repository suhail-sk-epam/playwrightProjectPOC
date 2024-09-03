using BsiPlaywrightPoc.Model.AppSettings;
using Microsoft.Playwright;
using TechTalk.SpecFlow;
using BrowserType = BsiPlaywrightPoc.Model.Enums.BrowserType;

namespace BsiPlaywrightPoc.Web;

public class PlaywrightDriver : IAsyncDisposable
{
    private readonly ScenarioContext _scenarioContext;
    private readonly Task<IPage> _page;
    private readonly BrowserType _browserType;
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private AppSettings _appSettings;

    public PlaywrightDriver(ScenarioContext scenarioContext, BrowserType browserType)
    {
        _scenarioContext = scenarioContext;
        _browserType = browserType;
        _page = InitialisePlaywright();
    }

    public IPage Page => _page.Result;

    private async Task<IPage> InitialisePlaywright()
    {
        _appSettings = _scenarioContext.Get<AppSettings>();
        var playwright = await Playwright.CreateAsync();

        var browser = _browserType switch
        {
            BrowserType.Firefox => playwright.Firefox,
            BrowserType.WebKit => playwright.Webkit,
            _ => playwright.Chromium,
        };

        _browser = await browser.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = _appSettings.RuntimeSettings.LunchBrowserInHeadlessMode,
        });

        _context = await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            HttpCredentials = new HttpCredentials
            {
                Username = _appSettings.KnowledgeBasicAuth.UserName!,
                Password = _appSettings.KnowledgeBasicAuth.Password!
            }
        });

        var page = await _context.NewPageAsync();

        //set specific browser size
        //await page.SetViewportSizeAsync(1980, 1080);

        // Navigate to the application URL
        await page.GotoAsync(_appSettings.BsiKnowledge.HostName!);

        // Wait for the cookie consent button to appear and click it
        var cookieAcceptButton = await page.WaitForSelectorAsync("#onetrust-accept-btn-handler", new PageWaitForSelectorOptions { Timeout = 10000 });
        if (cookieAcceptButton != null)
        {
            await cookieAcceptButton.ClickAsync();
        }

        return page;
    }

    public async ValueTask DisposeAsync()
    {
        if (_context != null)
        {
            await _context.DisposeAsync();
        }

        if (_browser != null)
        {
            await _browser.CloseAsync();
        }
    }
}