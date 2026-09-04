namespace LMSWebApi.Models
{
    public class Certificate
    {
        public int CertificateId { get; set; }

        public string CertificateNumber { get; set; }

        // Student
        public int StudentId { get; set; }

        // Course
        public int CourseId { get; set; }

        public DateTime IssuedDate { get; set; }


        // Navigation Properties

        public User Student { get; set; }

        public Course Course { get; set; }
    }
}
