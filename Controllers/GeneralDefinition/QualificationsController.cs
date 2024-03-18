using HRService.GeneralDefinitionService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NestHR.Controllers.GeneralDefinition
{
    
   
    public class QualificationsController : Controller
    {
        private IHRDefinitionWrapper _db;

        public QualificationsController(IHRDefinitionWrapper db)
        {
            _db = db;
        }
    }
}
