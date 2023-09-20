using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    public class InstructorUser
    {
        [Key]

        public string Id { get; set; }

        [Display(Name = "Instructor Name")]
        [StringLength(400, ErrorMessage = "Instructor Name Should be less tha 400")]

        public string UserName { get; set; }


        [Display(Name = "Profile Picture")]
        [StringLength(400, ErrorMessage = "Image Length Should be less tha 400")]

        public string ImageUrl { get; set; }


        [Display(Name = "Instructor OverView")]
        [StringLength(400, ErrorMessage = "Description name should not be more than 400 character")]

        public string Description { get; set; }


        [Display(Name = "Instructor JobTitle")]
        [StringLength(100, ErrorMessage = "JobTitle name should not be more than 100 character")]

        public string JobTitle { get; set; }



         

    }
}
