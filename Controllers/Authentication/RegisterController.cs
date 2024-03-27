using Domin.Models;
using HRService.GeneralDefinitionService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NestHR.LanguageSupport;
using System.Reflection;

namespace NestHR.Controllers.Authentication
{
    public class RegisterController : HRBaseController
    {
        private readonly IHRDefinitionWrapper _db;

        public RegisterController(IHRDefinitionWrapper db, LanguageService localization, IConfiguration config, IHttpContextAccessor httpContextAccessor)
            : base(localization, config, httpContextAccessor) => _db = db;


        [Route("Register")]
        public IActionResult RegisterPage()
        {
            return View();
        }


        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser([FromBody] User userModel)
        {
            try
            {
                bool isUserExist = _db.Users.
                    GetBy(x => x.UserName == userModel.UserName).Any();

                if (isUserExist)
                {
                    return BadRequest(_localization.Getkey("lbl_UserExist").Value);
                }

                userModel.Password = CryptPassword(userModel.Password);


                await _db.Users.AddAsync(userModel);
                await _db.Users.SaveChangesAsync();

                await _db.UserLog.AddUserLogAsync(1, "userName", 1, $"Add New User Name = [{userModel.UserName}] have Number = [{userModel.UserNum}]");

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                ErrorLog error = await _db.ErrorLog.AddErrorLogAsync(
                   new ErrorLog
                   {
                       UserNumRef = 1,
                       UserName = "userName",
                       PlaceName = MethodBase.GetCurrentMethod()?.DeclaringType?.FullName ?? "",
                       ExMessage = $"{ex.Message}",
                       InnerExceptionMessage = $"{(ex.InnerException != null ? ex.InnerException.Message : "")}",
                   });
                return BadRequest(new { success = false });
            }
        }


        [HttpPut("EditUser")]
        public async Task<IActionResult> EditUser([FromBody] User userModel)
        {
            try
            {
                var user = await _db.Users.GetAsync();

                var userExist = user.FirstOrDefault(x => x.UserNum == userModel.UserNum);

                if (userExist is null)
                {
                    return NotFound(_localization.Getkey("lbl_UserNotFound").Value);
                }

                if (!string.Equals(userExist.UserName, userModel.UserName, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (user.Any(x => x.UserName.Equals(userModel.UserName, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        return BadRequest(_localization.Getkey("lbl_UserExist").Value);
                    }
                }

                userExist.UserName = userModel.UserName;
                userExist.Password = CryptPassword(userModel.Password);

                _db.Users.Update(userExist);
                await _db.Users.SaveChangesAsync();

                await _db.UserLog.AddUserLogAsync(GetDataFromCookies().UserNum, GetDataFromCookies().UserName, 2, $"Edite User Name = [{userExist.UserName}] have Number = [{userExist.UserNum}]");

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                ErrorLog error = await _db.ErrorLog.AddErrorLogAsync(
                   new ErrorLog
                   {
                       UserNumRef = GetDataFromCookies().UserNum,
                       UserName = GetDataFromCookies().UserName,
                       PlaceName = MethodBase.GetCurrentMethod()?.DeclaringType?.FullName ?? "",
                       ExMessage = $"{ex.Message}",
                       InnerExceptionMessage = $"{(ex.InnerException != null ? ex.InnerException.Message : "")}",
                   });
                return BadRequest(new { success = false });
            }
        }


        [HttpDelete("DeleteUser/{userNum}")]
        public async Task<IActionResult> DeleteUser(int userNum)
        {
            try
            {
                var userExist = _db.Users.GetBy(x => x.UserNum == userNum).FirstOrDefault();

                if (userExist is null)
                {
                    return NotFound(_localization.Getkey("lbl_UserNotFound").Value);
                }

                _db.Users.Remove(userExist);
                await _db.Users.SaveChangesAsync();

                await _db.UserLog.AddUserLogAsync(1, "userName", 3, $"Delete User Name = [{userExist.UserName}] have Number = [{userExist.UserNum}]");

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                ErrorLog error = await _db.ErrorLog.AddErrorLogAsync(
                   new ErrorLog
                   {
                       UserNumRef = 1,
                       UserName = "userName",
                       PlaceName = MethodBase.GetCurrentMethod()?.DeclaringType?.FullName ?? "",
                       ExMessage = $"{ex.Message}",
                       InnerExceptionMessage = $"{(ex.InnerException != null ? ex.InnerException.Message : "")}",
                   });

                return BadRequest(new { success = false });
            }
        }



    }
}
