namespace LMSWebApi.Models
{
    public class Enrollment
    {
        public int EnrollmentId { get; set; }

        // Student
        public int StudentId { get; set; }

        // Course
        public int CourseId { get; set; }

        public DateTime EnrollmentDate { get; set; }

        public string Status { get; set; }

        public DateTime? CompletionDate { get; set; }


        // Navigation Properties

        public User Student { get; set; }

        public Course Course { get; set; }
    }
}
