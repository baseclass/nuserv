using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace nuserv.UiTests.Repository
{
    public static class HtmlRepositoryUtility
    {
        public static void CompareRepository(TableRow row, IWebElement htmlRepository)
        {
            //<div class="repository" ng-show="!repository.isNew">
            //    <h3><i class="fa fa-archive fa-align-left"></i> <span class="ng-binding">title</span></h3>
            //    <h6 class="ng-binding">api url: apiurl</h6>
            //    <h6 class="ng-binding">feed url: feedurl</h6>
            //    <p>
            //        <span class="ng-binding">description</span>
            //    </p>
            //</div>

            var titleTag = htmlRepository.FindElement(By.XPath("h3[1]"));

            if (titleTag != null)
            {
                //contains archive icon
                var iTag = titleTag.FindElement(By.XPath("i"));

                if (iTag == null)
                {
                    Assert.Fail("Icon not found!");
                }
                else
                {
                    var hasIcon = iTag.GetAttribute("class") == "fa fa-archive fa-align-left";

                    Assert.IsTrue(hasIcon, "Wrong icon");
                }

                var spanTitle = titleTag.FindElement(By.XPath("span"));

                if (spanTitle == null)
                {
                    Assert.Fail("Title not found!");
                }
                else
                {
                    Assert.AreEqual(row["Title"], spanTitle.Text, "Title doesn't match");
                }
            }
            else
            {
                Assert.Fail("Title and Icon not found");
            }

            var apiUrlTag = htmlRepository.FindElement(By.XPath("h6[1]"));

            if (apiUrlTag != null)
            {
                Assert.AreEqual(string.Format("api url: {0}", row["ApiUrl"]), apiUrlTag.Text, "Api url doesn't match");
            }
            else
            {
                Assert.Fail("Api url not found!");
            }

            var feedUrlTag = htmlRepository.FindElement(By.XPath("h6[2]"));

            if (feedUrlTag != null)
            {
                Assert.AreEqual(string.Format("feed url: {0}", row["FeedUrl"]), feedUrlTag.Text, "Feed url doesn't match");
            }
            else
            {
                Assert.Fail("Feed url not found!");
            }

            var descriptionSpanTag = htmlRepository.FindElement(By.XPath("p/span"));

            if (descriptionSpanTag != null)
            {
                Assert.AreEqual(row["Description"], descriptionSpanTag.Text, "Description doesn't match");
            }
            else
            {
                Assert.Fail("Description not found!");
            }
        }
    }
}
