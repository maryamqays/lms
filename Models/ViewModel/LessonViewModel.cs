using Lms.Models;
using System.Collections.Generic;

namespace LMS.Models.ViewModel
{
    public class LessonViewModel
    {
        public Course Course { get; set; }
        public List<Course> Courses { get; set; }

        public List<CourseChapter> CourseChapters { get; set; }
        public List<CourseChapterContent> CourseChapterContents { get; set; }
        public List<CourseComment> Comments { get; set; }
        public CourseComment Comment { get; set; }
        public List<CourseRating> CourseRatings { get; set; }
        public CourseChapterContent CourseChapterContent { get; set; }
        public CourseRating CourseRating { get; set; }
        public List<CourseComment> courseComments { get; set; }
        public List<CourseComplete> CourseCompletes { get; set; }

        public List<Enrollment> enrollments { get; set; }
        public int[] RatingPercentages { get; set; }

    }
}
