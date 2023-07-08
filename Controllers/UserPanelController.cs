using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using LMS.Models;
using System.Threading.Tasks;
using LMS.Data;
using Lms.Models;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using LMS.Models.ViewModel;

namespace LMS.Controllers
{
    public class UserPanelController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserPanelController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return RedirectToAction("About", "Home");
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUser updatedUser)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByIdAsync(updatedUser.Id);

                if (user == null)
                {
                    return NotFound();
                }

                user.UserName = updatedUser.UserName;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(updatedUser);
        }
        public IActionResult Index()
        {


            return View();
        }

    }
}
