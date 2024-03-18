using HRService.GeneralDefinitionService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NestHR.Controllers.GeneralDefinition
{
    
   
    public class SalaryDisbMethodController : Controller
    {
        private IHRDefinitionWrapper _db;

        public SalaryDisbMethodController(IHRDefinitionWrapper db)
        {
            _db = db;
        }
    }
}
