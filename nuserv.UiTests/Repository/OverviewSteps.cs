using Baseclass.Contrib.SpecFlow.Selenium.NUnit.Bindings;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;
using TechTalk.SpecFlow;

namespace nuserv.UiTests.Repository
{
    [Binding]
    public class OverviewSteps
    {
        private WebDriverWait wait;
        public WebDriverWait Wait
        {
            get
            {
                if (wait == null)
                {
                    this.wait = new WebDriverWait(Browser.Current, TimeSpan.FromSeconds(10));
                }
                return wait;
            }
        }

        [Then(@"I should see atleast (.*) existing repository")]
        public void ThenIShouldSeeExistingRepository(int p0)
        {
            var repositories = Wait.Until(d => d.FindElements(By.ClassName("repository")));

            Assert.GreaterOrEqual(repositories.Count(r => !r.GetAttribute("class").Contains("ng-hide")), p0, "Number of repositories wrong");
        }

        [Then(@"I should see a form to create a new repository")]
        public void ThenIShouldSeeAFormToCreateANewRepository()
        {
            var newRepositories = Wait.Until(d => d.FindElements(By.ClassName("newRepository")).Where(r => !r.GetAttribute("class").Contains("ng-hide")));

            Assert.AreEqual(1, newRepositories.Count(r => !r.GetAttribute("class").Contains("ng-hide")), "New repository creation form not found!");
        }

        [Then(@"I should see atleast the following repositories:")]
        public void ThenIShouldSeeTheFollowingRepositories(Table table)
        {
            var repositories = Wait.Until(d => d.FindElements(By.ClassName("repository")).Where(r => !r.GetAttribute("class").Contains("ng-hide"))).ToArray();

            Assert.GreaterOrEqual(repositories.Count(), table.RowCount, "Number of repositories wrong");

            for(var i = 0; i < table.RowCount; i++)
            {
                IWebElement repository = null;
                var tableRow = table.Rows[i];

                var title = tableRow["Title"];

                for (var z = 0; z < repositories.Count(); z++)
                {
                    repository = repositories[z].FindElement(By.Id(title));
                    if (repository != null)
                    {
                        break;
                    }
                }

                Assert.NotNull(repository, string.Format("Can't find repository {0}", title));

                HtmlRepositoryUtility.CompareRepository(tableRow, repository);
            }
        }
    }
}
