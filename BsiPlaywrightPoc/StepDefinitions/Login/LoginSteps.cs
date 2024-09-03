using BsiPlaywrightPoc.Pages;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace BsiPlaywrightPoc.StepDefinitions.Login;

[Binding]
public class LoginSteps
{
    private readonly LoginPage _loginPage;
    private readonly HomePage _homePage;

    public LoginSteps(LoginPage loginPage, HomePage homePage)
    {
        _loginPage = loginPage;
        _homePage = homePage;
    }

    [Given(@"I am on the knowledge log in page")]
    public async Task GivenIAmOnTheKnowledgeLogInPage()
    {
        await _loginPage.ClickLoginButton();
    }

    [Given(@"I fillOut username '([^']*)' and password '([^']*)' credentials")]
    public async Task GivenIFillOutUsernameAndPasswordCredentials(string userName, string password)
    {
        await _loginPage.FillOutLoginForm(userName, password);
    }

    [Given(@"I am logged In")]
    public async Task GivenIAmLoggedIn()
    {
        await _loginPage.FillOutLoginForm("test.test@example.org", "Password123");
        await _loginPage.ClickPersonalLogin();
    }

    [When(@"I click login")]
    public async Task WhenIClickLogin()
    {
        await _loginPage.ClickPersonalLogin();
    }

    [Then(@"the user login should be '([^']*)'")]
    public async Task ThenTheUserLoginShouldBe(string expectedResults)
    {
        if (expectedResults.Contains("Invalid", StringComparison.OrdinalIgnoreCase))
        {
            var friendlyErrorMessage = await _loginPage.GetFriendlyErrorMessage();
            friendlyErrorMessage.Should().Be(expectedResults);
        }
        else
        {
            var isUserLoggedIn = await _homePage.IsLoggedInAsync();
            isUserLoggedIn.Should().BeTrue();
        }
    }

    [When(@"I click logout")]
    public async Task WhenIClickLogout()
    {
        await _homePage.LogoutAsync();
    }

    [Then(@"I should be logged out successfully")]
    public async Task ThenIShouldBeLoggedOutSuccessfully()
    {
        var isLoginButtonVisible = await _loginPage.IsLoginButtonVisible();
        isLoginButtonVisible.Should().BeTrue();
    }
}