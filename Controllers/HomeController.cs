using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using NestHR.LanguageSupport;
using NestHR.Models;
using System.Diagnostics;

namespace NestHR.Controllers
{
    public class HomeController : Controller
    {
        private LanguageService _localization;
        public HomeController(LanguageService localization)
        {
            _localization = localization;
        }

        public IActionResult Index()
        {
           // var currentCulture = Thread.CurrentThread.CurrentUICulture.Name;
            ViewData["Greeting"] = _localization.Getkey("lbl_Home").Value;
            return View();
        }



        #region Localization
        public IActionResult ChangeLanguage(string culture)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions() { Expires = DateTimeOffset.UtcNow.AddYears(1) });

            return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
