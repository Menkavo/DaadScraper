namespace DAADScraper.Core
{
    public class DtoDaadCourse
    {
        public int Id { get; set; }

        public string CourseName { get; set; }

        public string Academy { get; set; }

        public string City { get; set; }

        public string Beginning { get; set; }

        public string ProgrammeDuration { get; set; }

        public string TuitionFees { get; set; }

        public string ApplicationDeadline { get; set; }

        public string SupportInternationalStudents { get; set; }

        public string Subject { get; set; }

        public string DaadLink { get; set; }

        public string ApplicationLink { get; set; }
    }

    public class DtoDaadResult
    {
        public List<DtoDaadAPICourse> Courses { get; set; }
    }
}