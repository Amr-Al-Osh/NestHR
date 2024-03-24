using Domin.Models;
using HRService.GeneralDefinitionService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NestHR.LanguageSupport;
using System.Linq.Expressions;
using System.Reflection;

namespace NestHR.Controllers.GeneralDefinition
{
    public class AreaController : HRBaseController
    {
        private readonly IHRDefinitionWrapper _db;

        public AreaController(IHRDefinitionWrapper db, LanguageService localization) : base(localization)
        {
            _db = db;
        }


        [Route("Area")]
        public IActionResult Area() => View();

        #region =================> [Get Data]

        [HttpGet("GetMaxAreaNum")]
        public async Task<IActionResult> GetMaxAreaNum() => Ok(await _db.Area.GetMaxAsync(x => x.AreaNum));

        [HttpGet("GetAllArea")]
        public async Task<IActionResult> GetAllArea() => Ok(await _db.Area.ReadAllAsync());

        [HttpGet("GetAreaBy/{areaNum}")]
        public IActionResult GetAreaBy(int areaNum)
        {
            var existArea = _db.Area.GetBy(x => x.AreaNum == areaNum);

            if (existArea.Count() == 0)
            {
                return NotFound("Area not found.");
            }

            return Ok(existArea.FirstOrDefault());
        }

        [HttpPost("GetDataTableArea")]
        public async Task<IActionResult> GetDataTableArea()
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

            var data = await _db.Area.ReadAllAsync();

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

        [HttpPost("NewArea")]
        public async Task<IActionResult> NewArea([FromBody] Area model)
        {
            try
            {
                var existingArea = _db.Area.GetBy(x => (x.NameAr == model.NameAr && x.NameAr != "") || (x.NameEng == model.NameEng && x.NameEng != ""));

                if (existingArea.Any())
                {
                    return BadRequest($"Area with the same name = {model.NameAr ?? model.NameEng} already exists.");
                }

                model.AreaNum = await _db.Area.GetMaxAsync(x => x.AreaNum);

                await _db.Area.AddAsync(model);

                await _db.Area.SaveChangesAsync();

                await _db.UserLog.AddUserLogAsync(1, "userName", 1, $"Add New Area Name =[{model.NameAr ?? model.NameEng}] have Number = [{model.AreaNum}]");

                return Ok(true);

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

                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("EditeArea")]
        public async Task<IActionResult> EditeArea([FromBody] Area model)
        {
            try
            {
                int lang = 1;

                var area = _db.Area.GetAll().ToList();

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

                await _db.UserLog.AddUserLogAsync(1, "userName", 2, $"Edit Area with Number = [{model.AreaNum}] from Name = [{(lang == 1 ? existingArea.NameAr : existingArea.NameEng)}] to Name = [{(lang == 1 ? model.NameAr : model.NameEng)}]");

                existingArea.NameEng = model.NameEng ?? "";
                existingArea.NameAr = model.NameAr ?? "";

                _db.Area.Update(existingArea);

                await _db.Area.SaveChangesAsync();

                return Ok(true);
            }
            catch (Exception ex)
            {
                ErrorLog error = _db.ErrorLog.AddErrorLog(
                    new ErrorLog
                    {
                        UserNumRef = 1,
                        UserName = "userName",
                        PlaceName = MethodBase.GetCurrentMethod()?.DeclaringType?.FullName ?? "",
                        ExMessage = $"{ex.Message}",
                        InnerExceptionMessage = $"{(ex.InnerException != null ? ex.InnerException.Message : "")}",
                    });

                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("DealetArea/{areaNum}")]
        public async Task<IActionResult> DealetArea(int areaNum)
        {
            try
            {
                var existingArea = _db.Area.GetBy(x => x.AreaNum == areaNum).FirstOrDefault();

                if (existingArea is null)
                {
                    return NotFound($"Area with this number = [{areaNum}] Not found.");
                }

                _db.Area.Remove(existingArea);

                await _db.Area.SaveChangesAsync();

                await _db.UserLog.AddUserLogAsync(1, "userName", 3, $"Delete Area Name = [{existingArea.NameAr ?? existingArea.NameEng}] have Number = [{existingArea.AreaNum}]");

                return Ok(true);
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

                return StatusCode(500, error);
            }
        }

        #endregion

    }
}
