using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace DAADScraper.Core.Handlers
{
    public class SeleniumHandler
    {
        protected IWebDriver Driver;
        protected WebDriverWait Wait;

        public SeleniumHandler()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless=new");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-gpu");
            Driver = new ChromeDriver();
            Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(100));
        }

        public void GoToUrl(string url)
        {
            Driver.Navigate().GoToUrl(url);
        }

        public void ClickItemWithXPath(string xPath) => Wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(xPath))).Click();

        public void ClickItemWithId(string id) => Wait.Until(ExpectedConditions.ElementToBeClickable(By.Id(id))).Click();

        public string GetTextByXPath(string xPath) => Wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xPath))).Text.Trim();

        public string GetTextByCssSelector(string selector) => Wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(selector))).Text.Trim();

        public bool ElementExists(string xPath)
        {
            try
            {
                Driver.FindElement(By.XPath(xPath));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void WaitForElementWithXPath(string xPath) => Wait.Until(ExpectedConditions.ElementExists(By.XPath(xPath)));

        public void WaitForElementWithCssSelector(string selector) => Wait.Until(ExpectedConditions.ElementExists(By.CssSelector(selector)));

        public string GetAttributeByXPath(string xPath, string attribute) => Wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xPath))).GetAttribute(attribute);

        public IReadOnlyCollection<IWebElement> GetElementsByXPath(string xPath)
        {
            var element = Driver.FindElements(By.XPath(xPath));
            if (element == null)
                Console.WriteLine($"Element doesn't exist. XPath : {xPath}");
            return element;

            //return Wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath(xPath)));
        }

        public IReadOnlyCollection<IWebElement> GetElementsByCssSelector(string cssSelector)
        {
            var elements = Driver.FindElements(By.CssSelector(cssSelector));
            if (elements == null)
                Console.WriteLine($"Element doesn't exist. XPath : {cssSelector}");
            return elements;

            //return Wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath(xPath)));
        }

        public void ScrollToTheEnd() => ((IJavaScriptExecutor)Driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");

        public void Dispose() => Driver.Quit();
    }
}