using System.ComponentModel.DataAnnotations;

namespace LMS.Models.ViewModel
{
    public class CreateUserWithRoleViewModel
    {
        public string RoleName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
