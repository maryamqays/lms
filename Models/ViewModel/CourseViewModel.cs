using Lms.Models;

namespace LMS.Models.ViewModel
{
    public class CourseViewModel
    {
        public Course Course { get; set; }
        public List<CourseComment> Comments { get; set; }   

    }
}
