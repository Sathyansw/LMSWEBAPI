using System.Data;

namespace LMSWebApi.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public int RoleId { get; set; }

        public Role Roles { get; set; }
        public string? RefreshToken { get; set; } = string.Empty;
        public DateTime? RefreshTokenExpiryTime { get; set; } = DateTime.UtcNow.AddDays(7);
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // One instructor can have many courses
        public ICollection<Course> Courses { get; set; }
        public string ApprovalStatus { get; set; } = "Pending";
    }
}
