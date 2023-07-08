namespace Lms.Models
{
    public class Common
    {

        public enum CertificateType
        {
            Paid = 0,
            Free = 1,
            NoCertificate = 2
        }

        public enum Contenttype
        {
            file = 0,
            youtubevideo=1,
            recordedvideo=2 
        }

        public enum Level 
        {
            Easy = 0,
            Mid = 1,
            Hard = 2
        }
        public enum Language
        {
            Arabic = 0,
            English = 1,
            other = 2
        }

    }
}
