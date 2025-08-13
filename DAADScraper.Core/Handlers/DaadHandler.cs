using OfficeOpenXml;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace DAADScraper.Core.Handlers
{
    public class DaadHandler
    {
        public DaadHandler()
        {
            _httpClient = new();
            _selenium = new();
        }

        private readonly HttpClient _httpClient;
        private readonly SeleniumHandler _selenium;

        private void Scrape(string mainPageUrl)
        {
            _selenium.GoToUrl(mainPageUrl);

            // Collect course links from main page
            var courseLinks = new List<string>();
            Thread.Sleep(2000);
            var courses = _selenium.GetElementsByXPath("//*[ @class='c-ad-carousel c-ad-carousel--list mb-5 pr-0 pt-3' ]");
            foreach (var course in courses)
            {
                var sixthDiv = course.FindElement(By.XPath("./div/div/div/div/div/div"));
                var link = sixthDiv.FindElement(By.TagName("a"));
                var href = link.GetAttribute("href");
                courseLinks.Add(href);
            }

            // Loop through each course page
            foreach (var courseLink in courseLinks)
            {
                _selenium.GoToUrl(courseLink);
                Thread.Sleep(500);

                // --- Overview tab ---
                var courseName = _selenium.GetTextByXPath("/html/body/div[2]/main/div[3]/div/div/div/div/h2/span[1]");
                var university = _selenium.GetTextByXPath("/html/body/div[2]/main/div[3]/div/div/div/div/h3");
                var location = _selenium.GetTextByXPath("//dt[contains(text(), 'Course location')]/following-sibling::dd[1]");
                var tuition = _selenium.GetTextByXPath("//dt[contains(text(), 'Tuition fees per semester in EUR')]/following-sibling::dd[1]");

                // --- Requirements / Registration tab ---
                _selenium.ClickItemWithId("registration-tab");

                //var deadlineBox = _selenium.GetElementsByXPath("/html/body/div[2]/main/div[5]/div/div/div/div[1]/div/div[2]/div[4]/div/dl/dd[3]").First();
                var deadlineBoxes = _selenium.GetElementsByXPath("//dt[contains(text(), 'Application deadline')]/following-sibling::dd[1]").First();

                //var deadlineBoxes = _selenium.GetElementsByXPath("//div[@id='registration')]/following-sibling::dd[1]").First();

                var deadlineElements = deadlineBoxes.FindElements(By.XPath("./p"));
                List<string> deadlineLinks = [];
                StringBuilder deadlineSb = new();
                foreach (var deadlineElement in deadlineElements)
                {
                    var text = deadlineElement.Text;
                    deadlineLinks.AddRange([.. (deadlineElement.FindElements(By.TagName("a")).Select(de => de.GetAttribute("href")) ?? new List<string>())]);
                    deadlineSb.AppendLine(text);
                }
                foreach (var deadlineLink in deadlineLinks)
                    deadlineSb.AppendLine(deadlineLink);

                var deadline = deadlineSb.ToString();

                var submitToBox = _selenium.GetElementsByXPath("//dt[text()='Submit application to']/following-sibling::dd").First();
                var submitToText = submitToBox.Text + "\n";
                var submitToLinks = submitToBox.FindElements(By.TagName("p")).SelectMany(p => p.FindElements(By.TagName("a")).Select(c => c.GetAttribute("href")));
                submitToText += string.Join("\n", submitToLinks);

            }
        }

        private void Scrape(DtoDaadCourse course)
        {
            _selenium.GoToUrl("https://www2.daad.de" + course.DaadLink);
            // --- Requirements / Registration tab ---
            _selenium.ClickItemWithId("registration-tab");

            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(course.ApplicationDeadline);
            StringBuilder sb = new();
            foreach (var pNode in htmlDoc.DocumentNode.SelectNodes("//p | //li"))
            {
                foreach (var child in pNode.ChildNodes)
                {
                    if (child.Name == "br")
                        sb.AppendLine();
                    else if (child.Name == "a")
                        sb.Append(child.GetAttributeValue("href", ""));
                    else
                        sb.Append(child.InnerText);
                }
                sb.AppendLine();
            }

            course.ApplicationDeadline = sb.ToString();

            var submitToBox = _selenium.GetElementsByXPath("//div[@id=\"registration\"]//dt[contains(text(), 'Submit application to')]/following-sibling::dd").First();
            var submitToParagraphs = submitToBox.FindElements(By.CssSelector("p"));
            var submitToText = "";
            foreach (var paragraph in submitToParagraphs)
            {
                // Get all child elements (including text and <a> tags)
                var childElements = paragraph.FindElements(By.XPath("./*"));

                // Split text into segments between links
                string paragraphHtml = paragraph.GetAttribute("innerHTML");

                // Use HtmlAgilityPack to parse and replace
                htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(paragraphHtml);

                foreach (var node in htmlDoc.DocumentNode.ChildNodes)
                {
                    if (node.Name == "a")
                    {
                        var linkText = node.InnerText;
                        var href = node.GetAttributeValue("href", "");
                        submitToText += href;
                    }
                    else
                        submitToText += node.InnerText;
                }
                submitToText += '\n';
            }
            course.ApplicationLink = submitToText;
        }

        private void Test(string mainPageUrl)
        {
            _selenium.GoToUrl(mainPageUrl);

            // Collect course links from main page
            var links = new List<string>();
            Thread.Sleep(2000);
            var courses = _selenium.GetElementsByXPath("//*[ @class='c-ad-carousel c-ad-carousel--list mb-5 pr-0 pt-3' ]");
            foreach (var course in courses)
            {
                var sixthDiv = course.FindElement(By.XPath("./div/div/div/div/div/div"));
                var link = sixthDiv.FindElement(By.TagName("a"));
                var href = link.GetAttribute("href");
                links.Add(href);
            }

            // Loop through each course page
            foreach (var link in links)
            {
                _selenium.GoToUrl(link);
                Thread.Sleep(1000);

                _selenium.ClickItemWithId("registration-tab");
                _selenium.WaitForElementWithCssSelector("#registration > div > dl > dd:nth-child(8)");
                _selenium.ScrollToTheEnd();
                Thread.Sleep(1000);

                //string submitToText = _selenium.GetTextByXPath("//dt[text()='Submit application to']/following-sibling::dd");
            }
        }

        private async Task<List<DtoDaadCourse>> GetDataFromAPI()
        {
            var apiUrl = "https://www2.daad.de/deutschland/studienangebote/international-programmes/api/solr/en/search.json?cert=&admReq=&langExamPC=&langExamLC=&langExamSC=&degree%5B%5D=2&langDeAvailable=&langEnAvailable=&lang%5B%5D=2&modStd%5B%5D=7&fee=2&bgn%5B%5D=2&sort=4&dur=&q=Computer%20Science&limit=1000&offset=&display=list&isElearning=&isSep=";
            return MapCourses((await _httpClient.GetFromJsonAsync<DtoDaadResult>(apiUrl)).Courses);
        }

        public async Task Begin()
        {
            var courses = await GetAndScrapeCoursesData();
            //var courses = GetCoursesFromJSON();
            ExportCoursesToExcel(courses, "D:\\Arya\\Files\\GermanUnis.xlsx");
        }

        private async Task<List<DtoDaadCourse>> GetAndScrapeCoursesData()
        {
            var courses = await GetDataFromAPI();
            var filteredPage = @"https://www2.daad.de/deutschland/studienangebote/international-programmes/en/result/?q=Computer%20Science&degree%5B%5D=2&lang%5B%5D=2&cert=&admReq=&langExamPC=&langExamLC=&langExamSC=&subjectGroup%5B%5D=&fos%5B%5D=&langDeAvailable=&langEnAvailable=&lvlEn%5B%5D=&modStd%5B%5D=7&cit%5B%5D=&tyi%5B%5D=&ins%5B%5D=&fee=2&bgn%5B%5D=2&dat%5B%5D=&prep_subj%5B%5D=&prep_degree%5B%5D=&sort=4&dur=&subjects%5B%5D=&limit=1&offset=&display=list";
            _selenium.GoToUrl(filteredPage);
            _selenium.ClickItemWithXPath("//button[@class = 'snoop-button qa-cookie-consent-accept-all snoop-button--primary']");

            foreach (var course in courses)
            {
                var completed = false;
                for (var i = 0; i < 10; i++)
                {
                    try
                    {
                        Scrape(course);
                        completed = true;
                    }
                    catch (Exception) { }
                    if (completed) break;
                }
            }
            return courses;
        }

        public static List<DtoDaadCourse>? GetCoursesFromJSON()
        {
            var jsonString = File.ReadAllText("D:\\Arya\\Files\\German universities.txt");
            var courses = JsonSerializer.Deserialize<List<DtoDaadCourse>>(jsonString);
            return courses;
        }

        [Obsolete]
        private void ExportCoursesToExcel(List<DtoDaadCourse> courses, string filePath)
        {
            ExcelPackage.License.SetNonCommercialPersonal("Arya");
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("DAAD Courses");

            // Get property names for headers
            var properties = typeof(DtoDaadCourse).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                ws.Cells[1, i + 1].Value = properties[i].Name;
                ws.Cells[1, i + 1].Style.Font.Bold = true;
            }

            // Fill rows
            for (int row = 0; row < courses.Count; row++)
            {
                for (int col = 0; col < properties.Length; col++)
                {
                    var value = properties[col].GetValue(courses[row]);
                    if (value is IEnumerable<string> arr && !(value is string))
                    {
                        ws.Cells[row + 2, col + 1].Value = string.Join(", ", arr);
                    }
                    else
                    {
                        ws.Cells[row + 2, col + 1].Value = value;
                    }
                }
            }

            // Auto-fit columns
            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            // Save file
            File.WriteAllBytes(filePath, package.GetAsByteArray());
        }

        private List<DtoDaadCourse> MapCourses(List<DtoDaadAPICourse> courses) => [.. courses.Select(c => new DtoDaadCourse
        {
            Id = c.Id,
            CourseName = c.CourseName,
            Academy = c.Academy,
            City = c.City,
            Beginning = c.Beginning,
            ProgrammeDuration = c.ProgrammeDuration,
            TuitionFees = c.TuitionFees,
            ApplicationDeadline = c.ApplicationDeadline,
            SupportInternationalStudents = string.Join("\n", c.SupportInternationalStudents.ToList()),
            Subject = c.Subject,
            DaadLink = c.Link,
        })];
    }
}