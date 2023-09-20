using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace LMS.Models
{
    public class LoginViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public string userId { get; set; }

        public string ImageUrl { get; set; }


    }

}
