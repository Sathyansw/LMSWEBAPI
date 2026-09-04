namespace LMSWebApi.Models
{
    public class Course
    {
        public int CourseId { get; set; }

        public string CourseName { get; set; }

        public string Description { get; set; }

        // Foreign Key
        public int InstructorId { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsPublished { get; set; }

        // Navigation property
        public User Instructor { get; set; }
    }
}
