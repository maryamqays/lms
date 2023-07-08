using Lms.Models;
using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LMS.Controllers
{
    public class CourseChapterController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CourseChapterController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: CourseChapters
        [Authorize(Roles = "Admin, Instructor")]

        public IActionResult Index()
        {
            var chapters = _dbContext.CourseChapters.Include(c => c.Course).ToList();
            return View(chapters);
        }

        // GET: CourseChapters/Create
        [Authorize(Roles = "Admin, Instructor")]

        public IActionResult Create(int courseId)
        {
            ViewBag.Courses = _dbContext.Courses.ToList();
            var model = new CourseChapter
            {
                courseId = courseId
            };
            return View(model);
        }

        // POST: CourseChapters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Instructor")]

        public IActionResult Create(CourseChapter Chapter)
        {
            _dbContext.CourseChapters.Add(Chapter);
            _dbContext.SaveChanges();
            return RedirectToAction("Details", "Course", new { id = Chapter.courseId });
        }

        // GET: CourseChapters/Edit/5
        [Authorize(Roles = "Admin, Instructor")]

        public IActionResult Edit(int? id)
        {
            ViewBag.Courses = _dbContext.Courses.ToList();

            if (id == null)
            {
                return NotFound();
            }

            var courseChapter = _dbContext.CourseChapters.FirstOrDefault(cc => cc.Id == id); ;
            if (courseChapter == null)
            {
                return NotFound();
            }

            return View(courseChapter);
        }

        // POST: CourseChapters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Instructor")]

        public IActionResult Edit(int id, CourseChapter Chapter)
        {
            if (id != Chapter.Id)
            {
                return NotFound();
            }
            _dbContext.CourseChapters.Update(Chapter);
            _dbContext.SaveChanges();
            return RedirectToAction("Details", "Course", new { id = Chapter.courseId });
        }


        // POST: CourseChapters/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Instructor")]

        public IActionResult Delete(int id)
        {
            var chapter = _dbContext.CourseChapters.Find(id);
            if (chapter == null)
            {
                return NotFound();
            }

            var courseId = chapter.courseId;
            _dbContext.CourseChapters.Remove(chapter);
            _dbContext.SaveChanges();

            // Redirect to the Details action of the Course controller with the courseId as the parameter
            return RedirectToAction("Details", "Course", new { id = courseId });
        }


    }
}
