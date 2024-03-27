using Domin.Models;
using HRService.GeneralDefinitionService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NestHR.LanguageSupport;
using System.Reflection;

namespace NestHR.Controllers.GeneralDefinition
{
    [Authorize]
    [Route("Company")]
    public class CompanyProfileController : HRBaseController
    {
        private readonly IHRDefinitionWrapper _db;

        public CompanyProfileController(IHRDefinitionWrapper db, LanguageService localization, IConfiguration config, IHttpContextAccessor httpContextAccessor)
            : base(localization, config, httpContextAccessor) => _db = db;


        [AllowAnonymous]
        [Route("CompanyProfile")]
        public IActionResult CompanyProfilePage()
        {
            var token = CheckIfHaveToken();

            if (token is null)
            {
                return RedirectToAction("Index", "Home");
            }

            userData = GetDataFromCookies();

            ViewBag.Token = token;

            return View();
        }


        [HttpGet("Get")]
        public async Task<IActionResult> Get() => Ok(await _db.CompProfile.ReadAsync());


        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromBody] CompanyProfile companyModel)
        {
            try
            {
                var isCompanyexist = await _db.CompProfile.GetAsync();

                if (isCompanyexist.Any())
                {
                    await _db.CompProfile.AddAsync(companyModel);

                    await _db.UserLog.AddUserLogAsync(userData.UserNum, userData.UserName, 1, $"Add Company Data");
                }
                else
                {
                    _db.CompProfile.Update(companyModel);

                    await _db.UserLog.AddUserLogAsync(userData.UserNum, userData.UserName, 2, $"Update Company Data");
                }

                bool success = await _db.CompProfile.SaveChangesAsync();

                return Ok(new { success });

            }
            catch (Exception ex)
            {
                await _db.ErrorLog.AddErrorLogAsync(
                    new ErrorLog
                    {
                        UserNumRef = userData.UserNum,
                        UserName = userData.UserName,
                        PlaceName = MethodBase.GetCurrentMethod()?.DeclaringType?.FullName ?? "",
                        ExMessage = $"{ex.Message}",
                        InnerExceptionMessage = $"{(ex.InnerException != null ? ex.InnerException.Message : "")}",
                    });

                return StatusCode(500, ex.Message);
            }
        }

    }
}
