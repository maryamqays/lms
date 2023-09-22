using Lms.Models;
using LMS.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LMS.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<InstructorUser> InstructorUsers { get; set; }


        public DbSet <Certificate> certificateUsers { get; set; }
        public DbSet<CourseCategory> CourseCategories { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<CourseChapter> CourseChapters { get; set; }

        public DbSet<CourseChapterContent> CourseChapterContent { get; set; }

        public DbSet<CourseComment> CourseComments { get; set; }

        public DbSet<CourseRating> CourseRating { get; set; }

        public DbSet<Enrollment> Enrollments { get; set; }

        public DbSet<CourseComplete> CourseCompletes { get; set; }

        public DbSet<CourseExam> CourseExams { get; set; }

    }
}
