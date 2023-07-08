using Lms.Models;
using LMS.Areas.Identity.Pages.Account;
using LMS.Data;
using LMS.Models;
using LMS.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;

namespace LMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            var viewModel = new HomeViewModel
            {
                enrollments = _dbContext.Enrollments.ToList(),
                CourseCategories = _dbContext.CourseCategories.ToList(),
                Courses = _dbContext.Courses.Include(c => c.CourseCategory).ToList(),
                CourseCountPerCategory = _dbContext.CourseCategories.ToDictionary(
                    category => category.Id,
                    category => _dbContext.Courses.Count(course => course.CourseCategoryId == category.Id)),
                Coursesingle = _dbContext.Courses.Include(c => c.ApplicationUser).FirstOrDefault(),
                CourseRatings = _dbContext.CourseRating.ToList(),
                CourseChapterContents = _dbContext.CourseChapterContent
                .Include(c => c.courseChapter)
                .ThenInclude(cc => cc.Course)
                .ToList()
            };

            double averageScore = CommonUsed.CalculateAverageScore(viewModel.CourseRatings);

            ViewBag.AverageScore = averageScore;
            return View(viewModel);
        }
        public IActionResult Notfound(int? statusCode)
        {
            if (statusCode.HasValue)
            {
                if (statusCode.Value == 404)
                {
                    return View("NotFound");
                }
                return View();
            }

            return View("Error");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contact()
        {

            return View();
        }

        public IActionResult AboutUs()
        {

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [Authorize(Roles = "Admin")]

        public IActionResult AdminPanel()
        {
            return View();
        }

        public JsonResult Data()
        {

            var viewModel = new HomeViewModel
            {
                //    enrollments = _dbContext.Enrollments.ToList(),
                //     CourseCategories = _dbContext.CourseCategories.ToList(),
                Courses = _dbContext.Courses.Include(c => c.CourseCategory).ToList(),
                //  CourseCountPerCategory = _dbContext.CourseCategories.ToDictionary(
                //     category => category.Id,
                //      category => _dbContext.Courses.Count(course => course.CourseCategoryId == category.Id)),
                //     Coursesingle = _dbContext.Courses.FirstOrDefault(),
                //    CourseRatings = _dbContext.CourseRating.ToList(),
                //    CourseChapterContents = _dbContext.CourseChapterContent
                //     .Include(c => c.courseChapter)
                //     .ThenInclude(cc => cc.Course)
                //     .ToList()
            };
            return Json(viewModel);
        }

    }
}