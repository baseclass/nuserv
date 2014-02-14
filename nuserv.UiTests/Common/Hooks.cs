using Baseclass.Contrib.SpecFlow.Selenium.NUnit;
using Baseclass.Contrib.SpecFlow.Selenium.NUnit.Bindings;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace nuserv.UiTests.Common
{
    [Binding]
    public class Hooks
    {
        [AfterScenario]
        public void AfterScenario()
        {
            var sauceLabsRemoteDriver = Browser.Current as ISauceLabsWebDriver;

            if(sauceLabsRemoteDriver != null)
            {
                var testName = string.Format("{0}: {1}", FeatureContext.Current.FeatureInfo.Title, ScenarioContext.Current.ScenarioInfo.Title);

                var buildNumber = ConfigurationManager.AppSettings["buildNumber"];

                sauceLabsRemoteDriver.UpdateSauceLabsResult(testName, buildNumber, ScenarioContext.Current.TestError == null);
            }
        }
    }
}
