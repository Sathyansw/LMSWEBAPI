using LMSWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LMSWebApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> contextOptions)
            : base(contextOptions)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        public DbSet<Certificate> Certificates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>().HasData(
            new Role
            {
                RoleId = 1,
                RoleName = "Admin"
            },
            new Role
            {
                RoleId = 2,
                RoleName = "Instructor"
            },
            new Role
            {
                RoleId = 3,
                RoleName = "Student"
            }
        );
            // one role for many users => one to many relationship
            modelBuilder.Entity<User>()
                .HasOne(u => u.Roles)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            // one instructor for many courses => one to many relationship
            modelBuilder.Entity<Course>()
               .HasOne(u => u.Instructor)
               .WithMany(r => r.Courses)
               .HasForeignKey(u => u.InstructorId);

            // A student can enroll in multiple courses //And a course can have multiple students:  => many to many relationship

            // Student → Enrollment
            // One Student can have many Enrollments
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany()
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Course → Enrollment
            // One Course can have many Enrollments
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany()
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            //One Student can have many Certificates
            //One Course can have many Certificates
            //Each Certificate belongs to one Student
            //Each Certificate belongs to one Course
            // Student → Certificate

            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.Student)
                .WithMany()
                .HasForeignKey(c => c.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Course → Certificate
            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.Course)
                .WithMany()
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.Restrict);


            //Code to remove column
            //modelBuilder.Entity<User>()
            //.Ignore(u => u.Status);


            //Code to add column
            //modelBuilder.Entity<User>()
            //   .Property(u => u.IsActive)
            //   .HasDefaultValue(true);

            //Code to rename column
            modelBuilder.Entity<User>()
            .Property(u => u.PasswordHash)
            .HasColumnName("PasswordHash");

            // Code to add column ApprovalStatus
            modelBuilder.Entity<User>()
            .Property(u => u.ApprovalStatus)
            .HasDefaultValue("Pending");

        }
    }
}
