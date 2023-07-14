using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TopNews.Core.DTOs.User;
using TopNews.Core.Entities.User;
using TopNews.Core.Services;
using TopNews.Core.Validation.User;

namespace TopNews.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserService _userService;
        private readonly SignInManager<AppUser> _signInManager;
        public DashboardController(UserService userService, SignInManager<AppUser> signInManager)
        {
            _userService = userService;
            _signInManager = signInManager;
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

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _userService.SingOutUserAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}
