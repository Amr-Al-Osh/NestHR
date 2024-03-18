using HRService.GeneralDefinitionService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NestHR.Controllers.GeneralDefinition
{
    
   
    public class EmailTypeController : Controller
    {
        private IHRDefinitionWrapper _db;

        public EmailTypeController(IHRDefinitionWrapper db)
        {
            _db = db;
        }








    }
}
