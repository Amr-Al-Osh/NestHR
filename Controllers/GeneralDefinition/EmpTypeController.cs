using HRService.GeneralDefinitionService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NestHR.Controllers.GeneralDefinition
{
    
   
    public class EmpTypeController : Controller
    {
        private IHRDefinitionWrapper _db;

        public EmpTypeController(IHRDefinitionWrapper db)
        {
            _db = db;
        }
    }
}
