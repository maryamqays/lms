using DinkToPdf;
using DinkToPdf.Contracts;
using Lms.Models;
using LMS.Data;
using LMS.Models;
using LMS.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using LMS.Extensions; // Make sure this is the correct namespace

namespace LMS.Controllers
{
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly GetUserNameAndImage _getUserNameAndImage;

        public CourseController(GetUserNameAndImage getUserNameAndImage, ApplicationDbContext dbContext, IWebHostEnvironment env, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _env = env;
            _userManager = userManager;
            _getUserNameAndImage = getUserNameAndImage;


        }


        public async Task<IActionResult> Exam(int courseId)
        {
            var questions = await _dbContext.CourseExams  
                .Where(q => q.CourseId == courseId)
                .OrderBy(r => Guid.NewGuid())  // This randomizes the questions.
                .Take(5)
                .ToListAsync();

            return View(questions); // Return the view with the questions as its model.
        }

        [HttpPost]
        public async Task<IActionResult> SubmitExam(List<CourseExam> submittedAnswers, int courseId)
        {
            if (submittedAnswers == null || !submittedAnswers.Any())
                return BadRequest("No answers provided.");

            int correctAnswersCount = 0;

            foreach (var answer in submittedAnswers)
            {
                var originalQuestion = await _dbContext.CourseExams.FindAsync(answer.Id);

                if (originalQuestion != null && originalQuestion.CorrectAnswer == answer.CorrectAnswer)
                {
                    correctAnswersCount++;
                }
            }

            if (correctAnswersCount >= 4)
            {
                return RedirectToAction("Certificate", new { Id = courseId });  // Redirect to Certificate action if passed.
            }
            else
            {
                return Content("Failed");
            }
        }





        // GET: course
        public IActionResult Index(int pageIndex = 1, int pageSize = 8, string searchTerm = "")
        {
            // Ensure pageIndex is not out of range
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }

            int numberOfCoursesToSkip = (pageIndex - 1) * pageSize;

            var query = _dbContext.Courses.AsQueryable();

