using Domin.Models;
using HRService.GeneralDefinition.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NestHR.Controllers.GeneralDefinition
{
    [Authorize]
    public class AreaController : Controller
    {
        private readonly IAreaService _areaService;

        public AreaController(IAreaService areaService)
        {
            _areaService = areaService;
        }

        [Route("Area")]
        public async Task<IActionResult> Area()
        {
            var data = await _areaService.ReadAll();
            return View(data);
        }

        [HttpPost("GetDataTable")]
        public async Task<IActionResult> GetDataTable()
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

            var data = await _areaService.ReadAll();

            totalRecord = data.Count();

            if (!string.IsNullOrEmpty(searchValue))
            {
                data = data.Where(x =>
                    x.Id.ToString().Contains(searchValue) ||
                    x.Value.ToString().Contains(searchValue) ||
                    (x.NameAr ?? "").Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                    (x.NameEng ?? "").Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            filterRecord = data.Count();
            //sort data
            if (!string.IsNullOrEmpty(sortColumnName) && !string.IsNullOrEmpty(sortDirection))
            {
                data = (List<Area>)(sortDirection.Equals("desc", StringComparison.CurrentCultureIgnoreCase) ?
                    data.OrderByDescending(x => x.GetType().GetProperty(sortColumnName)?.GetValue(x, null)) :
                    data.OrderBy(x => x.GetType().GetProperty(sortColumnName)?.GetValue(x, null)));
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

        [HttpPost("NewArea")]
        public async Task<IActionResult> NewArea([FromBody] Area model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingArea = await _areaService.GetBy(model.Value);

                    if (existingArea != null)
                    {
                        return BadRequest("Area already exists.");
                    }

                    model.Value = await _areaService.GetMax();

                    await _areaService.Add(model);

                    return Ok(true);
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
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
                    var existingArea = await _areaService.GetBy(model.Value);

                    if (existingArea != null)
                    {
                        existingArea.NameEng = model.NameEng ?? "";
                        existingArea.NameAr = model.NameAr ?? "";

                        await _areaService.Update(existingArea);

                        return Ok(true);
                    }
                    else
                    {
                        return NotFound("Area not found.");
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("DealetArea")]
        public async Task<IActionResult> DealetArea(int value)
        {
            try
            {
                var existingArea = await _areaService.GetBy(value);

                if (existingArea != null)
                {
                    await _areaService.Remove(existingArea);
                }

                return NotFound("Area not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetAllArea")]
        public async Task<IActionResult> GetAllArea()
        {
            return Ok(await _areaService.GetAll());
        }


    }
}
