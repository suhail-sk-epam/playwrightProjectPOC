using BsiPlaywrightPoc.Pages;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace BsiPlaywrightPoc.StepDefinitions.Standard
{
    [Binding]
    public sealed class StandardsSteps
    {
        private readonly HomePage _homePage;
        private readonly StandardPage _standardPage;

        public StandardsSteps(HomePage homePage, StandardPage standardPage)
        {
            _homePage = homePage;
            _standardPage = standardPage;
        }

        [Given(@"I am on the knowledge standard page")]
        public async Task GivenIAmOnTheKnowledgeStandardPage()
        {
            await _homePage.ClickHamburgerAsync();
            await _homePage.ClickStandardAsync();
        }

        [Then(@"(.*) standards should be visible")]
        public async Task ThenSeveralStandardsShouldBeVisible(int expectedStandardCount)
        {
            var actualStandardCount = await _standardPage.GetDisplayedStandardsCountAsync();
            actualStandardCount.Should().Be(expectedStandardCount);
        }
    }
}
