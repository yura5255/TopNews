using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopNews.Core.DTOs.User;
using TopNews.Core.Services;
using TopNews.Core.Validation.User;

namespace TopNews.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserService _userService;
        public DashboardController(UserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous] //GET
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous] //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(DTOUserLogin model)
        {
            var validator = new LoginUserValidation();
            var validationResult = validator.Validate(model);
            if (validationResult.IsValid) 
            {
                ServiceResponse result = await _userService.LoginUserAsync(model);
                if (result.Success)
                {
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.AuthError = result.Message;
                return View(model);
            }
            ViewBag.AuthError = validationResult.Errors[0];
            return View(model);
        }
    }
}
