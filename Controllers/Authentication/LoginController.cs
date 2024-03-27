using Domin.Models;
using HRService.GeneralDefinitionService.Interfaces;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using NestHR.LanguageSupport;

namespace NestHR.Controllers.Authentication
{    
    public class LoginController : HRBaseController
    {
        private readonly IHRDefinitionWrapper _db;

        public LoginController(IHRDefinitionWrapper db, LanguageService localization, IConfiguration config, IHttpContextAccessor httpContextAccessor)
            : base(localization, config, httpContextAccessor) => _db = db;


        public IActionResult LoginPage()
        {
            return View();
        }


        [HttpPost("CheakLoginManager")]
        public IActionResult CheakLoginManager([FromBody] User userModel)
        {
            var usersWithMatchingUsername = _db.Users
                .GetBy(x => x.UserName == userModel.UserName)
                .ToList();

            var isUserExist = usersWithMatchingUsername
                .FirstOrDefault(user => VerifyCryptPassword(userModel.Password, user.Password));


            if (isUserExist is null)
            {
                return NotFound(_localization.Getkey("lbl_UserNotFound").Value);
            }

            isUserExist.Lang = userModel.Lang ?? isUserExist.Lang;
            isUserExist.Theam = userModel.Theam ?? isUserExist.Theam;

            _db.Users.Update(isUserExist);
            _db.Users.SaveChanges();

            string tokenResult = CreatToken(isUserExist);

            SaveTokenInCookies(tokenResult);

            return Ok(tokenResult);
        }


        [HttpPost("ChangeLanguage")]
        public IActionResult ChangeLanguage(int lang)
        {
            var sytemLang = _db.HrSystemLange.GetBy(x => x.LangId == lang).FirstOrDefault();

            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(sytemLang?.LangCode??"ar-JO")),
                new CookieOptions() { Expires = DateTimeOffset.UtcNow.AddYears(1) });

            return Redirect(Request.Headers["Referer"].ToString());
        }


    }
}
