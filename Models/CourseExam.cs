using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Lms.Models;

namespace LMS.Models
{
    public class CourseExam
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Question { get; set; }

        [Required]
        [StringLength(200)]
        public string Answer1 { get; set; }

        [Required]
        [StringLength(200)]
        public string Answer2 { get; set; }

        [Required]
        [StringLength(200)]
        public string Answer3 { get; set; }

        [Required]
        [StringLength(200)]
        public string Answer4 { get; set; }

        [Range(1, 4)]
        public int CorrectAnswer { get; set; }

        public Lang Langs { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

    }
}

public enum Lang
{
    Arabic = 1,
    English = 2
}
