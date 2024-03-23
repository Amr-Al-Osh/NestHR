using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NestHR.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NestHR.Authentication
{
    [Route("Auths")]   
    public class AuthController : Controller
    {
        public static Users user = new Users();
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        [HttpPost("register")]
        public ActionResult<Users> Register(UserDto request)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Paswword);

            user.UserNum = request.UserNum;
            user.UserName = request.UserName;
            user.Password = passwordHash;

            return Ok(user);

        }

        [HttpPost("login")]
        public ActionResult<Users> login(UserDto request)
        {
            if (user.UserName != request.UserName)
            {
                return BadRequest("User Not Found");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Paswword, user.Password))
            {
                return BadRequest("Paswword Not Found");
            }

            string token = CreatToken(user);

            return Ok(token);

        }

        private string CreatToken(Users user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,user.UserName),
            };

            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
               claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;

        }


    }
}
