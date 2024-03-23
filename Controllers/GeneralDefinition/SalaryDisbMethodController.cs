using HRService.GeneralDefinitionService.Interfaces;
using HRService.LogHR.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NestHR.Controllers.GeneralDefinition
{  
    public class SalaryDisbMethodController : Controller
    {
        private IHRDefinitionWrapper _db;
        private IHrLogWarpper _HrLog;
        public SalaryDisbMethodController(IHRDefinitionWrapper db, IHrLogWarpper HrLog)
        {
            _db = db;
            _HrLog = HrLog;
        }



    }
}
