using System.Security.Claims;
using System.Threading.Tasks;
using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
    public class InstructorUserController : Controller
    {
        private readonly ApplicationDbContext _context;  // Assuming you have a DbContext named 'ApplicationDbContext'
        private readonly IWebHostEnvironment _env;

        public InstructorUserController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: InstructorUser/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: InstructorUser/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserName,ImageUrl,Description,JobTitle")] InstructorUser instructorUser, IFormFile file)
        {
            string wwwRootPath = _env.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"Instructor");
                var extension = Path.GetExtension(file.FileName);

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }
                instructorUser.ImageUrl = @"\Instructor\" + fileName + extension;
            }
            if (ModelState.IsValid)
            {

                // Assign the current user's ID to the instructor's ID
                instructorUser.Id = User.FindFirstValue(ClaimTypes.NameIdentifier); 
                _context.Add(instructorUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = instructorUser.Id });
            }
            return View(instructorUser);
        }


        // GET: InstructorUser/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructorUser = await _context.InstructorUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (instructorUser == null)
            {
                return NotFound();
            }

            return View(instructorUser);
        }

        // GET: InstructorUser/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructorUser = await _context.InstructorUsers.FindAsync(id);
            if (instructorUser == null)
            {
                return NotFound();
            }
            return View(instructorUser);
        }

        // POST: InstructorUser/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,UserName,ImageUrl,Description,JobTitle")] InstructorUser instructorUser, IFormFile file)
        {
            string wwwRootPath = _env.WebRootPath;

            // Check if a new file is uploaded
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"Instructor");
                var extension = Path.GetExtension(file.FileName);

                // Delete the old image file if it exists
                if (!string.IsNullOrEmpty(instructorUser.ImageUrl))
                {
                    var oldImagePath = Path.Combine(wwwRootPath, instructorUser.ImageUrl.TrimStart('\\'));
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
                instructorUser.ImageUrl = @"\Instructor\" + fileName + extension;
            }

            if (id != instructorUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(instructorUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstructorUserExists(instructorUser.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = instructorUser.Id });
            }
            return View(instructorUser);
        }

        private bool InstructorUserExists(string id)
        {
            return _context.InstructorUsers.Any(e => e.Id == id);
        }
    }
}
