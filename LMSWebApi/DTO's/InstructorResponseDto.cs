namespace LMSWebApi.DTO_s
{
    public class InstructorResponseDto
    {
        public int UserId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public int Courses { get; set; }

        public int Students { get; set; }

        public string ApprovalStatus { get; set; }
    }
}
