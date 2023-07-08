using LMS.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;
using static Lms.Models.Common;

namespace Lms.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Course name should not be more than 100 character")]
        public string EnTitle { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Course name  should not be more than 100 character")]
        [Display(Name = "Course Name Ar")]
        public string ArTitle { get; set; }

        [Required]
        [StringLength(2500, ErrorMessage = "Course Overview  should not be more than 2500 character")]
        [Display(Name = "Overview")]
        public string Overview { get; set; }

        [StringLength(400, ErrorMessage = " Course Tags  should not be more than 400 character")]
        [Display(Name = "Tags")]
        public string Tags { get; set; }

        [Display(Name = "DateOfRecord")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d}")]// short date formant
        public DateTime DateOfRecord { get; set; }

        [Display(Name = "Course Category")]
        public int CourseCategoryId { get; set; }
        public CourseCategory CourseCategory { get; set; }


        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Course Starting Date")]
        public DateTime CourseStartingDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Course Ending Date")]
        public DateTime CourseEndingDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Course Last Update")]
        public DateTime CourseLastUpdate { get; set; }


        [Display(Name = "Course Introduction Video")]
        [StringLength(150, ErrorMessage = "CourseIntroductionVideo should not be more than 150 character")]
        public string CourseIntroductionVideo { get; set; }

        [Display(Name = "Course Flyer")]
        public string CourseFlyer { get; set; }

        [Display(Name = "Course PDF File")]
        public string CoursePdfFile { get; set; }

        [Required]
        [Display(Name = "Is Course Paid?")]
        public bool IsPaid { get; set; }

        [Display(Name = "Certificate Type")]
        public CertificateType CertificateType { get; set; }

        [Display(Name = "Certificate Cost")]
        public decimal CertificateCost { get; set; }

        [Required(ErrorMessage = "This Filed is Required")]
        [Display(Name = "Is Admin Approved?")]
        public bool IsAdminApproved { get; set; }

        [Required(ErrorMessage = "This Filed is Required")]
        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "This Filed is Required")]
        [Display(Name = "Is Featured?")]
        public bool IsFeatured { get; set; }

        [Required(ErrorMessage = "This Filed is Required")]
        [Display(Name = "Course Prerequisite")]
        [StringLength(2000, ErrorMessage = "Prerequisite should not be more than 2000 character")]
        public string Prerequisite { get; set; }

        [Required(ErrorMessage = "This Filed is Required")]
        [Display(Name = "Course Level")]
        public Level Level { get; set; }

        [Display(Name = "Course Outcomes")]
        [StringLength(2000, ErrorMessage = "Outcomes should not be more than 2000 character")]
        public string Outcomes { get; set; }

        [Display(Name = "Course Important Dates")]
        [StringLength(2000, ErrorMessage = "ImportantDates should not be more than 2000 character")]
        public string ImportantDates { get; set; }

        [StringLength(2000, ErrorMessage = "TargetAudience should not be more than 2000 character")]
        [Display(Name = "Course Target Audience")]
        public string TargetAudience { get; set; }

        [Display(Name = "Course Language")]
        public Language Language { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Course Fee")]
        public decimal Fee { get; set; }

        [Display(Name = "Is Pre-recorded?")]
        public bool IsPreRecorded { get; set; }

        [Display(Name = "Is Deleted?")]
        public bool IsDeleted { get; set; }

        [Display(Name = "Is Hidden?")]
        public bool IsHidden { get; set; }

        [Display(Name = "Is Private?")]
        public bool IsPrivate { get; set; }

        [Display(Name = "Course Passing Mark")]
        public int PassingMark { get; set; }

        [Display(Name = "Course Livestream")]
        public string Livestream { get; set; }

        // add Enrollment 
        public ICollection<Enrollment> Enrollments { get; set; }


        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public IdentityUser ApplicationUser { get; set; }

    }

}

