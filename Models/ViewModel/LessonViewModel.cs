using Lms.Models;
using System.Collections.Generic;

namespace LMS.Models.ViewModel
{
    public class LessonViewModel
    {
        public Course Course { get; set; }
        public List<Course> Courses { get; set; }

        public CourseComplete CourseComplete { get; set; }
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

        public Enrollment enrollment { get; set; }
        public int[] RatingPercentages { get; set; }

        public int CourseCount { get; set; }

        public List<CourseRating> Instructorrating { get; set; }

        public double Instructorratingsum { get; set; }

        public List<Enrollment> totalstudent { get; set; }

        public int totalstudentcomments { get; set; }
         public CourseComplete courseComplete { get; set; }
        public List<Certificate> certificates { get; set; }
        public InstructorUser InstructorUsers { get; set; }

        public Certificate Certificate { get; set; }
        public CourseComment CourseComment { get; set; }
        public bool AllContentsCompleted { get; set; }
    }
}