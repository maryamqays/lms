using Lms.Models;
using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LMS.Controllers
{
    public class CourseChapterContentController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _env;

        public CourseChapterContentController(ApplicationDbContext dbContext, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _env = env;
        }
        [Authorize(Roles = "Admin, Instructor")]

        // GET: CourseChapter
        public IActionResult Index()
        {
            var Content= _dbContext.CourseChapterContent.Include(cc => cc.courseChapter).ToList();

            return View(Content);
        }

        // GET: CourseChapter/Create
        [Authorize(Roles = "Admin, Instructor")]

        public IActionResult Create(int courseChapterId)
        {
            var courseChapters = _dbContext.CourseChapters.Include(cc => cc.Course).ToList();

            ViewBag.CourseChapters = courseChapters.Select(cc => new SelectListItem
            {
                Value = cc.Id.ToString(),
                Text = $"{cc.Name}/{cc.Course.EnTitle}"
            }).ToList();
            var model = new CourseChapterContent
            {
                CourseChapterId = courseChapterId
            };
            return View(model);
        }




        // POST: CourseChapter/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Instructor")]

        public IActionResult Create(CourseChapterContent content, IFormFile file)
        {

            string wwwRootPath = _env.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"images");
                var extension = Path.GetExtension(file.FileName);

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }
                content.FilePath = @"\images\" + fileName + extension;
            }

            _dbContext.CourseChapterContent.Add(content);
            _dbContext.SaveChanges();
            var courseId = _dbContext.CourseChapters
       .Include(cc => cc.Course)
       .FirstOrDefault(cc => cc.Id == content.CourseChapterId)
       ?.Course?.Id;
            return RedirectToAction("Details", "Course", new { id = courseId });
        }





        // GET: CourseChapter/Edit/5
        [Authorize(Roles = "Admin, Instructor")]

        public IActionResult Edit(int? id)
        {
            ViewBag.courseChapter = _dbContext.CourseChapters.ToList();

            if (id == null)
            {
                return NotFound();
            }

            var content = _dbContext.CourseChapterContent.FirstOrDefault(cc => cc.Id == id);
            if (content == null)
            {
                return NotFound();
            }

            return View(content);
        }

        // POST: CourseChapter/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Instructor")]

        public IActionResult Edit(int id, CourseChapterContent content, IFormFile file)
        {
            if (id != content.Id)
            {
                return NotFound();
            }

            string wwwRootPath = _env.WebRootPath;

            // Check if a new file is uploaded
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"images");
                var extension = Path.GetExtension(file.FileName);

                // Delete the old image file if it exists
                if (!string.IsNullOrEmpty(content.FilePath))
                {
                    var oldImagePath = Path.Combine(wwwRootPath, content.FilePath.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Save the new image file
                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }
                content.FilePath = @"\images\" + fileName + extension;
            }

            _dbContext.CourseChapterContent.Update(content);
            _dbContext.SaveChanges();
            var courseId = _dbContext.CourseChapters
       .Include(cc => cc.Course)
       .FirstOrDefault(cc => cc.Id == content.CourseChapterId)
       ?.Course?.Id;
            return RedirectToAction("Details", "Course", new { id = courseId });
        }





        // POST: CourseChapter/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Instructor")]

        public IActionResult DeleteConfirmed(int id)
        {
            var content = _dbContext.CourseChapterContent.Find(id);

            // Delete the image file from the images path
            if (content != null && !string.IsNullOrEmpty(content.FilePath))
            {
                var imagePath = Path.Combine(_env.WebRootPath, content.FilePath.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _dbContext.CourseChapterContent.Remove(content);
            _dbContext.SaveChanges();
            var courseId = _dbContext.CourseChapters
       .Include(cc => cc.Course)
       .FirstOrDefault(cc => cc.Id == content.CourseChapterId)
       ?.Course?.Id;
            return RedirectToAction("Details", "Course", new { id = courseId });
        }
    }
}
