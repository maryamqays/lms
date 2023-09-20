using Lms.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    public class Enrollment
    {
        [Key]
        public int Id { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d}")]// short date formant
        public DateTime DateOfOfEnrollment{ get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public string ApplicationUserId { get; set; }


        // add date now 
    }
}
