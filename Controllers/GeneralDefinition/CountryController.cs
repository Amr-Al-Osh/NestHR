using HRService.GeneralDefinitionService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NestHR.Controllers.GeneralDefinition
{
    
   
    public class CountryController : Controller
    {
        private IHRDefinitionWrapper _db;

        public CountryController(IHRDefinitionWrapper db)
        {
            _db = db;
        }
    }
}
