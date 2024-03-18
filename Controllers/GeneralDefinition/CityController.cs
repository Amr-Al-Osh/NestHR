using HRService.GeneralDefinitionService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NestHR.Controllers.GeneralDefinition
{   
   
    public class CityController : Controller
    {
        private IHRDefinitionWrapper _db;

        public CityController(IHRDefinitionWrapper db)
        {
            _db = db;
        }
    }
}
