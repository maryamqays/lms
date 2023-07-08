using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LMS.Models
{
    public class ApplicationUser : IdentityUser
    {

        

        [Display(Name = "Is Instructor?")]
        public bool IsInstructor { get; set; }

        [Display(Name = "Profile Picture")]
        public string ImageUrl { get; set; }


        [Display(Name = "Instructor OverView")]
        public string Description { get; set; }


    }
}
