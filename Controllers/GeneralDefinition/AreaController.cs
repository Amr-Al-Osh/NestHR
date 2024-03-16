﻿using Domin.Models;
using HRService;
using HRService.GeneralDefinition.Interfaces;
using HRService.GeneralDefinitionService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NestHR.BusinessHR.GeneralDefinition;
using System.Linq.Expressions;

namespace NestHR.Controllers.GeneralDefinition
{
    public class AreaController : Controller
    {
        private IRepositoryWrapper _db;

        public AreaController(IRepositoryWrapper db)
        {
            _db = db;
        }

        [Route("Area")]
        public async Task<IActionResult> Area()
        {
          
            var data = await _db.Areas.ReadAllAsync();
            return View(data);
        }

        [HttpGet("GetAllArea")]
        public async Task<IActionResult> GetAllArea()
        {
            return Ok(await _db.Areas.ReadAllAsync());
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

            var data = await _db.Areas.ReadAllAsync();

            totalRecord = data.Count();

            if (!string.IsNullOrEmpty(searchValue))
            {
                data = data.Where(x =>
                    x.Id.ToString().Contains(searchValue) ||
                    x.Value.ToString().Contains(searchValue) ||
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

        [HttpPost("NewArea")]
        public async Task<IActionResult> NewArea([FromBody] Area model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingArea = await _db.Areas.GetByAsync(x => x.Value == model.Value);

                    if (existingArea != null)
                    {
                        return BadRequest("Area already exists.");
                    }

                    model.Value = await _db.Areas.GetMaxAsync(x => x.Value);

                    _db.Areas.Add(model);

                    await _db.Areas.SaveChangesAsync();

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
                    var existingArea = await _db.Areas.GetByAsync(x => x.Value == model.Value);

                    if (existingArea is null)
                    {
                        return NotFound("Area not found.");
                    }
                    else
                    {
                        existingArea.NameEng = model.NameEng ?? "";
                        existingArea.NameAr = model.NameAr ?? "";

                        _db.Areas.Update(existingArea);

                        await _db.Areas.SaveChangesAsync();

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
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("DealetArea")]
        public async Task<IActionResult> DealetArea(int value)
        {
            try
            {
                var existingArea = await _db.Areas.GetByAsync(x => x.Value == value);

                if (existingArea != null)
                {
                    _db.Areas.Remove(existingArea);
                    await _db.Areas.SaveChangesAsync();
                }

                return NotFound("Area not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }
}
