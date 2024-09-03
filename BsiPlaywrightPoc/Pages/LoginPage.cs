using BsiPlaywrightPoc.Extensions;
using Microsoft.Playwright;
using TechTalk.SpecFlow;

namespace BsiPlaywrightPoc.Pages;

[Binding]
public class LoginPage
{
    private readonly IPage _page;

    public LoginPage(IPage page)
    {
        _page = page;
    }

    private ILocator LoginButtonLocator => _page.Locator("[href*='/login?redirectTo']");
    private ILocator EmailFieldLocator => _page.Locator("input[name='email']");
    private ILocator PasswordFieldLocator => _page.Locator("input[name='password']");
    private ILocator PersonalLogInBtnLocator => _page.Locator("button[type='submit']");
    private ILocator LoginFriendlyErrorMessageLocator => _page.Locator("[aria-label='Login Failed']");

    public async Task ClickLoginButton()
    {
        await LoginButtonLocator.WaitUntilAvailableAndClickAsync();
    }

    public async Task FillOutLoginForm(string email, string password)
    {
        await EmailFieldLocator.WaitUntilAvailableAndSendTextAsync(email);
        await PasswordFieldLocator.WaitUntilAvailableAndSendTextAsync(password);
    }

    public async Task<HomePage> ClickPersonalLogin()
    {
        await PersonalLogInBtnLocator.WaitUntilAvailableAndClickAsync();
        return new HomePage(_page);
    }

    public async Task<HomePage> Login(string email, string password)
    {
        await ClickLoginButton();
        await FillOutLoginForm(email, password);
        await ClickPersonalLogin();
        return new HomePage(_page);
    }

    public async Task<string> GetFriendlyErrorMessage()
    {
        return await LoginFriendlyErrorMessageLocator.WaitUntilAvailableAndReturnTextAsync();
    }

    public async Task<bool> IsLoginButtonVisible()
    {
        return await LoginButtonLocator.WaitUntilAvailableAndReturnIsVisibleAsync();
    }
}