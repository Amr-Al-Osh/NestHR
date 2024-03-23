using HRService.GeneralDefinitionService.Interfaces;
using HRService.LogHR.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace NestHR.Controllers.GeneralDefinition
{
    public class BranchBankController : Controller
    {
        private IHRDefinitionWrapper _db;

        private IHrLogWarpper _HrLog;
        public BranchBankController(IHRDefinitionWrapper db, IHrLogWarpper HrLog)
        {
            _db = db;
            _HrLog = HrLog;
        }


    }
}
