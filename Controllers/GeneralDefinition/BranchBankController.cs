using HRService.GeneralDefinitionService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace NestHR.Controllers.GeneralDefinition
{
    public class BranchBankController : Controller
    {
        private IHRDefinitionWrapper _db;

        public BranchBankController(IHRDefinitionWrapper db)
        {
            _db = db;
        }
    }
}
