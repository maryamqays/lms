using Lms.Models;
using System.ComponentModel.DataAnnotations;

namespace LMS.Models
{
    public class Certificate
    {

        public int Id { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }

        public string ApplicationUserId { get; set; }
       // public string applicationuser applicationuser { get; set; }


         [Display(Name = "DateOfRecord")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d}")]// short date formant
        public DateTime DateOfCertification { get; set; }

        public bool IsDownloadable { get; set; }

        public int DownloadTimes { get; set; }

        public string CertifiedName { get; set; }

        public int CourseHours { get; set; }



    }
}
