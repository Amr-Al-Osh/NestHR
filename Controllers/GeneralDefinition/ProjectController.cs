using HRService.GeneralDefinitionService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NestHR.Controllers.GeneralDefinition
{
    
   
    public class ProjectController : Controller
    {
        private IHRDefinitionWrapper _db;

        public ProjectController(IHRDefinitionWrapper db)
        {
            _db = db;
        }
    }
}
