using BsiPlaywrightPoc.Extensions;
using BsiPlaywrightPoc.Model;
using Microsoft.Playwright;
using TechTalk.SpecFlow;

namespace BsiPlaywrightPoc.Pages;

[Binding]
public class HomePage
{
    private readonly IPage _page;

    public HomePage(IPage page)
    {
        _page = page;
    }

    private ILocator UserAvatarLocator => _page.Locator("[data-testid='initials-avatar']");
    private ILocator LogOutBtnLocator => _page.GetByRole(AriaRole.Button, new() { Name = "Log out"});
    private ILocator HamburgerLocator => _page.Locator("[data-testid='hamburger-menu-icon']");
    private ILocator StandardsButtonLocator => _page.GetByRole(AriaRole.Link, new() { Name = "Standards"});
    private ILocator YourOrderButtonLocator => _page.GetByRole(AriaRole.Link, new() { Name = "Your Orders" });


    public async Task<bool> IsLoggedInAsync() => await UserAvatarLocator.WaitUntilAvailableAndReturnIsVisibleAsync();

    public async Task ClickUserAvatar() => await UserAvatarLocator.WaitUntilAvailableAndClickAsync();

    public async Task<OrderPage> ClickYourOrders()
    {
        await YourOrderButtonLocator.WaitUntilAvailableAndClickAsync();
        return new OrderPage(_page);
    }

    public async Task<OrderPage> NavigateToYourOrders()
    {
        await ClickUserAvatar();
        await ClickYourOrders();
        return new OrderPage(_page);
    }

    public async Task LogoutAsync()
    {
        await UserAvatarLocator.WaitUntilAvailableAndClickAsync();
        await LogOutBtnLocator.WaitUntilAvailableAndClickAsync();
    }

    public async Task ClickHamburgerAsync() => await HamburgerLocator.WaitUntilAvailableAndClickAsync();

    public async Task<StandardPage> ClickStandardAsync()
    {
        await StandardsButtonLocator.WaitUntilAvailableAndClickAsync();
        return new StandardPage(_page);
    }

    public async Task<StandardPage> NavigateToStandardPageAsync()
    {
        await ClickHamburgerAsync();
        await ClickStandardAsync();
        return new StandardPage(_page);
    }
}