using LMSWebApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMSWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public DashboardController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAdminDashboard()
        {
            var totalUsers = await _applicationDbContext.Users
                .CountAsync();

            var totalStudents = await _applicationDbContext.Users
                .CountAsync(x => x.Roles.RoleName == "Student");

            var totalInstructors = await _applicationDbContext.Users
                .CountAsync(x => x.Roles.RoleName == "Instructor");

            var totalCourses = await _applicationDbContext.Courses
                .CountAsync();

            var totalEnrollments = await _applicationDbContext.Enrollments
                .CountAsync();

            var totalCertificates = await _applicationDbContext.Certificates
                .CountAsync();

            return Ok(new
            {
                TotalUsers = totalUsers,
                TotalCourses = totalCourses,
                TotalEnrollments = totalEnrollments,
                TotalStudents = totalStudents,
                TotalInstructors = totalInstructors,
                TotalCertificates = totalCertificates
            });
        }
    }
}
