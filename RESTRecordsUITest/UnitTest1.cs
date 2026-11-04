
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace RESTRecordsUITest
{
    public class RecordsSearchTests : IDisposable
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        private string url = "http://127.0.0.1:5500/index.html";

        public RecordsSearchTests()
        {
            driver = new ChromeDriver();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        public void Dispose()
        {
            driver.Quit();
            driver.Dispose();
        }

        [Fact]
        public void BrowserOpensProject_WithTitleRecords()
        {
            driver.Navigate().GoToUrl(url);

            Assert.Equal("Records", driver.Title);
        }

        [Fact]
        public void SearchByTitle_ShowsOnlyMatchingRecord()
        {
            driver.Navigate().GoToUrl(url);
            LoginBeforeSearch();

            // Wait for the three records to load
            wait.Until(browser =>
                browser.FindElements(By.CssSelector("#recordsBody tr")).Count == 3);

            // Select search type
            IWebElement searchType = driver.FindElement(By.Id("searchType"));
            new SelectElement(searchType).SelectByValue("title");

            // Enter search value
            IWebElement searchInput = driver.FindElement(By.Id("searchInput"));
            searchInput.SendKeys("Song 2");

            // Click search
            IWebElement searchButton = driver.FindElement(By.Id("searchButton"));
            searchButton.Click();

            // Wait for one matching result
            wait.Until(browser =>
                browser.FindElements(By.CssSelector("#recordsBody tr")).Count == 1);

            // Check result
            var rows = driver.FindElements(By.CssSelector("#recordsBody tr"));

            Assert.Single(rows);

            var cells = rows[0].FindElements(By.TagName("td"));

            Assert.Equal("Song 2", cells[1].Text);
            Assert.Equal("Artist B", cells[2].Text);
        }


        [Fact]
        public void SearchByArtist_ShowsOnlyMatchingRecord()
        {
            driver.Navigate().GoToUrl(url);
            LoginBeforeSearch();

            // Wait for the three records to load
            wait.Until(browser =>
                browser.FindElements(By.CssSelector("#recordsBody tr")).Count == 3);

            // Select search type
            IWebElement searchType = driver.FindElement(By.Id("searchType"));
            new SelectElement(searchType).SelectByValue("artist");

            // Enter search value
            IWebElement searchInput = driver.FindElement(By.Id("searchInput"));
            searchInput.SendKeys("Artist B");

            // Click search
            IWebElement searchButton = driver.FindElement(By.Id("searchButton"));
            searchButton.Click();

            // Wait for one matching result
            wait.Until(browser =>
                browser.FindElements(By.CssSelector("#recordsBody tr")).Count == 1);

            // Check result
            var rows = driver.FindElements(By.CssSelector("#recordsBody tr"));

            Assert.Single(rows);

            var cells = rows[0].FindElements(By.TagName("td"));

            Assert.Equal("Song 2", cells[1].Text);
            Assert.Equal("Artist B", cells[2].Text);
        }


        [Fact]
        public void SearchByMaxDuration_ShowsMatchingRecords()
        {
            driver.Navigate().GoToUrl(url);
            LoginBeforeSearch();

            // Wait for the three records to load
            wait.Until(browser =>
                browser.FindElements(By.CssSelector("#recordsBody tr")).Count == 3);

            // Select search type
            IWebElement searchType = driver.FindElement(By.Id("searchType"));
            new SelectElement(searchType).SelectByValue("maxDuration");

            // Enter search value
            IWebElement searchInput = driver.FindElement(By.Id("searchInput"));
            searchInput.SendKeys("210");

            // Click search
            IWebElement searchButton = driver.FindElement(By.Id("searchButton"));
            searchButton.Click();

            // Wait for two matching results
            wait.Until(browser =>
                browser.FindElements(By.CssSelector("#recordsBody tr")).Count == 2);

            // Check results
            var rows = driver.FindElements(By.CssSelector("#recordsBody tr"));

            Assert.Equal(2, rows.Count);

            Assert.Contains(rows, row =>
                row.FindElements(By.TagName("td"))[1].Text == "Song 1");

            Assert.Contains(rows, row =>
                row.FindElements(By.TagName("td"))[1].Text == "Song 2");
        }


        [Fact]
        public void SearchByPublicationYear_ShowsOnlyMatchingRecord()
        {
            driver.Navigate().GoToUrl(url);
            LoginBeforeSearch();

            // Wait for the three records to load
            wait.Until(browser =>
                browser.FindElements(By.CssSelector("#recordsBody tr")).Count == 3);

            // Select search type
            IWebElement searchType = driver.FindElement(By.Id("searchType"));
            new SelectElement(searchType).SelectByValue("publicationYear");

            // Enter search value
            IWebElement searchInput = driver.FindElement(By.Id("searchInput"));
            searchInput.SendKeys("2020");

            // Click search
            IWebElement searchButton = driver.FindElement(By.Id("searchButton"));
            searchButton.Click();

            // Wait for one matching result
            wait.Until(browser =>
                browser.FindElements(By.CssSelector("#recordsBody tr")).Count == 1);

            // Check result
            var rows = driver.FindElements(By.CssSelector("#recordsBody tr"));

            Assert.Single(rows);

            var cells = rows[0].FindElements(By.TagName("td"));

            Assert.Equal("Song 1", cells[1].Text);
            Assert.Equal("2020", cells[4].Text);
        }


        [Fact]
        public void SearchByTitle_NoMatch_ShowsNoRecords()
        {
            driver.Navigate().GoToUrl(url);
            LoginBeforeSearch();

            // Wait for the three records to load
            wait.Until(browser =>
                browser.FindElements(By.CssSelector("#recordsBody tr")).Count == 3);

            // Select search type
            IWebElement searchType = driver.FindElement(By.Id("searchType"));
            new SelectElement(searchType).SelectByValue("title");

            // Enter search value
            IWebElement searchInput = driver.FindElement(By.Id("searchInput"));
            searchInput.SendKeys("NonExistingSong");

            // Click search
            IWebElement searchButton = driver.FindElement(By.Id("searchButton"));
            searchButton.Click();

            // Wait for no results
            wait.Until(browser =>
                browser.FindElements(By.CssSelector("#recordsBody tr")).Count == 0);

            // Check result
            var rows = driver.FindElements(By.CssSelector("#recordsBody tr"));

            Assert.Empty(rows);
        }


        private void LoginBeforeSearch()
        {
            // Vent på, at Vue har vist loginformularen og skjult records.
            wait.Message = "Loginformularen blev ikke klar før søgningen.";
            wait.Until(browser =>
                browser.FindElements(By.Id("recordsBody")).Count == 0
                && browser.FindElement(
                    By.CssSelector("#app input[placeholder='Username']")).Displayed);

            IWebElement usernameInput = driver.FindElement(
                By.CssSelector("#app input[placeholder='Username']"));
            usernameInput.Clear();
            usernameInput.SendKeys("admin");

            IWebElement passwordInput = driver.FindElement(
                By.CssSelector("#app input[type='password']"));
            passwordInput.Clear();
            passwordInput.SendKeys("1234");

            IWebElement loginButton = driver.FindElement(
                By.CssSelector("#app .card button"));
            loginButton.Click();

            wait.Message = "De tre records blev ikke indlæst efter login.";
        }

    }
}
