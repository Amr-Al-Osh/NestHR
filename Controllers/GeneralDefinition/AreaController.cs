using Domin.Models;
using HRService.GeneralDefinitionService.Interfaces;
using HRService.LogHR.Interfaces;
using Microsoft.AspNetCore.Mvc;

using System.Linq.Expressions;
using System.Reflection;

namespace NestHR.Controllers.GeneralDefinition
{
    public class AreaController : Controller
    {
        private IHRDefinitionWrapper _db;
        private IHrLogWarpper _HrLog;
        public AreaController(IHRDefinitionWrapper db, IHrLogWarpper HrLog)
        {
            _db = db;
            _HrLog = HrLog;
        }

        [Route("Area")]
        public async Task<IActionResult> Area()
        {
            var data = await _db.Area.ReadAllAsync();

            return View(data);
        }

        #region =================> [Get Data]

        [HttpGet("GetAllArea")]
        public async Task<IActionResult> GetAllArea()
        {

            return Ok(await _db.Area.ReadAllAsync());
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
                    x.Id.ToString().Contains(searchValue) ||
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
            var users = await _db.Users.GetAllAsync();
            try
            {
                if (ModelState.IsValid)
                {
                    var existingArea = await _db.Area.GetByAsync(x => x.AreaNum == model.AreaNum);

                    if (existingArea != null)
                    {
                        return BadRequest("Area already exists.");
                    }

                    model.AreaNum = await _db.Area.GetMaxAsync(x => x.AreaNum);

                    _db.Area.Add(model);

                    await _db.Area.SaveChangesAsync();

                    var userNames = string.Join(", ", (await _db.Users.GetAllAsync()).Select(u => u.UserName));

                    await _HrLog.UserLog.AddUserLogAsync(1, "userName", 1, $"Add New Area Name = {model.NameAr??model.NameEng} have Number = {model.AreaNum}");

                    return Ok(true);
                }
                else
                {
                    //_logger.LogError("Invalid model state while adding new area: {@Model}", model);
                    return BadRequest(ModelState);
                }

            }
            catch (Exception ex)
            {
                ErrorLog error = await _HrLog.ErrorLog.AddErrorLogAsync(
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


        [HttpPost("EditeArea")]
        public async Task<IActionResult> EditeArea([FromBody] Area model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingArea = await _db.Area.GetByAsync(x => x.AreaNum == model.AreaNum);

                    if (existingArea is null)
                    {
                        return NotFound("Area not found.");
                    }
                    else
                    {
                        existingArea.NameEng = model.NameEng ?? "";
                        existingArea.NameAr = model.NameAr ?? "";

                        _db.Area.Update(existingArea);

                        await _db.Area.SaveChangesAsync();

                        await _HrLog.UserLog.AddUserLogAsync(1, "userName", 2, $"Edite Area Name = {model.NameAr ?? model.NameEng} have Number = {model.AreaNum}");

                        return Ok(true);
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                ErrorLog error = await _HrLog.ErrorLog.AddErrorLogAsync(
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


        [HttpPost("DealetArea")]
        public async Task<IActionResult> DealetArea(int areaNum)
        {
            try
            {
                var existingArea = await _db.Area.GetByAsync(x => x.AreaNum == areaNum);

                if (existingArea is null)
                {
                    return NotFound("Area not found.");
                }

                _db.Area.Remove(existingArea);

                await _db.Area.SaveChangesAsync();

                await _HrLog.UserLog.AddUserLogAsync(1, "userName", 2, $"Delete Area Name = {existingArea.NameAr ?? existingArea.NameEng} have Number = {existingArea.AreaNum}");

                return Ok(true);
            }
            catch (Exception ex)
            {
                ErrorLog error = await _HrLog.ErrorLog.AddErrorLogAsync(
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