            // Perform the search if searchTerm is not empty




            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                query = query.Where(course =>
                    course.EnTitle.ToLower().Contains(searchTerm) ||
                    course.ArTitle.ToLower().Contains(searchTerm));
            }




            var viewModel = new HomeViewModel
            {
                CourseCategories = _dbContext.CourseCategories.ToList(),
                Courses = query
                    .OrderBy(c => c.Id)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList(),
                CourseCountPerCategory = _dbContext.CourseCategories.
                ToDictionary(category => category.Id, category => _dbContext.Courses.
                Count(course => course.CourseCategoryId == category.Id)),
                Coursesingle = _dbContext.Courses.Include(c => c.InstructorUsers).FirstOrDefault(),
                CourseRatings = _dbContext.CourseRating.ToList(),
                CourseChapterContents = _dbContext.CourseChapterContent
                .Include(c => c.courseChapter)
                .ThenInclude(cc => cc.Course)
                .Skip(numberOfCoursesToSkip)
                .Take(pageSize)
                .ToList()
            };

            double averageScore = CommonUsed.CalculateAverageScore(viewModel.CourseRatings);
            ViewBag.AverageScore = @Math.Round(averageScore, 1);


            // Total number of pages
            ViewBag.TotalPages = (int)Math.Ceiling((double)_dbContext.Courses.Count() / pageSize);
            ViewBag.CurrentPage = pageIndex;

            return View(viewModel);
        }




        // GET: course/Create
        [Authorize(Roles = "Admins, Instructors")]
        public async Task<IActionResult> Create()
        {
            var userId = _userManager.GetUserId(User);  // get the logged-in user's ID

            // Assuming you have DbSet for InstructorUser in your context
            var instructorUserExists = await _dbContext.InstructorUsers.AnyAsync(i => i.Id == userId);

            // If the user is in the "Instructor" role and doesn't have an InstructorUser profile
            if (User.IsInRole("Instructors") || User.IsInRole("Admins") && !instructorUserExists)
            {
                // Redirect them to the page to create their InstructorUser profile
                return RedirectToAction(nameof(Create), "InstructorUser");
            }

            ViewBag.CourseCategoryOptions = _dbContext.CourseCategories.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admins, Instructors")]

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
                course.InstructorUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                _dbContext.Courses.Add(course);
                _dbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            // Model state is not valid, retrieve the course categories and pass them to the view
            ViewBag.CourseCategoryOptions = _dbContext.CourseCategories.ToList();

            // Return the view with validation errors
            return View(course);
        }

        // GET: course/Edit/5
        [Authorize(Roles = "Admins, Instructors")]
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
        [Authorize(Roles = "Admins, Instructors")]

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
            course.InstructorUserId = _userManager.GetUserId(User);

            course.CourseLastUpdate = DateTime.Now;
            _dbContext.Courses.Update(course);
            _dbContext.SaveChanges();
            return RedirectToAction("Details", new { id = id });
        }


        // POST: course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admins, Instructors")]

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


        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = _dbContext.Courses.SingleOrDefault(a => a.Id == id);
            LessonViewModel lessonViewModel = new LessonViewModel();
            lessonViewModel.Courses = _dbContext.Courses.Where(c => c.InstructorUserId == course.InstructorUserId).ToList();
            lessonViewModel.CourseChapters = _dbContext.CourseChapters.Where(c => c.courseId == id).ToList();
            lessonViewModel.CourseChapterContents = _dbContext.CourseChapterContent.Where(c => c.courseChapter.courseId == id).ToList();
            lessonViewModel.Course = _dbContext.Courses.Include(c => c.CourseCategory).Include(c => c.InstructorUsers).FirstOrDefault(c => c.Id == id);
            // Filter CourseRatings for the specific course
            lessonViewModel.CourseRatings = _dbContext.CourseRating.Where(r => r.CourseId == id).ToList();
            lessonViewModel.CourseCompletes = _dbContext.CourseCompletes.Where(cc => cc.ApplicationUserId == userId).ToList();
            lessonViewModel.enrollments = _dbContext.Enrollments.Where(e => e.CourseId == id).ToList();
            lessonViewModel.Certificate = _dbContext.certificateUsers
                .SingleOrDefault(c => c.CourseId == id && c.ApplicationUserId == userId);
            lessonViewModel.courseComments = _dbContext.CourseComments
           .Where(e => e.CourseId == id && e.ApplicationUserId != null)
           //.Include(c => c.ApplicationUser)
           .ToList();

            // total students who reviewed with a specific teacher 

            lessonViewModel.totalstudentcomments = _dbContext.CourseComments
           .Count(c => c.Course.InstructorUserId == course.InstructorUserId);
            // total students enrolled with a specific teacher 
            lessonViewModel.totalstudent = _dbContext.Enrollments
                .Where(e => e.Course.InstructorUserId == course.InstructorUserId)
                .ToList();

            lessonViewModel.Instructorrating = _dbContext.CourseRating
             .Where(c => c.Course.InstructorUserId == course.InstructorUserId)
             .ToList();


            // Check if all CourseChapterContents have been completed
            lessonViewModel.AllContentsCompleted = lessonViewModel.CourseChapterContents
                .All(content => lessonViewModel.CourseCompletes
                    .Any(cc => cc.CourseChapterContentId == content.Id && cc.IsCompleted));

            double Total = 0;
            foreach (var rating in lessonViewModel.Instructorrating)
            {
                Total += ((double)rating.InstructorPerformance +
                           rating.CourseContent +
                           rating.CourseDuration +
                           rating.CourseTime +
                           rating.OutComeAchievment) / 5;
            }
            if (lessonViewModel.Instructorrating.Count > 0)
            {
                lessonViewModel.Instructorratingsum = Total / lessonViewModel.Instructorrating.Count;
                Console.WriteLine(lessonViewModel.Instructorrating.Count);
            }

            double averageScore = CommonUsed.CalculateAverageScore(lessonViewModel.CourseRatings);

            ViewBag.AverageScore = @Math.Round(averageScore, 1);

            Console.WriteLine(Total + "count" + lessonViewModel.Instructorrating.Count);
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
            lessonViewModel.CourseChapterContents = _dbContext.CourseChapterContent.Include(c => c.courseComplete).ToList();
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

                ViewBag.AverageScore = @Math.Round(averageScore, 1);
            }
            else
            {
                ViewBag.AverageScore = "not rated";
            }

            return View(lessonViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> AddComment(CourseComment comment, int courseId)
        {
            // Set the DateTime of the comment
            comment.DateTime = DateTime.Now;

            // Get the currently logged-in user's ClaimsPrincipal
            var userClaimsPrincipal = User; // Assuming User is of type ClaimsPrincipal

            if (userClaimsPrincipal != null)
            {
                // Get the user's ID from the ClaimsPrincipal
                var userId = userClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!string.IsNullOrEmpty(userId))
                {
                    // Set the ApplicationUserId to the currently logged-in user's ID
                    comment.ApplicationUserId = userId;

                    // Get user details using UserDetails action
                    var userDetails = await _getUserNameAndImage.GetUserDetailsAndImageAsync(userClaimsPrincipal);

                    if (userDetails != null)
                    {
                        // Set comment properties from user details
                        comment.UserName = userDetails.UserName;
                        comment.ProfilePicture = userDetails.ProfilePicture;
                    }
                }
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

            // Get the currently logged-in user
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if the course ID exists in the database
            var course = _dbContext.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null)
            {
                return NotFound("Course not found"); // Handle the case when the course ID does not exist
            }
            // Assign the CourseId of the comment
            rating.ApplicationUserId = user;
            rating.CourseId = courseId;
            rating.DateTime = DateTime.Now;
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = _dbContext.Courses.Include(c => c.Enrollments).FirstOrDefault(c => c.Id == courseId);

            if (userId != null && course != null)
            {
                var isAlreadyEnrolled = course.Enrollments.Any(e => e.ApplicationUserId == userId);

                if (!isAlreadyEnrolled)
                {
                    var enrollment = new Enrollment
                    {
                        CourseId = courseId,
                        ApplicationUserId = userId,
                        DateOfOfEnrollment = DateTime.Now
                    };
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
                var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Console.WriteLine(currentUser);
                // Check if a CourseComplete object exists for the current user and the selected content
                var courseComplete = _dbContext.CourseCompletes.FirstOrDefault(cc => cc.ApplicationUserId == currentUser && cc.CourseChapterContentId == contentId);

                if (courseComplete == null)
                {
                    // Create a new CourseComplete object if one does not exist
                    courseComplete = new CourseComplete
                    {
                        ApplicationUserId = currentUser,
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


        [Authorize(Roles = "Admins, Instructors")]
        public IActionResult CourseListInstructor()
        {// Get the current user's ID
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve the courses associated with the user
            List<Course> courses = _dbContext.Courses
                .Where(c => c.InstructorUserId == userId)
                .ToList();

            return View(courses);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadCertificate(int id)
        {
            // Get the ID of the currently logged in user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch the certificate that belongs to the current user and has the specified course ID
            var certificate = await _dbContext.certificateUsers
                .Include(c => c.Course)
               // .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.ApplicationUserId == userId && c.CourseId == id);

            if (certificate == null)
            {
                return NotFound();
            }

            return View(certificate);
        }

        [HttpGet]
        public IActionResult Certificate(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var course = _dbContext.Courses.FirstOrDefault(c => c.Id == Id);

            if (course == null)
            {
                return NotFound();
            }

            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (user == null)
            {
                return NotFound();
            }
            var courseHours = _dbContext.CourseChapterContent
                            .Where(content => content.courseChapter.courseId == course.Id)
                            .Sum(content => content.Duration) / 60;

            if (courseHours < 1)
            {
                courseHours = 1;
            }


            var certificate = new Certificate
            {
                ApplicationUserId = user,
                CourseId = course.Id,
                DateOfCertification = DateTime.Now,
                CourseHours = courseHours,
                Course = course,
                Passedexam=true
                // User = user
            };


            Console.WriteLine(certificate);

            return View(certificate);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Certificate(Certificate model, int? Id)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var course = _dbContext.Courses.FirstOrDefault(c => c.Id == Id);

            if (course == null)
            {
                return NotFound();
            }

            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (user == null)
            {
                return NotFound();
            }

            var certificate = new Certificate
            {
                CertifiedName = model.CertifiedName,
                CourseId = course.Id,
                ApplicationUserId = user,
                DateOfCertification = DateTime.Now,
                Course = course,
                Passedexam = true

                //  User = user
            };

            _dbContext.certificateUsers.Add(certificate);
            await _dbContext.SaveChangesAsync();

            // instead of redirecting, return the same view with the model
            // this will cause the page to reload with the posted data
            return RedirectToAction("DownloadCertificate", new { id = course.Id });
        }


        [HttpGet]
        public async Task<IActionResult> LoadMoreComments(int courseId, int pageNumber)
        {
            int pageSize = 10; // Always load 10 comments

            var comments = await _dbContext.CourseComments
                .Where(e => e.CourseId == courseId && e.ApplicationUserId != null)
                .OrderByDescending(c => c.DateTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new
                {
                    CommentText = c.Comment,
                    DateTime = c.DateTime
                })
                .ToListAsync();

            return Json(comments);
        }












    }




}

