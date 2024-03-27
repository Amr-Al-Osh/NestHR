using Domin.Models;
using HRService.GeneralDefinitionService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NestHR.LanguageSupport;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace NestHR.Controllers.GeneralDefinition
{
    [Authorize]
    [Route("Area")]
    public class AreaController : HRBaseController
    {
        private readonly IHRDefinitionWrapper _db;

        public AreaController(IHRDefinitionWrapper db, LanguageService localization, IConfiguration config, IHttpContextAccessor httpContextAccessor)
            : base(localization, config, httpContextAccessor) => _db = db;

        [AllowAnonymous]
        [Route("AreaPage")]
        public IActionResult AreaPage()
        {
            var token = CheckIfHaveToken();

            if (token is null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Token = token;

            return View();
        }


        #region =================> [Get Data]

        [HttpGet("MaxNum")]
        public async Task<IActionResult> MaxNum() => Ok(await _db.Area.MaxAsync(x => x.AreaNum));


        [HttpGet("Get")]
        public async Task<IActionResult> Get() => Ok(await _db.Area.ReadAsync());


        [HttpGet("GetBy/{areaNum}")]
        public async Task<IActionResult> GetBy(int areaNum)
        {
            var existArea = await _db.Area.GetByAsync(x => x.AreaNum == areaNum);

            if (existArea.Count() == 0)
            {
                return NotFound("Area not found.");
            }

            return Ok(existArea.FirstOrDefault());
        }


        [HttpPost("DataTable")]
        public async Task<IActionResult> DataTable()
        {
            int totalRecord = 0;
            int filterRecord = 0;
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumnIndex = int.TryParse(Request.Form["order[0][column]"].FirstOrDefault(), out int index) ? index : 0;
            var sortColumnName = Request.Form[$"columns[{sortColumnIndex}][name]"].FirstOrDefault();
            var sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            var data = await _db.Area.ReadAsync();

            totalRecord = data.Count();

            if (!string.IsNullOrEmpty(searchValue))
            {
                data = data.Where(x =>
                    x.AreaNum.ToString().Contains(searchValue) ||
                    (x.NameAr ?? "").Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                    (x.NameEng ?? "").Contains(searchValue, StringComparison.OrdinalIgnoreCase));
            }

            filterRecord = data.Count();
            //sort data
            if (!string.IsNullOrEmpty(sortColumnName) && !string.IsNullOrEmpty(sortDirection))
            {
                var property = typeof(Area).GetProperty(sortColumnName);
                if (property != null)
                {
                    var orderByExpression = Expression.Lambda<Func<Area, object>>(
                        Expression.Convert(
                            Expression.Property(
                                Expression.Parameter(typeof(Area), "x"),
                                property),
                            typeof(object)),
                        Expression.Parameter(typeof(Area), "x"));

                    if (sortDirection.Equals("desc", StringComparison.CurrentCultureIgnoreCase))
                    {
                        data = data.OrderByDescending(orderByExpression);
                    }
                    else
                    {
                        data = data.OrderBy(orderByExpression);
                    }
                }
            }

            var result = data.Skip(skip).Take(pageSize).ToList();

            return Ok(new
            {
                draw = draw,
                recordsTotal = totalRecord,
                recordsFiltered = filterRecord,
                data = result
            });
        }

        #endregion


        #region =================> [Operation On Data]

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] Area model)
        {
            try
            {
                var existingArea = _db.Area.GetBy(x => (x.NameAr == model.NameAr && x.NameAr != "") || (x.NameEng == model.NameEng && x.NameEng != ""));

                if (existingArea.Any())
                {
                    return BadRequest($"Area with the same name = {model.NameAr ?? model.NameEng} already exists.");
                }

                model.AreaNum = await _db.Area.MaxAsync(x => x.AreaNum);

                await _db.Area.AddAsync(model);

                bool success = await _db.Area.SaveChangesAsync();

                await _db.UserLog.AddUserLogAsync(
                    userData.UserNum,
                    userData.UserName,
                    1,
                    $"Add New Area Name = [{(userData.Lang == 1 ? model.NameAr : model.NameEng)}] with Number = [{model.AreaNum}]"
                    );

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


        [HttpPut("Edite")]
        public async Task<IActionResult> Edite([FromBody] Area model)
        {
            try
            {
                int lang = 1;

                var area = await _db.Area.GetAsync();

                var existingArea = area.FirstOrDefault(x => x.AreaNum == model.AreaNum);

                if (existingArea is null)
                {
                    return NotFound($"Area with this number = [{model.AreaNum}] Not found.");
                }

                var checkNameExist = area.Any(x => lang == 1 ?
                    (x.NameAr ?? "").Equals(model.NameAr ?? "", StringComparison.CurrentCultureIgnoreCase) :
                    (x.NameEng ?? "").Equals(model.NameEng ?? "", StringComparison.CurrentCultureIgnoreCase));

                var isNameChangeConflict = checkNameExist ? (lang == 1 ?
                    !(existingArea.NameAr ?? "").Equals(model.NameAr ?? "", StringComparison.CurrentCultureIgnoreCase) :
                    !(existingArea.NameEng ?? "").Equals(model.NameEng ?? "", StringComparison.CurrentCultureIgnoreCase))
                    : false;

                if (isNameChangeConflict)
                {
                    return BadRequest($"An area with the same name [{model.NameAr ?? model.NameEng}] already exists.");
                }

                await _db.UserLog.AddUserLogAsync(userData.UserNum, userData.UserName, 2, $"Edit Area with Number = [{model.AreaNum}] from Name = [{(lang == 1 ? existingArea.NameAr : existingArea.NameEng)}] to Name = [{(lang == 1 ? model.NameAr : model.NameEng)}]");

                existingArea.NameEng = model.NameEng ?? "";
                existingArea.NameAr = model.NameAr ?? "";

                _db.Area.Update(existingArea);

                bool success = await _db.Area.SaveChangesAsync();

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


        [HttpDelete("Dealet/{areaNum}")]
        public async Task<IActionResult> Dealet(int areaNum)
        {
            try
            {
                var existingArea = _db.Area.GetBy(x => x.AreaNum == areaNum).FirstOrDefault();

                if (existingArea is null)
                {
                    return NotFound($"Area with this number = [{areaNum}] Not found.");
                }

                _db.Area.Remove(existingArea);

                bool success = await _db.Area.SaveChangesAsync();

                await _db.UserLog.AddUserLogAsync(userData.UserNum, userData.UserName, 3, $"Delete Area Name = [{existingArea.NameAr ?? existingArea.NameEng}] have Number = [{existingArea.AreaNum}]");

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

        #endregion

    }
}
