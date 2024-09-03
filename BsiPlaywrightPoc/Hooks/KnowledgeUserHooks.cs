using BsiPlaywrightPoc.Helpers;
using BsiPlaywrightPoc.Model.RequestObject;
using BsiPlaywrightPoc.Model.User;
using BsiPlaywrightPoc.Pages;
using FluentAssertions;
using BsiPlaywrightPoc.Model.AppSettings;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.UnitTestProvider;

namespace BsiPlaywrightPoc.Hooks
{
    [Binding]
    public class KnowledgeUserHooks
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly MiddlewareHelper _middlewareHelper;
        private readonly LoginPage _loginPage;
        private readonly HomePage _homePage;
        private readonly IUnitTestRuntimeProvider _unitTestRuntimeProvider;
        private AppSettings _appSettings;

        public KnowledgeUserHooks(ScenarioContext scenarioContext, MiddlewareHelper middlewareHelper, LoginPage loginPage, HomePage homePage, IUnitTestRuntimeProvider unitTestRuntimeProvider)
        {
            _scenarioContext = scenarioContext;
            _middlewareHelper = middlewareHelper;
            _loginPage = loginPage;
            _homePage = homePage;
            _unitTestRuntimeProvider = unitTestRuntimeProvider;
        }

        [BeforeScenario("@createRandomUserAndLogin", Order = HookOrdering.User)]
        public async Task SetUpAndLoginRandomUserBeforeScenario()
        {
            string? payBy = null;
            UserCredentials randomUser;
            _appSettings = _scenarioContext.Get<AppSettings>();

            // Check if 'payBy' exists in the ScenarioContext
            var scenarioTitle = _scenarioContext.ScenarioInfo.Title;
            switch (scenarioTitle.ToLower())
            {
                case "purchasing a standard":
                    {
                        var arguments = _scenarioContext.ScenarioInfo.Arguments;
                        if (arguments != null && arguments.Contains("payBy"))
                        {
                            payBy = arguments["payBy"]?.ToString();

                            if (payBy!.Equals("invoice", StringComparison.OrdinalIgnoreCase) && _appSettings.RuntimeSettings.Environment!.ToLower() != "uat")
                            {
                                _unitTestRuntimeProvider.TestInconclusive(
                                    $"{scenarioTitle} Scenario for {payBy} user, is not set-up on {_appSettings.RuntimeSettings.Environment} environment, so therefor it needs to be skipped {Environment.NewLine}");
                            }
                        }
                        break;
                    }
            }

            switch (payBy!.ToLower())
            {
                case "invoice":
                    randomUser = RandomData.GetInvoiceUserDetails();
                    break;
                default:
                    {
                        randomUser = RandomData.GenerateRandomUserCredentials();
                        var payload = new UserSignUpRequestObject()
                        {
                            email = randomUser.Email,
                            firstName = randomUser.Firstname,
                            lastName = randomUser.Lastname,
                            password = randomUser.Password
                        };

                        await _middlewareHelper.SignUp(payload);
                        break;
                    }
            }

            await _loginPage.Login(randomUser.Email, randomUser.Password);

            var isUserLoggedIn = await _homePage.IsLoggedInAsync();
            isUserLoggedIn.Should().BeTrue();

            _scenarioContext.Set(randomUser);
        }
    }
}
