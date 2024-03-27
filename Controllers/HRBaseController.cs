using Domin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NestHR.LanguageSupport;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NestHR.Controllers
{
    public class HRBaseController(LanguageService localization, IConfiguration config, IHttpContextAccessor httpContextAccessor) : Controller
    {
        protected readonly LanguageService _localization = localization;

        public static User? userData;

        private IConfiguration _config = config;

        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;


        public void SaveUserInCookies(User userModel)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var serializedUser = JsonConvert.SerializeObject(userModel);
                httpContext.Response.Cookies.Append("UserData", serializedUser, new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(7),
                    Secure = false,
                    HttpOnly = false,
                });
            }
        }

        public void SaveTokenInCookies(string token)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                httpContext.Response.Cookies.Append("HrToken", token, new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(7),
                    Secure = false,
                    HttpOnly = false,
                });
            }
        }

        public User? GetDataFromCookies()
        {
            var userDataCookie = Request.Cookies["UserData"];
            if (!string.IsNullOrEmpty(userDataCookie))
            {
                return JsonConvert.DeserializeObject<User>(userDataCookie);
            }
            return null;
        }

        public string CryptPassword(string Password) => BCrypt.Net.BCrypt.HashPassword(Password);

        public bool VerifyCryptPassword(string CryptPassword, string Password) => BCrypt.Net.BCrypt.Verify(CryptPassword, Password);

        [HttpPost("CreatToken")]
        public string CreatToken([FromBody] User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("UserNum", user.UserNum.ToString()),
                new Claim("Password", user.Password),
                new Claim("UserName", user.UserName),
                new Claim("Lang", (user.Lang??0).ToString()),
                new Claim("Theam", (user.Theam??0).ToString())
            };

            var token = new JwtSecurityToken(
                _config["JwtSettings:Issuer"],
                _config["JwtSettings:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string? CheckIfHaveToken()
        {
            var token = Request.Cookies["HrToken"];

            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            userData = GetDataFromCookies();

            return token;
        }


    }
}
