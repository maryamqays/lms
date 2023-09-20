using Azure;
using Lms.Models;

namespace LMS.Models
{
    public static class CommonUsed
    {
        public static double CalculateAverageScore(List<CourseRating> courseRatings)
        {
            if (courseRatings != null && courseRatings.Count > 0)
            {
                double totalScore = 0;

                foreach (var rating in courseRatings)
                {
                    totalScore += (rating.InstructorPerformance +
                                   rating.CourseContent +
                                   rating.CourseDuration +
                                   rating.CourseTime +
                                   rating.OutComeAchievment);
                }

                return totalScore / (courseRatings.Count * 5.0);
            }
            else
            {
                return 0; // or any default value you prefer
            }
        }
    }

   

}
