using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace RESTRecordsUITest
{
    public class RecordsSearchTests : IDisposable
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;

        private readonly string url =
            "http://127.0.0.1:5500/index.html";

        public RecordsSearchTests()
        {
            driver = new ChromeDriver();

            wait = new WebDriverWait(
                driver,
                TimeSpan.FromSeconds(10));
        }

        public void Dispose()
        {
            driver.Quit();
            driver.Dispose();
        }

        [Fact]
        public void BrowserOpensProject_WithTitleRecords()
        {
            // Act
            driver.Navigate().GoToUrl(url);

            wait.Until(browser =>
                browser.Title == "Records");

            // Assert
            Assert.Equal("Records", driver.Title);
        }

        [Fact]
        public void SearchByTitle_ShowsOnlyMatchingRecord()
        {
            // Arrange
            driver.Navigate().GoToUrl(url);

            wait.Message = "De tre eksempeludgivelser blev ikke indlæst.";

            wait.Until(browser =>
                browser.FindElement(By.Id("recordsBody"))
                    .FindElements(By.TagName("tr")).Count == 3);

            // Act
            var searchType = new SelectElement(
                driver.FindElement(
                    By.Id("searchType")));

            searchType.SelectByValue("title");

            var searchInput = driver.FindElement(
                By.Id("searchInput"));

            searchInput.Clear();
            searchInput.SendKeys("Song 2");

            driver.FindElement(
                By.Id("searchButton"))
                .Click();

            wait.Message = "Søgningen viste ikke én række med Song 2.";

            wait.Until(browser =>
            {
                var rows = browser.FindElement(By.Id("recordsBody"))
                    .FindElements(By.TagName("tr"));

                return rows.Count == 1
                    && rows[0].FindElements(By.TagName("td"))[1]
                        .Text == "Song 2";
            });

            // Assert
            var resultRows = driver.FindElement(By.Id("recordsBody"))
                .FindElements(By.TagName("tr"));

            var row = Assert.Single(resultRows);
            var cells = row.FindElements(By.TagName("td"));

            Assert.Equal("Song 2", cells[1].Text);
            Assert.Equal("Artist B", cells[2].Text);
        }
    }
}