using HRService.GeneralDefinitionService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NestHR.Controllers.GeneralDefinition
{
    
   
    public class IdentifierTypController : Controller
    {
        private IHRDefinitionWrapper _db;

        public IdentifierTypController(IHRDefinitionWrapper db)
        {
            _db = db;
        }
    }
}
