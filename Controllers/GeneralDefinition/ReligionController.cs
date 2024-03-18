using HRService.GeneralDefinitionService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NestHR.Controllers.GeneralDefinition
{
    
   
    public class ReligionController : Controller
    {
        private IHRDefinitionWrapper _db;

        public ReligionController(IHRDefinitionWrapper db)
        {
            _db = db;
        }
    }
}
