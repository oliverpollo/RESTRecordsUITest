using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace RESTRecordsUITest
{
    public class LoginUITest : IDisposable
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;

        private readonly string url = "http://127.0.0.1:5500/index.html";
        private readonly string username = "admin";
        private readonly string password = "1234";

        public LoginUITest()
        {
            driver = new ChromeDriver();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
        }

        public void Dispose()
        {
            driver.Quit();
            driver.Dispose();
        }

        [Fact]
        public void BeforeLogin_ShowsLoginAndHidesRecords()
        {
            // Arrange / Act
            OpenLoginPage();

            // Assert
            Assert.True(driver.FindElement(
                By.CssSelector("#app input[placeholder='Username']")).Displayed);
            Assert.True(driver.FindElement(
                By.CssSelector("#app input[type='password']")).Displayed);
            Assert.Equal("Login", driver.FindElement(
                By.CssSelector("#app .card button")).Text);
            Assert.Empty(driver.FindElements(By.Id("recordsBody")));
            Assert.Empty(driver.FindElements(By.Id("searchType")));
            Assert.Empty(driver.FindElements(
                By.CssSelector("#app button.btn-danger")));
        }

        [Fact]
        public void Login_WithCorrectCredentials_ShowsUserRoleAndRecords()
        {
            // Arrange / Act
            Login(username, password);

            wait.Message = "Login viste ikke records. Kontrollér login og Azure-API'et.";
            wait.Until(browser =>
                browser.FindElements(By.CssSelector("#recordsBody tr")).Count > 0);

            // Assert
            IWebElement userInformation = driver.FindElement(
                By.CssSelector("#app > div > p"));

            Assert.Contains("Logged in as " + username, userInformation.Text);
            Assert.Contains("(Admin)", userInformation.Text);
            Assert.True(driver.FindElement(By.Id("searchType")).Displayed);
            Assert.NotEmpty(driver.FindElements(
                By.CssSelector("#recordsBody tr")));
            Assert.Empty(driver.FindElements(
                By.CssSelector("#app input[type='password']")));
        }

        [Fact]
        public void Login_WithWrongPassword_ShowsErrorAndHidesRecords()
        {
            // Arrange / Act
            Login(username, "wrong-password");

            wait.Message = "Forkert adgangskode viste ikke den forventede loginfejl.";
            wait.Until(browser =>
                browser.FindElement(By.CssSelector("#app .card p.text-danger"))
                    .Text == "Forkert brugernavn eller adgangskode.");

            // Assert
            IWebElement errorMessage = driver.FindElement(
                By.CssSelector("#app .card p.text-danger"));

            Assert.Equal("Forkert brugernavn eller adgangskode.", errorMessage.Text);
            Assert.Empty(driver.FindElements(By.Id("recordsBody")));
            Assert.Empty(driver.FindElements(
                By.CssSelector("#app button.btn-danger")));
        }

        [Fact]
        public void Login_WithUnknownUsername_ShowsErrorAndHidesRecords()
        {
            // Arrange / Act
            Login("unknown-test-user", password);

            wait.Message = "Ukendt bruger viste ikke den forventede loginfejl.";
            wait.Until(browser =>
                browser.FindElement(By.CssSelector("#app .card p.text-danger"))
                    .Text == "Forkert brugernavn eller adgangskode.");

            // Assert
            IWebElement errorMessage = driver.FindElement(
                By.CssSelector("#app .card p.text-danger"));

            Assert.Equal("Forkert brugernavn eller adgangskode.", errorMessage.Text);
            Assert.Empty(driver.FindElements(By.Id("recordsBody")));
            Assert.Empty(driver.FindElements(
                By.CssSelector("#app button.btn-danger")));
        }

        [Fact]
        public void Logout_ShowsLoginAndHidesRecords()
        {
            // Arrange
            Login(username, password);

            wait.Message = "Records blev ikke vist før logout.";
            wait.Until(browser =>
                browser.FindElements(By.CssSelector("#recordsBody tr")).Count > 0);

            // Act
            IWebElement logoutButton = driver.FindElement(
                By.CssSelector("#app button.btn-danger"));
            logoutButton.Click();

            wait.Message = "Logout viste ikke loginformularen igen.";
            wait.Until(browser =>
                browser.FindElements(By.Id("recordsBody")).Count == 0
                && browser.FindElement(
                    By.CssSelector("#app input[placeholder='Username']")).Displayed);

            // Assert
            Assert.Empty(driver.FindElements(By.Id("recordsBody")));
            Assert.Empty(driver.FindElements(By.Id("searchType")));
            Assert.Empty(driver.FindElements(
                By.CssSelector("#app button.btn-danger")));
            Assert.Equal("", driver.FindElement(
                By.CssSelector("#app input[placeholder='Username']"))
                    .GetDomProperty("value"));
            Assert.Equal("", driver.FindElement(
                By.CssSelector("#app input[type='password']"))
                    .GetDomProperty("value"));
        }

        private void OpenLoginPage()
        {
            driver.Navigate().GoToUrl(url);

            // Vent på Vue: records skal være fjernet fra den færdige loginvisning.
            wait.Message = "Loginformularen blev ikke klar. Start Live Server på testens adresse.";
            wait.Until(browser =>
                browser.FindElements(By.Id("recordsBody")).Count == 0
                && browser.FindElement(
                    By.CssSelector("#app input[placeholder='Username']")).Displayed);
        }

        private void Login(string loginUsername, string loginPassword)
        {
            OpenLoginPage();

            IWebElement usernameInput = driver.FindElement(
                By.CssSelector("#app input[placeholder='Username']"));
            usernameInput.Clear();
            usernameInput.SendKeys(loginUsername);

            IWebElement passwordInput = driver.FindElement(
                By.CssSelector("#app input[type='password']"));
            passwordInput.Clear();
            passwordInput.SendKeys(loginPassword);

            IWebElement loginButton = driver.FindElement(
                By.CssSelector("#app .card button"));
            loginButton.Click();
        }
    }
}
