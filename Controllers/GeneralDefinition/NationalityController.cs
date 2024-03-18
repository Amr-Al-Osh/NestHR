using HRService.GeneralDefinitionService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NestHR.Controllers.GeneralDefinition
{
    
   
    public class NationalityController : Controller
    {
        private IHRDefinitionWrapper _db;

        public NationalityController(IHRDefinitionWrapper db)
        {
            _db = db;
        }
    }
}
