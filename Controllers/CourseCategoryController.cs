using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace LMS.Controllers
{
    public class CourseCategoryController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _env;

        public CourseCategoryController(ApplicationDbContext dbContext, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _env = env;
        }

        // GET: CourseCategory
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View(_dbContext.CourseCategories.ToList());
        }

        // GET: CourseCategory/Create
        [Authorize(Roles = "Admin, Instructor")]

        public IActionResult Create()
        {
            return View();
        }

        // POST: CourseCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Instructor")]

        public IActionResult Create(CourseCategory category, IFormFile file)
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
                category.Image = @"\images\" + fileName + extension;
            }

            _dbContext.CourseCategories.Add(category);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }





        // GET: CourseCategory/Edit/5
        [Authorize(Roles = "Admin, Instructor")]

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseCategory = _dbContext.CourseCategories.FirstOrDefault(cc => cc.Id == id);
            if (courseCategory == null)
            {
                return NotFound();
            }

            return View(courseCategory);
        }

        // POST: CourseCategory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Instructor")]

        public IActionResult Edit(int id, CourseCategory courseCategory, IFormFile file)
        {
            if (id != courseCategory.Id)
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
                if (!string.IsNullOrEmpty(courseCategory.Image))
                {
                    var oldImagePath = Path.Combine(wwwRootPath, courseCategory.Image.TrimStart('\\'));
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
                courseCategory.Image = @"\images\" + fileName + extension;
            }

            _dbContext.Update(courseCategory);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }





        // POST: CourseCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Instructor")]

        public IActionResult DeleteConfirmed(int id)
        {
            var courseCategory = _dbContext.CourseCategories.Find(id);

            // Delete the image file from the images path
            if (courseCategory != null && !string.IsNullOrEmpty(courseCategory.Image))
            {
                var imagePath = Path.Combine(_env.WebRootPath, courseCategory.Image.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _dbContext.CourseCategories.Remove(courseCategory);
            _dbContext.SaveChanges();
            return RedirectToAction("Index", "Home");
        }


    }
}