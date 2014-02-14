using Baseclass.Contrib.SpecFlow.Selenium.NUnit;
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
    public class AddRepositorySteps
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

        public void ResetWait()
        {
            wait = null;
        }

        [Given(@"I have entered the following information:")]
        public void GivenIHaveEnteredTheFollowingInformation(Table table)
        {
            var newRepository = GetNewRepository();

            if (newRepository == null)
            {
                Assert.Inconclusive("New repository form not found!");
            }

            var row = table.Rows[0];

            var uniqueness = Guid.NewGuid().ToString().Replace("{", string.Empty).Replace("}", string.Empty);

            ScenarioContext.Current["uniqueness"] = uniqueness;

            var title = string.Format(row["Title"], uniqueness);

            var titleInputField = newRepository.FindElement(By.Id("name"));

            Assert.IsNotNull(titleInputField, "Name input field not found!");

            titleInputField.SendKeys(title);

            var descriptionInputField = newRepository.FindElement(By.Id("description"));

            Assert.IsNotNull(descriptionInputField, "Description input field not found!");

            descriptionInputField.SendKeys(row["Description"]);
        }

        [When(@"I press Save")]
        public void WhenIPressSave()
        {
           var newRepository = GetNewRepository();

           var saveButton = newRepository.FindElements(By.Id("saveNewRepository")).FirstOrDefault();

           Assert.IsNotNull(saveButton, "Can't find save button!");

           saveButton.Click();

           ResetWait();
        }

        [Then(@"I should see the following repository:")]
        public void ThenIShouldSeeTheFollowingRepository(Table table)
        {
            var row = table.Rows[0];

            row["Title"] = string.Format(row["Title"], ScenarioContext.Current["uniqueness"]);
            row["ApiUrl"] = string.Format(row["ApiUrl"], ScenarioContext.Current["uniqueness"]);
            row["FeedUrl"] = string.Format(row["FeedUrl"], ScenarioContext.Current["uniqueness"]);

            var newRepository = GetNewRepository();

            var repository = newRepository.FindElements(By.Id(row["Title"])).LastOrDefault();

            Assert.IsNotNull(repository, "Repository not found!");

            HtmlRepositoryUtility.CompareRepository(row, repository);
        }

        [Then(@"I should see the error ""(.*)"" on (.*)")]
        public void ThenIShouldSeeTheErrorOn(string error, string name)
        {
            var newRepository = GetNewRepository();

            var labels = newRepository.FindElements(By.TagName("label"));

            var label = labels.FirstOrDefault(l => l.GetAttribute("for") == name);

            Assert.IsNotNull(label, string.Format("Error for {0} not found", name));

            Assert.AreEqual(error, label.Text);
        }

        [Then(@"I should see the url ""(.*)""")]
        public void ThenIShouldSeeTheUrl(string url)
        {
            var newRepository = GetNewRepository();

            var urlInputField = newRepository.FindElement(By.Id("id"));

            Assert.IsNotNull(urlInputField, "Url input field not found!");

            Assert.AreEqual(url, urlInputField.GetAttribute("value"));
        }

        private IWebElement GetNewRepository()
        {
            var newRepository = Wait.Until(d => d.FindElements(By.ClassName("newRepository")).FirstOrDefault(r => !r.GetAttribute("class").Contains("ng-hide")));

            Assert.NotNull(newRepository, "Can't find new repository form!");

            return newRepository;
        }
    }
}
