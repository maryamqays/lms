using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Lms.Models
{
    public class CourseComment
    {
        [Key]
        public int Id { get; set; }

        [StringLength(2000, ErrorMessage = "Comment should not be more than 2000 characters")]
        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "Date Time")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d}")] // short date format
        public DateTime DateTime { get; set; }

        [Display(Name = "Hidden Comment")]
        public bool Ishidden { get; set; }

        [Display(Name = "Special")]
        public bool IsFeatured { get; set; }

        [Display(Name = "Deleted")]
        public bool Isdeletd { get; set; }

        [Display(Name = "Course")]
        public int CourseId { get; set; }
        [Display(Name = "Course")]
        public Course Course { get; set; }

        [Display(Name = "User")]
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public IdentityUser ApplicationUser { get; set; }
    }
}
