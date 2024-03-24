using Domin.Models;
using Microsoft.AspNetCore.Mvc;
using NestHR.LanguageSupport;
using Newtonsoft.Json;

namespace NestHR.Controllers
{
    public class HRBaseController(LanguageService localization) : Controller
    {
        protected readonly LanguageService _localization = localization;

        public void SaveUserInCookies(User userModel) => Response.Cookies.Append("UserData", JsonConvert.SerializeObject(userModel),
            new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7),
                Secure = true,
                HttpOnly = true,
            });

        public User GetDataFromCookies() => JsonConvert.DeserializeObject<User>(Request.Cookies["UserData"]);

        public string CryptPassword(string Password) => BCrypt.Net.BCrypt.HashPassword(Password);

        public bool VerifyCryptPassword(string CryptPassword, string Password) => BCrypt.Net.BCrypt.Verify(CryptPassword, Password);





    }
}
