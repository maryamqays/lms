using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Lms.Models;

namespace LMS.Models
{
    public class CourseComplete
    {
        public int Id { get; set; }

        [Display(Name = "User")]

        public string ApplicationUserId { get; set; }



        public int CourseChapterContentId { get; set; }
        public CourseChapterContent CourseChapterContent { get; set; }

        public bool IsCompleted { get; set; } 
    }
}
