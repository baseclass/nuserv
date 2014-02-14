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

        [Then(@"I should see (.*) existing repository")]
        public void ThenIShouldSeeExistingRepository(int p0)
        {
            var repositories = Wait.Until(d => d.FindElements(By.ClassName("repository")));

            Assert.AreEqual(p0, repositories.Count(r => r.Displayed), "Number of repositories wrong");
        }

        [Then(@"I should see a form to create a new repository")]
        public void ThenIShouldSeeAFormToCreateANewRepository()
        {
            var newRepositories = Wait.Until(d => d.FindElements(By.ClassName("newRepository")));

            Assert.AreEqual(1, newRepositories.Count(r => r.Displayed), "New repository creation form not found!");
        }

        [Then(@"I should see the following repositories:")]
        public void ThenIShouldSeeTheFollowingRepositories(Table table)
        {
            var repositories = Wait.Until(d => d.FindElements(By.ClassName("repository")).Where(r => r.Displayed)).ToArray();

            Assert.AreEqual(repositories.Count(), table.RowCount, "Number of repositories wrong");

            for(var i = 0; i < table.RowCount; i++)
            {
                HtmlRepositoryUtility.CompareRepository(table.Rows[i], repositories[i]);
            }
        }
    }
}
