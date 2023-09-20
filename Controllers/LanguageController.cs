using Microsoft.AspNetCore.Mvc;
using LMS.Extensions;
using System;

namespace LMS.Controllers
{
    public class LanguageController : Controller
    {
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Language.SetCulture(Response.Cookies, culture, null);

            return LocalRedirect(returnUrl);
        }
    }
}
