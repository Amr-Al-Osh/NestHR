using Domin.Models;
using HRService.GeneralDefinitionService.Interfaces;
using HRService.LogHR.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NestHR.Controllers.GeneralDefinition
{   
   
    public class CityController : Controller
    {
        private IHRDefinitionWrapper _db;
        private IHrLogWarpper _HrLog;
        public CityController(IHRDefinitionWrapper db, IHrLogWarpper HrLog)
        {
            _db = db;
            _HrLog = HrLog;
        }







    }
}
