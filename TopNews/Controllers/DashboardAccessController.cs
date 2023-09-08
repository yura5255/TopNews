using Microsoft.AspNetCore.Mvc;
using TopNews.Core.DTOs.Ip;
using TopNews.Core.Interfaces;
using TopNews.Core.Validation.Ip;

namespace TopNews.Web.Controllers
{
    public class DashboardAccessController : Controller
    {
        private readonly IDashboardAccessService _ipService;
        public DashboardAccessController(IDashboardAccessService dashdoardAccessService)
        {
            _ipService = dashdoardAccessService;
        }
        public IActionResult Index()
        {
            return RedirectToAction(nameof(GetAll));
        }

        #region Get All page
        public async Task<IActionResult> GetAll()
        {
            List<DashboardAccessDto> ips = await _ipService.GetAll();
            return View(ips);
        }
        #endregion

        #region Create page
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DashboardAccessDto model)
        {
            var validationResult = await new CreateDashboardAccessValidation().ValidateAsync(model);
            if (validationResult.IsValid)
            {
                DashboardAccessDto? result = await _ipService.Get(model.IpAddress);
                if (result == null)
                {
                    ViewBag.AuthError = "DashdoardAccesses exists.";
                    return View(model);
                }
                _ipService.Create(model);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.AuthError = validationResult.Errors.FirstOrDefault();
            return View(model);
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int id)
        {
            DashboardAccessDto? model = await _ipService.Get(id);
            if (model == null)
            {
                ViewBag.AuthError = "Category not found.";
                return RedirectToAction(nameof(GetAll));
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteById(int Id)
        {
            _ipService.Delete(Id);
            return RedirectToAction(nameof(GetAll));
        }
        #endregion
    }
}
