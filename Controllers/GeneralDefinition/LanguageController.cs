using HRService.GeneralDefinitionService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NestHR.Controllers.GeneralDefinition
{
    
   
    public class LanguageController : Controller
    {
        private IHRDefinitionWrapper _db;

        public LanguageController(IHRDefinitionWrapper db)
        {
            _db = db;
        }
    }
}
