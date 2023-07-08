using System.ComponentModel.DataAnnotations;

namespace Lms.Models
{
    public class CourseChapter
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage=" required name ")]
        [StringLength(100, ErrorMessage=" name should not be more than 100 charcter")]
        [Display(Name="course chapter name ")]
        public string Name { get; set; }

        [Required(ErrorMessage = "required Indx")]
        [Display(Name = "Course chapter position (Indx) ")]
        public int Indx { get; set; }

        [Required(ErrorMessage = " required Courses ")]
        [Display(Name = "Courses ")]
        public  int courseId { get; set; }  
        public Course Course { get; set; }

        [Required]
        [Display(Name = "IsDeleted ")]
        public bool IsDeleted { get; set; }

        [Required]
        [Display(Name = "Is hidden ")]
        public bool Ishidden { get; set; }  
                

    }
}
