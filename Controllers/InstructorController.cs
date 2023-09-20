using Humanizer;
using Lms.Models;
using LMS.Data;
using LMS.Models;
using LMS.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Buffers.Text;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace LMS.Controllers
{
    public class InstructorController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public InstructorController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(string id, string searchTerm, int page = 1, int pageSize = 8)
        {
            if (page < 1)
            {
                page = 1; // Set the page number to 1 if it is less than 1
            }

            var query = _dbContext.Courses
                .Include(c => c.InstructorUsers)
                .Where(c => c.InstructorUsers.Id == id); // Filter courses by instructor ID

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c => c.EnTitle.Contains(searchTerm) || c.ArTitle.Contains(searchTerm));
            }

            var viewModel = new HomeViewModel
            {
                InstructorUser = _dbContext.InstructorUsers.FirstOrDefault(a => a.Id == id),
                CourseCategories = _dbContext.CourseCategories.ToList(),
                Courses = query
                    .OrderBy(c => c.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList(),
                CourseCountPerCategory = _dbContext.CourseCategories.ToDictionary(
                    category => category.Id,
                    category => _dbContext.Courses.Count(course => course.CourseCategoryId == category.Id)),
                Coursesingle = _dbContext.Courses.Include(c => c.InstructorUsers).FirstOrDefault(),
                CourseRatings = _dbContext.CourseRating.ToList(),
                CourseChapterContents = _dbContext.CourseChapterContent
                    .Include(c => c.courseChapter)
                    .ThenInclude(cc => cc.Course)
                    .ToList(),
                        CurrentPage = page,
                            SearchTerm = searchTerm


            };

            viewModel.TotalCourseCount = query.Count(); // Retrieve the total count of courses for the instructor
            int totalCourses = query.Count();
            viewModel.TotalPagesInsTructor = (int)Math.Ceiling(totalCourses / (double)pageSize);

            double averageScore = CommonUsed.CalculateAverageScore(viewModel.CourseRatings);
            ViewBag.AverageScore = @Math.Round(averageScore, 1);

            return View(viewModel);
        }

        public IActionResult CourseListInstructor()
        {
            return View();
        }


        public IActionResult CourseListAdmin()
        {
            return View();
        }
    }
}
