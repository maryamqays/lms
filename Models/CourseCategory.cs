using System.ComponentModel.DataAnnotations;

namespace LMS.Models
{
    public class CourseCategory
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The Arabic name is required.")]
        [Display(Name = "Name (Arabic)")]
        public string ArName { get; set; }

        [Required(ErrorMessage = "The English name is required.")]
        [Display(Name = "Name (English)")]
        public string EnName { get; set; }

        [Display(Name = "Category description")]
        [StringLength(2500, ErrorMessage = "Course Description  should not be more than 2500 character")]
        public string Description { get; set; }

        [Display(Name = "Category image")]
        public string Image { get; set; }

        public string CombinedName
        {
            get
            {
                return $"{EnName} / {ArName}";
            }
        }

    }
}
