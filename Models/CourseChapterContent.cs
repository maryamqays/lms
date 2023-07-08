using LMS.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mime;
using System.Xml.Linq;
using static Lms.Models.Common;

namespace Lms.Models
{
    public class CourseChapterContent
    {
        [Key]
        public int Id { get; set; }


        [Required(ErrorMessage = "required Title")]
        [StringLength(100, ErrorMessage = " Title should not be more than 100 charcter")]
        [Display(Name = " Course Chapter Content Title ")]
        public string Title { get; set; }


        [Required(ErrorMessage = " required Description")]
        [StringLength(500, ErrorMessage = "Description should not be more than 100 charcter")]
        [Display(Name = "Course Chapter Content Description ")]
        public string Description { get; set; }



        [Required(ErrorMessage = "required Indx ")]
        [Display(Name = "Course chapter Content position (Indx) ")]
        public int Indx { get; set; }

        [Required(ErrorMessage = "required Course Chapter ")]
        [Display(Name = "Courses Chapter ")]
        public int CourseChapterId { get; set; }
        public CourseChapter courseChapter { get; set; }

        [Required(ErrorMessage = "required Content Type ")]
        [Display(Name = "Courses Content Type ")]
        public Contenttype ContentType { get; set; }

        // in case content type was Youtube 

        [Display(Name = "Youtube Video")]
        public string Youtube { get; set; }

        // in case content type was file 
        [Display(Name = "File path")]
        public string FilePath { get; set; }

        [Display(Name = "Duration in min")]
        public int Duration { get; set; }

        // pass model if its complete

        // add course id 


    }
}