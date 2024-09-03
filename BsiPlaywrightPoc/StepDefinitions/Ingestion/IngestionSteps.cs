using BsiPlaywrightPoc.Helpers;
using BsiPlaywrightPoc.Pages;
using BsiPlaywrightPoc.TestData;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace BsiPlaywrightPoc.StepDefinitions.Ingestion;

[Binding]
public sealed class IngestionSteps
{
    private readonly StandardPage _standardPage;
    private readonly IngestionHelper _ingestionHelper;
    private readonly ExecuteDbQueriesHelper _executeDbQueriesHelper;
    private Model.Standard _standard;

    public IngestionSteps(StandardPage standardPage, IngestionHelper ingestionHelper, ExecuteDbQueriesHelper executeDbQueriesHelper)
    {
        _standardPage = standardPage;
        _ingestionHelper = ingestionHelper;
        _executeDbQueriesHelper = executeDbQueriesHelper;
    }

    [Given(@"I have ingested a digital copy")]
    public async Task GivenIHaveIngestedADigitalCopy()
    {
        _standard = "DigitalOnlyStandard".GetStandardByName();
        var standardDbResponse = _executeDbQueriesHelper.QueryStandardDb(_standard.SapId);

        if (standardDbResponse.Rows != null)
        {
            var response = await _ingestionHelper.DeleteIngestedStandard(_standard);
            response!.DeletedSapIds.FirstOrDefault().Should().Be(_standard.SapId);
        }

        await _ingestionHelper.IngestStandard(_standard);
    }

    [When(@"I search for the ingested standard")]
    public async Task WhenISearchForTheIngestedStandard()
    {
        var standardDbResponse = _executeDbQueriesHelper.QueryStandardDb(_standard.SapId);
        standardDbResponse.Rows.Count.Should().Be(1);

        await _standardPage.SearchForStandardAsync(_standard.SapId);
    }

    [Then(@"it should be visible")]
    public async Task ThenItShouldBeVisible()
    {
        var actualTitle = await _standardPage.GetDisplayedStandardsTitleAsync();
        actualTitle.Should().Be(_standard.Title);
    }
}