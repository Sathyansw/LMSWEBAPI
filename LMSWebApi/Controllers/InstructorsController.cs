using LMSWebApi.Data;
using LMSWebApi.DTO_s;
using LMSWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMSWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin")]
    public class InstructorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InstructorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Instructors
        [HttpGet]
        public async Task<IActionResult> GetInstructors()
        {
            var instructors = await _context.Users
                .Where(u => u.RoleId == 2)
                .Select(u => new InstructorResponseDto
                {
                    UserId = u.UserId,

                    Name = u.FirstName + " " + u.LastName,

                    Email = u.Email,

                    Courses = u.Courses.Count(),

                    Students = _context.Enrollments
                        .Where(e => e.Course.InstructorId == u.UserId)
                        .Select(e => e.StudentId)
                        .Distinct()
                        .Count(),

                    ApprovalStatus =
                        u.ApprovalStatus == "Approved"
                            ? "Approved"
                            : "Pending"

                })
                .ToListAsync();

            return Ok(instructors);
        }

        // GET: api/Instructors/search?keyword=john
        [HttpGet("search")]
        public async Task<IActionResult> SearchInstructors(
            [FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Search keyword is required.");
            }

            var instructors = await _context.Users
                .Where(u =>
                    u.RoleId == 2 &&
                    (
                        u.FirstName.Contains(keyword) ||
                        u.LastName.Contains(keyword) ||
                        u.Email.Contains(keyword)
                    ))
                .Select(u => new InstructorResponseDto
                {
                    UserId = u.UserId,

                    Name = u.FirstName + " " + u.LastName,

                    Email = u.Email,

                    Courses = u.Courses.Count(),

                    Students = _context.Enrollments
                        .Where(e => e.Course.InstructorId == u.UserId)
                        .Select(e => e.StudentId)
                        .Distinct()
                        .Count(),

                    ApprovalStatus =
                        u.ApprovalStatus == "Approved"
                            ? "Approved"
                            : "Pending"
                })
                .ToListAsync();

            return Ok(instructors);
        }

        // GET: api/Instructors/10
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInstructor(int id)
        {
            var instructor = await _context.Users
                .Where(u =>
                    u.UserId == id &&
                    u.RoleId == 2)
                .Select(u => new InstructorResponseDto
                {
                    UserId = u.UserId,

                    Name = u.FirstName + " " + u.LastName,

                    Email = u.Email,

                    Courses = u.Courses.Count(),

                    Students = _context.Enrollments
                        .Where(e => e.Course.InstructorId == u.UserId)
                        .Select(e => e.StudentId)
                        .Distinct()
                        .Count(),

                    ApprovalStatus =
                        u.ApprovalStatus == "Approved"
                            ? "Approved"
                            : "Pending"
                })
                .FirstOrDefaultAsync();

            if (instructor == null)
            {
                return NotFound("Instructor not found.");
            }

            return Ok(instructor);
        }

        // POST: api/Instructors
        [HttpPost]
        public async Task<IActionResult> CreateInstructor(
            CreateInstructorDto dto)
        {
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == dto.Email);

            if (emailExists)
            {
                return BadRequest("Email already exists.");
            }

            var instructorRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleId == 2);

            if (instructorRole == null)
            {
                return BadRequest("Instructor role not found.");
            }

            var instructor = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,

                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(dto.Password),

                RoleId = 2,

                IsActive = false,

                ApprovalStatus = "Pending",

                CreatedDate = DateTime.UtcNow
            };

            _context.Users.Add(instructor);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Instructor created successfully and is pending approval.",
                userId = instructor.UserId
            });
        }
        // PUT: api/Instructors/12/approve
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveInstructor(int id)
        {
            var instructor = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.UserId == id &&
                    u.RoleId == 2);

            if (instructor == null)
            {
                return NotFound("Instructor not found.");
            }

            instructor.ApprovalStatus = "Approved";

            instructor.IsActive = true;

            await _context.SaveChangesAsync();

            return Ok("Instructor approved successfully.");
        }

        // PUT: api/Instructors/12/deactivate
        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> DeactivateInstructor(int id)
        {
            var instructor = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.UserId == id &&
                    u.RoleId == 2);

            if (instructor == null)
            {
                return NotFound("Instructor not found.");
            }

            instructor.IsActive = false;

            await _context.SaveChangesAsync();

            return Ok("Instructor deactivated successfully.");
        }


        // PUT: api/Instructors/12
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInstructor(
            int id,
            UpdateInstructorDto dto)
        {
            var instructor = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.UserId == id &&
                    u.RoleId == 2);

            if (instructor == null)
            {
                return NotFound("Instructor not found.");
            }

            var emailExists = await _context.Users
                .AnyAsync(u =>
                    u.Email == dto.Email &&
                    u.UserId != id);

            if (emailExists)
            {
                return BadRequest("Email already exists.");
            }

            instructor.FirstName = dto.FirstName;

            instructor.LastName = dto.LastName;

            instructor.Email = dto.Email;

            await _context.SaveChangesAsync();

            return Ok("Instructor updated successfully.");
        }

        // GET: api/Instructors/12/courses
        [HttpGet("{id}/courses")]
        public async Task<IActionResult> GetInstructorCourses(int id)
        {
            var instructorExists = await _context.Users
                .AnyAsync(u =>
                    u.UserId == id &&
                    u.RoleId == 2);

            if (!instructorExists)
            {
                return NotFound("Instructor not found.");
            }

            var courses = await _context.Courses
                .Where(c => c.InstructorId == id)
                .Select(c => new
                {
                    c.CourseId,
                    c.CourseName,
                    c.Description,
                    c.IsPublished,
                    c.CreatedDate,

                    Students = _context.Enrollments
                        .Where(e => e.CourseId == c.CourseId)
                        .Select(e => e.StudentId)
                        .Distinct()
                        .Count()
                })
                .ToListAsync();

            return Ok(courses);
        }
    }
}
