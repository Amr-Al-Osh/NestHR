using Domin.Models;
using HRService.GeneralDefinitionService.Interfaces;
using HRService.LogHR.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NestHR.Models.GeneralDefinition;
using System.Linq.Expressions;

namespace NestHR.Controllers.GeneralDefinition
{
    [Route("Banks")]
    public class BanksController : Controller
    {
        private IHRDefinitionWrapper _db;
        private IHrLogWarpper _HrLog;
        public BanksController(IHRDefinitionWrapper db, IHrLogWarpper HrLog)
        {
            _db = db;
            _HrLog = HrLog;
        }

     
        public IActionResult BanksPage()
        {
            return View();
        }



    }
}
