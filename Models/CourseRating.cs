using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Lms.Models
{
    public class CourseRating
    {
        public Guid Id { get; set; }

        [Display(Name = "User")]
        public string ApplicationUserId { get; set; }

        [Display(Name = "Course")]
        public int CourseId { get; set; }
        public Course Course { get; set; }

        [Display(Name = "Rating Date Time")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d}")]// short date formant
        public DateTime DateTime { get; set; }

        [Display(Name = "InstructorPerformance")]
        public int InstructorPerformance { get; set; }

        [Display(Name = "CourseContent")]
        public int CourseContent { get; set; }

        [Display(Name = "CourseDuration")]
        public int CourseDuration { get; set; }
        // what is the difference between coruse duearion and course time
        [Display(Name = "CourseTime")]
        public int CourseTime { get; set; }

        [Display(Name = "OutComeAchievment")]
        public int OutComeAchievment { get; set; }


        [NotMapped] // This property is not mapped to the database
        [Display(Name = "Instructor Performance %")]
        public int InstructorPerformancePercentage
        {
            get { return (InstructorPerformance * 100) / 5; } // Assuming the rating is out of 5
        }

        [NotMapped] // This property is not mapped to the database
        [Display(Name = "Course Content %")]
        public int CourseContentPercentage
        {
            get { return (CourseContent * 100) / 5; } // Assuming the rating is out of 5
        }

        [NotMapped] // This property is not mapped to the database
        [Display(Name = "Course Duration %")]
        public int CourseDurationPercentage
        {
            get { return (CourseDuration * 100) / 5; } // Assuming the rating is out of 5
        }

        [NotMapped] // This property is not mapped to the database
        [Display(Name = "Course Time %")]
        public int CourseTimePercentage
        {
            get { return (CourseTime * 100) / 5; } // Assuming the rating is out of 5
        }

        [NotMapped] // This property is not mapped to the database
        [Display(Name = "Outcome Achievement %")]
        public int OutcomeAchievementPercentage
        {
            get { return (OutComeAchievment * 100) / 5; } // Assuming the rating is out of 5
        }

    }
}
