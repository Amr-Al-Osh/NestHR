using HRService.GeneralDefinitionService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NestHR.Controllers.GeneralDefinition
{
    
   
    public class PhoneCountryCodeController : Controller
    {
        private IHRDefinitionWrapper _db;

        public PhoneCountryCodeController(IHRDefinitionWrapper db)
        {
            _db = db;
        }
    }
}
