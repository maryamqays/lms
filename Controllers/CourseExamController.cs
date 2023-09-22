using Lms.Models;
using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
    public class CourseExamController : Controller
    {
        private readonly ApplicationDbContext _context;


        public CourseExamController(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        // GET: CourseExam
        public ActionResult Index(int courseId)
        {
            var exams = _context.CourseExams.Where(e => e.CourseId == courseId).ToList();
            return View(exams);
        }



        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CourseExam exam)
        {
            
                _context.CourseExams.Add(exam);
                _context.SaveChanges();
            var course = _context.Courses.Find(exam.CourseId);

            return RedirectToAction("Details", "Course", new { id = exam.CourseId });

        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var exam = _context.CourseExams.Find(id);
            if (exam == null)
            {
                return NotFound();
            }

            return View(exam);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CourseExam exam)
        {
            
                _context.Entry(exam).State = EntityState.Modified;
                _context.SaveChanges();
            int courseId = exam.CourseId;

            return RedirectToAction("Index", new { courseId = courseId });


        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var exam = _context.CourseExams.Find(id);
            if (exam == null)
            {
                return NotFound();
            }

            int courseId = exam.CourseId;

            _context.CourseExams.Remove(exam);
            _context.SaveChanges();

            return RedirectToAction("Index", new { courseId = courseId });
        }


    }
}
