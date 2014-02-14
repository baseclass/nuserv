﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.9.0.77
//      SpecFlow Generator Version:1.9.0.0
//      Runtime Version:4.0.30319.34003
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace nuserv.UiTests.Repository
{
    using TechTalk.SpecFlow;
    using Autofac;
    using Autofac.Configuration;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("AddRepository")]
    public partial class AddRepositoryFeature
    {
        
        private OpenQA.Selenium.IWebDriver driver;
        
        private IContainer container;
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "AddRepository.feature"
#line hidden
        
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new ConfigurationSettingsReader());
            this.container = builder.Build();
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "AddRepository", "In order to add a repository\r\nAs a user\r\nI want to be able to fill out a simple f" +
                    "orm with validation.", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.TestFixtureTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
            if(this.driver != null)
                ScenarioContext.Current.Add("Driver", this.driver);
            if(this.container != null)
                ScenarioContext.Current.Add("Container", this.container);
        }
        
        public virtual void ScenarioCleanup()
        {
            try { System.Threading.Thread.Sleep(50); this.driver.Quit(); } catch (System.Exception) {}
            this.driver = null;
            ScenarioContext.Current.Remove("Driver");
            ScenarioContext.Current.Remove("Container");
            testRunner.CollectScenarioErrors();
        }
        
        private void InitializeSelenium(string browser)
        {
            this.driver = this.container.ResolveNamed<OpenQA.Selenium.IWebDriver>(browser);
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Successfully add a repository")]
        [NUnit.Framework.TestCaseAttribute("IE", Category="IE", TestName="SuccessfullyAddARepository on IE")]
        [NUnit.Framework.TestCaseAttribute("Firefox", Category="Firefox", TestName="SuccessfullyAddARepository on Firefox")]
        [NUnit.Framework.TestCaseAttribute("Chrome", Category="Chrome", TestName="SuccessfullyAddARepository on Chrome")]
        public virtual void SuccessfullyAddARepository(string browser)
        {
            InitializeSelenium(browser);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Successfully add a repository", new string[] {
                        "Browser:IE",
                        "Browser:Firefox",
                        "Browser:Chrome"});
#line 9
this.ScenarioSetup(scenarioInfo);
#line 10
 testRunner.Given("I navigated to /repository", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "Title",
                        "Description"});
            table1.AddRow(new string[] {
                        "new repository{0}",
                        "New repository Description"});
#line 11
 testRunner.And("I have entered the following information:", ((string)(null)), table1, "And ");
#line 14
 testRunner.When("I press Save", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "Title",
                        "ApiUrl",
                        "FeedUrl",
                        "Description"});
            table2.AddRow(new string[] {
                        "new repository{0}",
                        "repository/new-repository{0}",
                        "repository/new-repository{0}/api/v2",
                        "New repository Description"});
#line 15
 testRunner.Then("I should see the following repository:", ((string)(null)), table2, "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Add a repository without data")]
        [NUnit.Framework.TestCaseAttribute("IE", Category="IE", TestName="AddARepositoryWithoutData on IE")]
        [NUnit.Framework.TestCaseAttribute("Firefox", Category="Firefox", TestName="AddARepositoryWithoutData on Firefox")]
        [NUnit.Framework.TestCaseAttribute("Chrome", Category="Chrome", TestName="AddARepositoryWithoutData on Chrome")]
        public virtual void AddARepositoryWithoutData(string browser)
        {
            InitializeSelenium(browser);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Add a repository without data", new string[] {
                        "Browser:IE",
                        "Browser:Firefox",
                        "Browser:Chrome"});
#line 22
this.ScenarioSetup(scenarioInfo);
#line 23
 testRunner.Given("I navigated to /repository", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 24
 testRunner.When("I press Save", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 25
 testRunner.Then("I should see the error \"Name is to short\" on name", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 26
 testRunner.And("I should see the error \"Description is to short\" on description", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Automatic url generation")]
        [NUnit.Framework.TestCaseAttribute("IE", "New Rep0sitory", "new-rep0sitory", null, Category="IE", TestName="AutomaticUrlGeneration on IE with: \"New Rep0sitory\" ,\"new-rep0sitory\"")]
        [NUnit.Framework.TestCaseAttribute("Firefox", "New Rep0sitory", "new-rep0sitory", null, Category="Firefox", TestName="AutomaticUrlGeneration on Firefox with: \"New Rep0sitory\" ,\"new-rep0sitory\"")]
        [NUnit.Framework.TestCaseAttribute("Chrome", "New Rep0sitory", "new-rep0sitory", null, Category="Chrome", TestName="AutomaticUrlGeneration on Chrome with: \"New Rep0sitory\" ,\"new-rep0sitory\"")]
        [NUnit.Framework.TestCaseAttribute("IE", "group/name", "group-name", null, Category="IE", TestName="AutomaticUrlGeneration on IE with: \"group/name\" ,\"group-name\"")]
        [NUnit.Framework.TestCaseAttribute("Firefox", "group/name", "group-name", null, Category="Firefox", TestName="AutomaticUrlGeneration on Firefox with: \"group/name\" ,\"group-name\"")]
        [NUnit.Framework.TestCaseAttribute("Chrome", "group/name", "group-name", null, Category="Chrome", TestName="AutomaticUrlGeneration on Chrome with: \"group/name\" ,\"group-name\"")]
        [NUnit.Framework.TestCaseAttribute("IE", "group\\Came", "group-came", null, Category="IE", TestName="AutomaticUrlGeneration on IE with: \"group\\Came\" ,\"group-came\"")]
        [NUnit.Framework.TestCaseAttribute("Firefox", "group\\Came", "group-came", null, Category="Firefox", TestName="AutomaticUrlGeneration on Firefox with: \"group\\Came\" ,\"group-came\"")]
        [NUnit.Framework.TestCaseAttribute("Chrome", "group\\Came", "group-came", null, Category="Chrome", TestName="AutomaticUrlGeneration on Chrome with: \"group\\Came\" ,\"group-came\"")]
        public virtual void AutomaticUrlGeneration(string browser, string title, string expectedUrl, string[] exampleTags)
        {
            InitializeSelenium(browser);
            string[] @__tags = new string[] {
                    "Browser:IE",
                    "Browser:Firefox",
                    "Browser:Chrome"};
            if ((exampleTags != null))
            {
                @__tags = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(@__tags, exampleTags));
            }
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Automatic url generation", @__tags);
#line 31
this.ScenarioSetup(scenarioInfo);
#line 32
 testRunner.Given("I navigated to /repository", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "Title",
                        "Description"});
            table3.AddRow(new string[] {
                        string.Format("{0}", title),
                        ""});
#line 33
 testRunner.And("I have entered the following information:", ((string)(null)), table3, "And ");
#line 36
 testRunner.Then(string.Format("I should see the url \"{0}\"", expectedUrl), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
