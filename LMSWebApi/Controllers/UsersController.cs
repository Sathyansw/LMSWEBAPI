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
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.Roles)
                .Select(u => new UserResponseDto
                {
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    RoleId = u.RoleId,
                    RoleName = u.Roles!.RoleName,
                    IsActive = u.IsActive,
                    CreatedDate = u.CreatedDate
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .Where(u => u.UserId == id)
                .Select(u => new UserResponseDto
                {
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    RoleId = u.RoleId,
                    RoleName = u.Roles!.RoleName,
                    IsActive = u.IsActive,
                    CreatedDate = u.CreatedDate
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto dto)
        {
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == dto.Email);

            if (emailExists)
            {
                return BadRequest("Email already exists.");
            }

            var roleExists = await _context.Roles
                .AnyAsync(r => r.RoleId == dto.RoleId);

            if (!roleExists)
            {
                return BadRequest("Invalid RoleId.");
            }

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = dto.RoleId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User created successfully.",
                userId = user.UserId
            });
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(
            int id,
            UpdateUserDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var emailExists = await _context.Users
                .AnyAsync(u =>
                    u.Email == dto.Email &&
                    u.UserId != id);

            if (emailExists)
            {
                return BadRequest("Email already exists.");
            }

            var roleExists = await _context.Roles
                .AnyAsync(r => r.RoleId == dto.RoleId);

            if (!roleExists)
            {
                return BadRequest("Invalid RoleId.");
            }

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;
            user.RoleId = dto.RoleId;

            await _context.SaveChangesAsync();

            return Ok("User updated successfully.");
        }
        // PUT: api/Users/5/deactivate
        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.IsActive = false;

            await _context.SaveChangesAsync();

            return Ok("User deactivated successfully.");
        }

        [HttpPut("{id}/activate")]
        public async Task<IActionResult> ActivateUser(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.IsActive = true;

            await _context.SaveChangesAsync();

            return Ok("User activated successfully.");
        }
        // PUT: api/Users/5/role
        [HttpPut("{id}/role")]
        public async Task<IActionResult> ChangeRole(
            int id,
            ChangeUserRoleDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleId == dto.RoleId);

            if (role == null)
            {
                return BadRequest("Invalid role.");
            }

            user.RoleId = dto.RoleId;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User role updated successfully.",
                userId = user.UserId,
                roleId = role.RoleId,
                roleName = role.RoleName
            });
        }

        // GET: api/Users/search?keyword=sathya
        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers(
            [FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Search keyword is required.");
            }

            var users = await _context.Users
                .Include(u => u.Roles)
                .Where(u =>
                    u.FirstName.Contains(keyword) ||
                    u.LastName.Contains(keyword) ||
                    u.Email.Contains(keyword) ||
                    u.Roles!.RoleName.Contains(keyword))
                .Select(u => new UserResponseDto
                {
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    RoleId = u.RoleId,
                    RoleName = u.Roles!.RoleName,
                    IsActive = u.IsActive,
                    CreatedDate = u.CreatedDate
                })
                .ToListAsync();

            return Ok(users);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return Ok("User deleted successfully.");
        }
    }
}
