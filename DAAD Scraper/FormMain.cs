using DAADScraper.Core;
using DAADScraper.Core.Handlers;
using OfficeOpenXml;
using System.Text.Json;
using System.Threading.Tasks;

namespace DAAD_Scraper
{
    public partial class FormMain : Form
    {
        private List<DtoDaadCourse>? _courses;
        int _currentIndex = 0;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            _courses = DaadHandler.GetCoursesFromJSON();
            LblCount.Text = $" Of {_courses.Count}";
            TxtIndex.Text = "1";
            _currentIndex = 0;
            LoadCourse();
        }

        private async void BtnStart_Click(object sender, EventArgs e)
        {

        }

        private void BtnNext_Click(object sender, EventArgs e) => SaveAndMoveNext();

        private void TxtReplacements_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            SaveAndMoveNext();
        }

        private void TxtIndex_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData != Keys.Enter) return;
            _currentIndex = Convert.ToInt32(TxtIndex.Text) - 1;
            LoadCourse();
        }


        private void SaveAndMoveNext()
        {
            _courses[_currentIndex].ApplicationDeadline = TxtSource.Text;
            _currentIndex++;
            SaveCourses();
            TxtIndex.Text = (_currentIndex + 1).ToString();
            LoadCourse();
        }

        private void LoadCourse()
        {
            TxtSource.Text = _courses[_currentIndex].ApplicationDeadline.Replace("&rsquo;", "'").Replace("&#39;", "'")
                .Replace("&ndash;", "-").Replace("&shy;", "").Replace("&nbsp;", " ").Replace("&amp;", "&").Replace("&quot;","\"");
        }

        private void SaveCourses()
        {
            var path = "D:\\Arya\\Files\\German universities updated.txt";
            var excelPath = "D:\\Arya\\Files\\GermanUnis.xlsx";
            var json = JsonSerializer.Serialize(_courses, new JsonSerializerOptions
            {
                WriteIndented = true // Makes it pretty-printed
            });
            File.WriteAllText(path, json);
            ExportCoursesToExcel(_courses, excelPath);
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
    }
}