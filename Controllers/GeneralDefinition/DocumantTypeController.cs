using HRService.GeneralDefinitionService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NestHR.Controllers.GeneralDefinition
{
    
   
    public class DocumantTypeController : Controller
    {
        private IHRDefinitionWrapper _db;

        public DocumantTypeController(IHRDefinitionWrapper db)
        {
            _db = db;
        }
    }
}
