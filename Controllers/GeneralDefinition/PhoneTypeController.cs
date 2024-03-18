using HRService.GeneralDefinitionService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NestHR.Controllers.GeneralDefinition
{
    
   
    public class PhoneTypeController : Controller
    {
        private IHRDefinitionWrapper _db;

        public PhoneTypeController(IHRDefinitionWrapper db)
        {
            _db = db;
        }
    }
}
