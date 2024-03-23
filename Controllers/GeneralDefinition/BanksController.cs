using Domin.Models;
using HRService;
using HRService.GeneralDefinitionService.Interfaces;
using HRService.LogHR.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NestHR.Models.GeneralDefinition;
using System.Linq.Expressions;

namespace NestHR.Controllers.GeneralDefinition
{
    public class BanksController : Controller
    {
        private IHRDefinitionWrapper _db;
        private IHrLogWarpper _HrLog;
        public BanksController(IHRDefinitionWrapper db, IHrLogWarpper HrLog)
        {
            _db = db;
            _HrLog = HrLog;
        }

        [Route("Banks")]
        public IActionResult BanksPage()
        {
            return View();
        }

        [HttpGet("GetAllBanks")]
        public async Task<IActionResult> GetAllBanks()=>        
             Ok(await _db.Banks.ReadAllAsync());
        
        [HttpPost("GetDataTableBanks")]
        public async Task<IActionResult> GetDataTableBanks()
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

            var data = await _db.Banks.ReadAllAsync();

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
                var property = typeof(Bank).GetProperty(sortColumnName);
                if (property != null)
                {
                    var orderByExpression = Expression.Lambda<Func<Bank, object>>(
                        Expression.Convert(
                            Expression.Property(
                                Expression.Parameter(typeof(Bank), "x"),
                                property),
                            typeof(object)),
                        Expression.Parameter(typeof(Bank), "x"));

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

        [HttpPost("AddNewBanks")]
        public async Task<IActionResult> AddNewBanks([FromBody] Bank model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingBank = await _db.Banks.GetByAsync(x=>x.Value == model.Value);

                    if (existingBank != null)
                    {
                        return BadRequest("Banks already exists.");
                    }

                    model.Value = await _db.Banks.GetMaxAsync(x=>x.Value);
                    _db.Banks.Add(model);

                    await _db.Banks.SaveChangesAsync();

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

        [HttpPost("EditeBanks")]
        public async Task<IActionResult> EditeBanks([FromBody] BanksModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingBank = await _db.Banks.GetByAsync(x => x.Value == model.Value);

                    if (existingBank != null)
                    {
                        existingBank.NameEng = model.NameEng ?? "";
                        existingBank.NameAr = model.NameAr ?? "";
                        _db.Banks.Update(existingBank);

                        await _db.Banks.SaveChangesAsync();

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

        [HttpPost("DealetBanks")]
        public async Task<IActionResult> DealetBanks(int value)
        {
            try
            {
                var existingBank = await _db.Banks.GetByAsync(x => x.Value == value);

                if (existingBank != null)
                {
                    _db.Banks.Remove(existingBank);
                    await _db.Banks.SaveChangesAsync();
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
