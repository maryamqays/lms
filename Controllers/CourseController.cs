using Lms.Models;
using LMS.Data;
using LMS.Models;
using LMS.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Security.Claims;

namespace LMS.Controllers
{
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;

        public CourseController(ApplicationDbContext dbContext, IWebHostEnvironment env, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _env = env;
            _userManager = userManager;

        }



        // GET: course
        public IActionResult Index()
        {
            var viewModel = new HomeViewModel
            {
                CourseCategories = _dbContext.CourseCategories.ToList(),
                Courses = _dbContext.Courses.ToList(),
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



        // GET: course/Create
        [Authorize(Roles = "Admin, Instructor")]

        public IActionResult Create()
        {
            ViewBag.CourseCategoryOptions = _dbContext.CourseCategories.ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Instructor")]

        public IActionResult Create(Course course, IFormFile courseFlyerFile, IFormFile coursePdfFile)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _env.WebRootPath;

                if (courseFlyerFile != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"CourseFlyer");
                    var extension = Path.GetExtension(courseFlyerFile.FileName);

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        courseFlyerFile.CopyTo(fileStreams);
                    }
                    course.CourseFlyer = @"\CourseFlyer\" + fileName + extension;
                }

                if (coursePdfFile != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"CoursePdf");
                    var extension = Path.GetExtension(coursePdfFile.FileName);

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        coursePdfFile.CopyTo(fileStreams);
                    }
                    course.CoursePdfFile = @"\CoursePdf\" + fileName + extension;
                }
                course.DateOfRecord = DateTime.Now;
                course.ApplicationUserId = _userManager.GetUserId(User);

                _dbContext.Courses.Add(course);
                _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Model state is not valid, retrieve the course categories and pass them to the view
            ViewBag.CourseCategoryOptions = _dbContext.CourseCategories.ToList();

            // Return the view with validation errors
            return View(course);
        }








        // GET: course/Edit/5
        [Authorize(Roles = "Admin, Instructor")]

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = _dbContext.Courses.Include(c => c.CourseCategory).FirstOrDefault(c => c.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            ViewBag.CourseCategoryOptions = _dbContext.CourseCategories.ToList();
            return View(course);
        }

        // POST: course/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Instructor")]

        public IActionResult Edit(int id, Course course, IFormFile courseFlyerFile, IFormFile coursePdfFile)
        {
            if (id != course.Id)
            {
                return NotFound();
            }
            string wwwRootPath = _env.WebRootPath;

            // Check if a new course flyer file is uploaded
            if (courseFlyerFile != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"CourseFlyer");
                var extension = Path.GetExtension(courseFlyerFile.FileName);

                // Delete the old course flyer file if it exists
                if (!string.IsNullOrEmpty(course.CourseFlyer))
                {
                    var oldImagePath = Path.Combine(wwwRootPath, course.CourseFlyer.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Save the new course flyer file
                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    courseFlyerFile.CopyTo(fileStreams);
                }

                course.CourseFlyer = @"\CourseFlyer\" + fileName + extension;
            }

            // Check if a new course PDF file is uploaded
            if (coursePdfFile != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"CoursePdf");
                var extension = Path.GetExtension(coursePdfFile.FileName);

                // Delete the old course PDF file if it exists
                if (!string.IsNullOrEmpty(course.CoursePdfFile))
                {
                    var oldPdfPath = Path.Combine(wwwRootPath, course.CoursePdfFile.TrimStart('\\'));
                    if (System.IO.File.Exists(oldPdfPath))
                    {
                        System.IO.File.Delete(oldPdfPath);
                    }
                }

                // Save the new course PDF file
                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    coursePdfFile.CopyTo(fileStreams);
                }
                course.CoursePdfFile = @"\CoursePdf\" + fileName + extension;
            }
            course.ApplicationUserId = _userManager.GetUserId(User);

            course.CourseLastUpdate = DateTime.Now;
            _dbContext.Courses.Update(course);
            _dbContext.SaveChanges();
            return RedirectToAction("Details", new { id = id });
        }


        // POST: course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Instructor")]

        public IActionResult DeleteConfirmed(int id)
        {
            var course = _dbContext.Courses.Include(c => c.CourseCategory).FirstOrDefault(c => c.Id == id);

            if (course != null)
            {
                _dbContext.Courses.Remove(course);
                _dbContext.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: course/Details/5
        // public IActionResult Details(int? id)
        // {



        //    CourseViewModel model = new CourseViewModel();
        //      model.Comments = _dbContext.CourseComments
        //        .Include(c => c.ApplicationUser)
        //        .Where(c => c.CourseId == id) // Filter comments by the CourseId
        //         .ToList();
        //       model.Course = _dbContext.Courses.Include(c => c.CourseCategory).Include(c => c.Enrollments).FirstOrDefault(c => c.Id == id);



        //      return View(model);
        //     }
        public IActionResult Details(int? id, int? contentId)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            LessonViewModel lessonViewModel = new LessonViewModel();
            lessonViewModel.Courses = _dbContext.Courses.ToList();
            lessonViewModel.CourseChapters = _dbContext.CourseChapters.Where(c => c.courseId == id).ToList();
            lessonViewModel.CourseChapterContents = _dbContext.CourseChapterContent.Where(c => c.courseChapter.courseId == id).ToList();
            lessonViewModel.Course = _dbContext.Courses.Include(c => c.CourseCategory).Include(c => c.ApplicationUser).FirstOrDefault(c => c.Id == id);
            // Filter CourseRatings for the specific course
            lessonViewModel.CourseRatings = _dbContext.CourseRating.Where(r => r.CourseId == id).ToList();
            lessonViewModel.CourseCompletes = _dbContext.CourseCompletes.Where(cc => cc.ApplicationUserId == userId).ToList();
            lessonViewModel.enrollments = _dbContext.Enrollments.Where(e => e.CourseId == id).ToList();
            lessonViewModel.courseComments = _dbContext.CourseComments
            .Where(e => e.CourseId == id && e.ApplicationUserId != null)
            .Include(c => c.ApplicationUser)
            .ToList();

            double averageScore = CommonUsed.CalculateAverageScore(lessonViewModel.CourseRatings);

            ViewBag.AverageScore = averageScore;

            return View(lessonViewModel);
        }

        public IActionResult Lesson(int? id, int? contentId)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            LessonViewModel lessonViewModel = new LessonViewModel();
            lessonViewModel.Courses = _dbContext.Courses.ToList();
            lessonViewModel.CourseChapters = _dbContext.CourseChapters.Where(c => c.courseId == id).ToList();
            lessonViewModel.CourseChapterContents = _dbContext.CourseChapterContent.ToList();
            lessonViewModel.Course = _dbContext.Courses.Include(c => c.CourseCategory).FirstOrDefault(c => c.Id == id);
            // Filter CourseRatings for the specific course
            lessonViewModel.CourseRatings = _dbContext.CourseRating.Where(r => r.CourseId == id).ToList();

            if (lessonViewModel.CourseRatings != null && lessonViewModel.CourseRatings.Count > 0)
            {
                double totalScore = 0;

                foreach (var rating in lessonViewModel.CourseRatings)
                {
                    totalScore += (rating.InstructorPerformance +
                                   rating.CourseContent +
                                   rating.CourseDuration +
                                   rating.CourseTime +
                                   rating.OutComeAchievment);
                }

                double averageScore = totalScore / (lessonViewModel.CourseRatings.Count * 5.0);

                ViewBag.AverageScore = averageScore;
            }
            else
            {
                ViewBag.AverageScore = "not rated";
            }

            return View(lessonViewModel);
        }






        [HttpPost]
        public IActionResult AddComment(CourseComment comment, int courseId)
        {
            var courseChapters = _dbContext.CourseChapters.ToList();
            // Set the DateTime of the comment
            comment.DateTime = DateTime.Now;

            // Get the currently logged-in user
            var user = _userManager.GetUserAsync(User).Result;

            if (user != null)
            {
                // Set the ApplicationUserId to the currently logged-in user's ID
                comment.ApplicationUserId = user.Id;
            }

            // Check if the course ID exists in the database
            var course = _dbContext.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null)
            {
                return NotFound("Course not found"); // Handle the case when the course ID does not exist
            }

            // Assign the CourseId of the comment
            comment.CourseId = courseId;

            // Save the comment to the database
            _dbContext.CourseComments.Add(comment);
            _dbContext.SaveChanges();

            // Access the comment's generated ID
            int commentId = comment.Id;

            // Redirect back to the Lesson action
            return RedirectToAction("Details", new { id = courseId });
        }


        [HttpPost]
        public IActionResult AddRating(CourseRating rating, int courseId)
        {

            var courseChapters = _dbContext.CourseChapters.ToList();
            // Set the DateTime of the comment
            rating.DateTime = DateTime.Now;

            // Get the currently logged-in user
            var user = _userManager.GetUserAsync(User).Result;

            if (user != null)
            {
                // Set the ApplicationUserId to the currently logged-in user's ID
                rating.ApplicationUserId = user.Id;
            }

            // Check if the course ID exists in the database
            var course = _dbContext.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null)
            {
                return NotFound("Course not found"); // Handle the case when the course ID does not exist
            }

            // Assign the CourseId of the comment
            rating.CourseId = courseId;

            // Save the comment to the database
            _dbContext.CourseRating.Add(rating);
            _dbContext.SaveChanges();



            // Redirect back to the Lesson action
            return RedirectToAction("details", new { id = courseId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Enroll(int courseId)
        {
            var user = _userManager.GetUserAsync(User).Result;
            var course = _dbContext.Courses.Include(c => c.Enrollments).FirstOrDefault(c => c.Id == courseId);

            if (user != null && course != null)
            {
                var isAlreadyEnrolled = course.Enrollments.Any(e => e.UserId == user.Id);

                if (!isAlreadyEnrolled)
                {
                    var enrollment = new Enrollment { CourseId = courseId, UserId = user.Id };
                    _dbContext.Enrollments.Add(enrollment);
                    _dbContext.SaveChanges();
                }

                return RedirectToAction("Details", "Course", new { id = courseId });
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> MarkCourseContentComplete(int contentId, bool isCompleted)
        {
            try
            {
                // Get the currently logged in user
                var currentUser = await _userManager.GetUserAsync(User);

                // Check if a CourseComplete object exists for the current user and the selected content
                var courseComplete = _dbContext.CourseCompletes.FirstOrDefault(cc => cc.ApplicationUserId == currentUser.Id && cc.CourseChapterContentId == contentId);

                if (courseComplete == null)
                {
                    // Create a new CourseComplete object if one does not exist
                    courseComplete = new CourseComplete
                    {
                        ApplicationUserId = currentUser.Id,
                        CourseChapterContentId = contentId,
                        IsCompleted = isCompleted
                    };

                    _dbContext.CourseCompletes.Add(courseComplete);
                }
                else
                {
                    // Update the existing CourseComplete object if one exists
                    courseComplete.IsCompleted = isCompleted;
                }

                await _dbContext.SaveChangesAsync();

                // Redirect to the appropriate page based on the completion
                if (courseComplete.CourseChapterContent != null)
                {
                    return RedirectToAction("Lesson", "CourseChapter", new { id = courseComplete.CourseChapterContent.CourseChapterId, contentId = courseComplete.CourseChapterContentId });
                }
                else
                {
                    // Handle the case where the CourseChapterContent property is null
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex); // Print the exception details in the console

                // Optionally, you can log the exception using a logging framework like Serilog or NLog

                // Handle the error and return an appropriate response
                return StatusCode(500); // Internal Server Error
            }
        }





    }




}

