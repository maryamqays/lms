using LMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace LMS.Controllers
{
    [Authorize(Roles = "Admin, Instructor")]

    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }



        [HttpGet]
        public IActionResult GetRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }
        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                // Handle validation error
                return RedirectToAction("GetRoles", "Role"); // Redirect to a suitable page
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                {
                    // Role created successfully
                    return RedirectToAction("GetRoles", "Role"); // Redirect to a suitable page
                }
                else
                {
                    // Handle role creation error
                    return RedirectToAction("GetRoles", "Role"); // Redirect to a suitable page
                }
            }
            else
            {
                // Role already exists
                return RedirectToAction("GetRoles", "Role"); // Redirect to a suitable page
            }
        }
        [HttpGet]
        public IActionResult EditRole(string currentRoleName)
        {
            // Retrieve the current role from the role manager or perform any necessary validations

            ViewBag.CurrentRoleName = currentRoleName;

            return View();
        }



        [HttpPost]
        public async Task<IActionResult> EditRole(string currentRoleName, string newRoleName)
        {
            if (string.IsNullOrEmpty(currentRoleName) || string.IsNullOrEmpty(newRoleName))
            {
                // Handle validation error
                return RedirectToAction("GetRoles", "Role"); // Redirect to a suitable page
            }

            var role = await _roleManager.FindByNameAsync(currentRoleName);
            if (role != null)
            {
                role.Name = newRoleName;
                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    // Role updated successfully
                    return RedirectToAction("GetRoles", "Role"); // Redirect to a suitable page
                }
                else
                {
                    // Handle role update error
                    return RedirectToAction("GetRoles", "Role"); // Redirect to a suitable page
                }
            }
            else
            {
                // Role does not exist
                return RedirectToAction("GetRoles", "Role"); // Redirect to a suitable page
            }
        }


        [HttpPost]
        public async Task<IActionResult> RemoveRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                // Handle validation error
                return RedirectToAction("GetRoles", "Role"); // Redirect to a suitable page
            }

            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    // Role deleted successfully
                    return RedirectToAction("GetRoles", "Role"); // Redirect to a suitable page
                }
                else
                {
                    // Handle role deletion error
                    return RedirectToAction("GetRoles", "Role"); // Redirect to a suitable page
                }
            }
            else
            {
                // Role does not exist
                return RedirectToAction("GetRoles", "Role"); // Redirect to a suitable page
            }
        }
        [HttpGet]
        public IActionResult CreateUserWithRole()
        {
          
            var roles = _roleManager.Roles.ToList();
            ViewBag.Roles = new SelectList(roles, "Name", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserWithRole(string roleName, string userName, string password, string email)
        {
            if (string.IsNullOrEmpty(roleName) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
            {
                // Handle validation error
                return RedirectToAction("Index", "Role"); // Redirect to a suitable page
            }

            // Find the selected role
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                // Role not found
                return RedirectToAction("Index", "Role"); // Redirect to a suitable page
            }

            // Create a new user with email
            var user = new ApplicationUser
            {
                UserName = userName,
                Email = email
            };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                // Assign the role to the user
                var roleResult = await _userManager.AddToRoleAsync(user, roleName);
                if (roleResult.Succeeded)
                {
                    // User created successfully with the assigned role
                    return RedirectToAction("Index", "Role"); // Redirect to a suitable page
                }
                else
                {
                    // Handle role assignment error
                    return RedirectToAction("Index", "Role"); // Redirect to a suitable page
                }
            }
            else
            {
                // Handle user creation error
                return RedirectToAction("Index", "Role"); // Redirect to a suitable page
            }
        }






    }
}
