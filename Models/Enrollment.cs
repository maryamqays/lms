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

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public string UserId { get; set; }
        public IdentityUser User { get; set; }


        // add date now 
    }
}
