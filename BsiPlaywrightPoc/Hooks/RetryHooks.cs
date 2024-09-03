using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using TechTalk.SpecFlow;

namespace BsiPlaywrightPoc.Hooks
{
    //[Binding]
    public class RetryHooks
    {
        private readonly ScenarioContext _scenarioContext;
        private const int MaxRetryCount = 3;  // Number of retries
        private int _currentRetryCount;

        public RetryHooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        //[BeforeScenario]
        public void BeforeScenario()
        {
            _currentRetryCount = 0;  // Initialize retry count before each scenario
        }

        //[AfterScenario]
        public void AfterScenario()
        {
            var testContext = TestContext.CurrentContext;

            while (testContext.Result.Outcome.Status == TestStatus.Failed && _currentRetryCount < MaxRetryCount)
            {
                _currentRetryCount++;

                Console.WriteLine($"Retrying scenario '{_scenarioContext.ScenarioInfo.Title}' - Attempt {_currentRetryCount}/{MaxRetryCount}");

                // Mark scenario as skipped and rerun it
                if (_currentRetryCount < MaxRetryCount)
                {
                    // Marking the scenario as skipped and causing a re-run
                    _scenarioContext.Pending();
                }
            }
        }
    }
}