using Lms.Models;
using LMS.Areas.Identity.Pages.Account;

namespace LMS.Models.ViewModel
{
    public class HomeViewModel
    {
        public List<CourseCategory> CourseCategories { get; set; }

        public List<Course> Courses { get; set; }

        public Course Coursesingle { get; set; }

        public RegisterModel Register { get; set; }

        public Dictionary<int, int> CourseCountPerCategory { get; set; } // New Property

        public CourseRating CourseRating { get; set; }
        public List<CourseRating > CourseRatings { get; set; }

        public List<CourseChapterContent> CourseChapterContents { get; set; }

        public List<Enrollment> enrollments { get; set; }
    }
}
