using Baseclass.Contrib.SpecFlow.Selenium.NUnit.Bindings;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace nuserv.UiTests.Common
{
    [Binding]
    public class Common
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

        [Then(@"the browser title should be ""(.*)""")]
        public void ThenTheBrowserTitleShouldBe(string title)
        {
            var result = Wait.Until(d => d.Title);

            Assert.AreEqual(title, result);
        }

        [Then(@"the title should be ""(.*)""")]
        public void ThenTheTitleShouldBe(string title)
        {
            var jumbotron = Wait.Until(d => d.FindElement(By.ClassName("jumbotron")));

            var header = jumbotron.FindElement(By.TagName("h1"));

            Assert.AreEqual(title, header.Text);
        }

        [Then(@"the lead should be ""(.*)""")]
        public void ThenTheLeadShouldBe(string p0)
        {
            var lead = Wait.Until(d => d.FindElement(By.ClassName("lead")));

            Assert.AreEqual(p0, lead.Text);
        }
    }
}
