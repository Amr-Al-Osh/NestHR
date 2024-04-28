using HRService.Pages.Interfaces;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using NestHR.LanguageSupport;
using NestHR.Models;
using System.Diagnostics;

namespace NestHR.Controllers
{
    public class HomeController : HRBaseController
    {
        private readonly IGetPagesWarpper _db;
        public HomeController(
            IGetPagesWarpper db,
            LanguageService localization,
            IConfiguration config,
            IHttpContextAccessor httpContextAccessor) : base(localization, config, httpContextAccessor)
        {
            _db = db;
        }

        public IActionResult MainPage()
        {
            return View();
        }

        public async Task<IActionResult> GetGroupPages()
        {
            try
            {
                IQueryable<Domin.Models.GroupPage> groupPages = await _db.GroupPages.ReadAsync();
                IQueryable<Domin.Models.Page> pages = await _db.Pages.ReadAsync();

                IQueryable<PageModel> pagesData = pages
                    .Where(page => page.IsShow ?? false)
                    .OrderBy(page => page.OrderPage)
                    .Select(page => new PageModel
                    {
                        PageNameAr = page.PageNameAr,
                        Icon = page.Icon,
                        Url = page.Url,
                        PageGroup = page.PageGroup
                    }).AsQueryable();

                List<GroupPagesModel> sidebarData = (
                    from _group in groupPages
                    where _group.IsShow ?? false
                    orderby _group.OrderGroup
                    let _pages = pagesData.Where(page => _group.GroupNum == page.PageGroup).ToList()
                    select new GroupPagesModel
                    {
                        GroupNum = _group.GroupNum,
                        GroupNameAr = _group.GroupNameAr,
                        GroupNameEng = _group.GroupNameEng,
                        MasterGroup = _group.MasterGroup,
                        OrderGroup = _group.OrderGroup,
                        PagesList = _pages
                    }).AsEnumerable().ToList();

                return Ok(sidebarData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        #region Localization
        public IActionResult ChangeLanguage(string culture)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions() { Expires = DateTimeOffset.UtcNow.AddYears(1) });

            return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
