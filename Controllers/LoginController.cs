using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using LMS.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using LMS.Models.ViewModel;

namespace LMS.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public LoginController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }



        [HttpGet]
        public IActionResult Index()
        {
            string username = HttpContext.Session.GetString("username");
            string userId = HttpContext.Session.GetString("userId");
            string Imageurl = HttpContext.Session.GetString("Imageurl");

            LoginViewModel model = new LoginViewModel
            {
                Username = username,
                userId = userId,
                ImageUrl = Imageurl
            };

            return View(model);
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (model == null)
            {
                Console.WriteLine("Model is null");
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _clientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("https://imam.academy/JWTauthentication/login", new { Username = model.Username, Password = model.Password });

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Response was not successful");
            }

            var content = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

            if (content == null || !content.ContainsKey("token"))
            {
                Console.WriteLine("Content is null or doesn't contain 'token'");
            }

            var token = content["token"];
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtTokenHandler.ReadJwtToken(token);
            if (HttpContext.Session == null)
            {
                Console.WriteLine("Session is null");
            }

            // Save the token in the user's session, or wherever you want to store it
            HttpContext.Session.SetString("token", token);

            // Save the token in a cookie
            Response.Cookies.Append("jwt", token, new CookieOptions { HttpOnly = true });
            var userIdFromToken = jwtToken.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            model.userId = userIdFromToken;  // Assign the userId to the model
            var imageProfileFromToken = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "ImageProfile")?.Value;
            model.ImageUrl = imageProfileFromToken;
            HttpContext.Session.SetString("username", model.Username);
            HttpContext.Session.SetString("userId", model.userId);  // Storing userId in the session
            if (!string.IsNullOrEmpty(imageProfileFromToken))
            {
                HttpContext.Session.SetString("imageProfile", imageProfileFromToken);
            }

            var rawContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(rawContent);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Response was not successful: {response.StatusCode}. Content: {rawContent}");
                return View("Error");  // Or another appropriate error-handling mechanism.
            }

            return RedirectToAction("Index", "Home");
        }


        
       [HttpGet]
        public async Task<IActionResult> UserDetails()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                using (var httpClient = _clientFactory.CreateClient())
                {
                    var response = await httpClient.GetAsync($"https://imam.academy/JWTauthentication/GetUserById/{userId}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var userDetails = JsonConvert.DeserializeObject<UserDetailsDto>(content);

                        if (userDetails == null)
                        {
                            return View("UserDetailsNotFound");
                        }

                        var model = new UserDetailsDto
                        {
                            UserName = userDetails.UserName,
                            ProfilePicture = userDetails.ProfilePicture
                        };

                        return View(model); // This line renders the UserDetails.cshtml view
                    }
                    else
                    {
                        throw new ApplicationException($"Error fetching user details. StatusCode: {response.StatusCode}");
                    }
                }
            }
            catch (ApplicationException ex)
            {
                return View("ErrorView", new ErrorViewModel { ErrorMessage = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            // 1. Clear any authentication data you have in cookies, session, or any other storage
            HttpContext.Session.Clear();  // Clear everything from the session
            Response.Cookies.Delete("jwt"); // Remove JWT cookie

            // 2. If you have any other custom middleware or services that manage user state, clear them here.
            // For instance, if you're using a custom caching mechanism:
            // _userCacheService.ClearCurrentUser();

            // 3. Redirect the user to a suitable page, typically the login page or the home page.
            return RedirectToAction("Index", "Home");  // Replace 'YourControllerName' with the name of your controller that handles authentication.
        }








        public class ErrorViewModel
        {
            public string ErrorMessage { get; set; }
        }


    }

}
