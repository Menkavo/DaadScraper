namespace DAADScraper.Core
{
    public class DtoDaadAPICourse
    {
        public int Id { get; set; }

        public string CourseName { get; set; }

        public string Academy { get; set; }

        public string City { get; set; }

        public string Beginning { get; set; }

        public string ProgrammeDuration { get; set; }

        public string TuitionFees { get; set; }

        public string ApplicationDeadline { get; set; }

        public string[] SupportInternationalStudents { get; set; }

        public string Subject { get; set; }

        public string Link { get; set; }
    }
}