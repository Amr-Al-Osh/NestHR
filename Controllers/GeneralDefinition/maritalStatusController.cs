using HRService.GeneralDefinitionService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NestHR.Controllers.GeneralDefinition
{
    
   
    public class maritalStatusController : Controller
    {
        private IHRDefinitionWrapper _db;

        public maritalStatusController(IHRDefinitionWrapper db)
        {
            _db = db;
        }
    }
}
