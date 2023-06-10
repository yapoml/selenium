using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Yapoml.Selenium.Sample
{
    [TestFixture]
    public class NuGetSearchTest
    {
        private IWebDriver _webDriver;

        [SetUp]
        public void SetUp()
        {
            _webDriver = new ChromeDriver();
        }

        [TearDown]
        public void TearDown()
        {
            _webDriver?.Quit();
        }

        [Test]
        public void SearchWithSelenium()
        {
            _webDriver.Navigate().GoToUrl("https://nuget.org");

            _webDriver.FindElement(By.Id("search")).SendKeys("selenium");
            _webDriver.FindElement(By.CssSelector(".btn-search")).Click();

            var packages = _webDriver.FindElements(By.CssSelector(".package"));

            Assert.That(packages.Count, Is.EqualTo(20));

            foreach (var package in packages)
            {
                Assert.That(package.FindElement(By.XPath(".//a")).Text, Is.Not.Empty);
                Assert.That(package.FindElement(By.CssSelector(".package-details")).Text, Is.Not.Empty);

                var tags = package.FindElements(By.CssSelector(".package-tags a"));

                foreach (var tag in tags)
                {
                    Assert.That(tag.Text, Is.Not.Empty);
                }
            }
        }

        [Test]
        public void SearchWithYapoml()
        {
            _webDriver.Ya(opts => opts.WithBaseUrl("https://nuget.org"))
                .HomePage.Open().Search("selenium")
                .Packages.Expect(that => that.Count.Is(20).All(package =>
                    {
                        package.Title.Text.IsNot("");
                        package.Description.Text.IsNot("");
                        package.Tags.All(tag => tag.Text.IsNot(""));
                    }));
        }
    }
}