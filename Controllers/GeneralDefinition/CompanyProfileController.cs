using Domin.Models;
using HRService.GeneralDefinitionService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NestHR.LanguageSupport;
using System.Reflection;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;


namespace NestHR.Controllers.GeneralDefinition
{
    [Authorize]
    [Route("Company")]
    public class CompanyProfileController : HRBaseController
    {
        private readonly IHRDefinitionWrapper _db;

        public CompanyProfileController(IHRDefinitionWrapper db, LanguageService localization,
            IConfiguration config, IHttpContextAccessor httpContextAccessor)
            : base(localization, config, httpContextAccessor)
        {
            _db = db;
        }

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
                    companyModel.Logo = await CompressImg(companyModel.Logo);
                    companyModel.SignatureManeger = await CompressImg(companyModel.SignatureManeger);
                    companyModel.SignatureHr = await CompressImg(companyModel.SignatureHr);

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


        public async Task<byte[]?> CompressImg(byte[]? img)
        {
            try
            {
                if (img == null || img.Length == 0)
                {
                    return null;
                }

                using (var inputStream = new MemoryStream(img))
                {
                    using (var outputStream = new MemoryStream())
                    {
                        var image = await Image.LoadAsync(inputStream);

                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Mode = ResizeMode.Max,
                            Size = new Size(800, 600)
                        }));

                        await image.SaveAsync(outputStream, new PngEncoder());

                        return outputStream.ToArray();
                    }
                }

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

                return null;
            }
        }


    }
}
