using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace DAADScraper.Core.Handlers
{
    public class DaadHandler
    {
        public DaadHandler() => _selenium = new SeleniumHandler();

        private readonly SeleniumHandler _selenium;

        private void Scrape(string mainPageUrl)
        {
            _selenium.GoToUrl(mainPageUrl);

            // Collect course links from main page
            var links = new List<string>();
            var courses = _selenium.GetElementsByXPath("//*[ @class='c-ad-carousel c-ad-carousel--list mb-5 pr-0 pt-3' ]");
            foreach (var course in courses)
            {
                var sixthDiv = course.FindElement(By.XPath("./div/div/div/div/div/div"));
                var link = sixthDiv.FindElement(By.TagName("a"));

                var href = link.GetAttribute("href");
                var linkText = link.Text;
                links.Add(href);
            }


            // Get all course link elements
            var linkElements = _selenium.GetElementsByXPath("//div[contains(@class,'c-result-list__content')]/div/div[6]/a");

            var courseLinks = new List<string>();
            foreach (var linkEl in linkElements)
            {
                var href = linkEl.GetAttribute("href");
                if (!string.IsNullOrEmpty(href))
                    courseLinks.Add(href);
            }

            // Loop through each course page
            foreach (var link in courseLinks)
            {
                _selenium.GoToUrl(link);

                // --- Overview tab ---
                string courseName = _selenium.GetTextByXPath("//h1");
                string university = _selenium.GetTextByXPath("//div[contains(@class, 'institution')]");
                string location = _selenium.GetTextByXPath("//dt[text()='Course location']/following-sibling::dd");
                string tuition = _selenium.GetTextByXPath("//dt[text()='Tuition fees per semester in EUR']/following-sibling::dd");

                // --- Requirements / Registration tab ---
                _selenium.ClickItemWithXPath("//a[contains(text(),'Requirements / Registration')]");
                _selenium.WaitForElement("//dt[text()='Application deadline']");
                string deadline = _selenium.GetTextByXPath("//dt[text()='Application deadline']/following-sibling::dd");

                string submitToText = _selenium.GetTextByXPath("//dt[text()='Submit application to']/following-sibling::dd");
                if (_selenium.ElementExists("//dt[text()='Submit application to']/following-sibling::dd//a"))
                {
                    string linkHref = _selenium.GetAttributeByXPath("//dt[text()='Submit application to']/following-sibling::dd//a", "href");
                    string linkText = _selenium.GetTextByXPath("//dt[text()='Submit application to']/following-sibling::dd//a");
                    submitToText = $"=HYPERLINK(\"{linkHref}\", \"{linkText}\")";
                }

                // Send to Google Sheets
                GoogleSheetsHandler.AppendRow(new List<object>
                {
                    courseName, university, location, tuition, deadline, submitToText
                });
            }
        }


        public void Begin()
        {
            var filteredPage = @"https://www2.daad.de/deutschland/studienangebote/international-programmes/en/result/?q=Computer%20Science&degree%5B%5D=2&lang%5B%5D=2&cert=&admReq=&langExamPC=&langExamLC=&langExamSC=&subjectGroup%5B%5D=&fos%5B%5D=&langDeAvailable=&langEnAvailable=&lvlEn%5B%5D=&modStd%5B%5D=7&cit%5B%5D=&tyi%5B%5D=&ins%5B%5D=&fee=2&bgn%5B%5D=2&dat%5B%5D=&prep_subj%5B%5D=&prep_degree%5B%5D=&sort=4&dur=&subjects%5B%5D=&limit=10&offset=&display=list";
            Scrape(filteredPage);
        }
    }
}