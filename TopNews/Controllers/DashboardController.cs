using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TopNews.Core.DTOs.User;
using TopNews.Core.Entities.User;
using TopNews.Core.Services;
using TopNews.Core.Validation.User;
using TopNews.Web.Models.ViewModel;

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
            var user = HttpContext.User.Identity.IsAuthenticated;
            if (user)
            {
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        [AllowAnonymous] //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(DTOUserLogin model)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
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

        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAllAsync();
            return View(result.Payload);
        }

        public async Task<IActionResult> Profile(string Id)
        {
            var result = await _userService.GetByIdAsync(Id);
            if (result.Success)
            {
                UpdateProfileVM profile = new UpdateProfileVM()
                {
                    UserInfo = (UpdateUserDto) result.Payload
                };
                return View(profile);
            }
            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordDto model)
        {
            var validator = new UpdatePasswordValidation();
            var validationResult = await validator.ValidateAsync(model);
            if(validationResult.IsValid)
            {
                var result = await _userService.UpdatePasswordASync(model);
                if (result.Success)
                {
                    return RedirectToAction(nameof(Login));
                }
                ViewBag.UpdatePasswordError = result.Payload;
                return View("Profile");
            }
            ViewBag.UpdatePasswordError = validationResult.Errors[0];
            return View("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserInfo(UpdateUserDto model)
        {
            var validator = new UpdateUserInfoValidation();
            var validationResult = await validator.ValidateAsync(model);
            if (validationResult.IsValid)
            {
                var result = await _userService.UpdateUserInfoAsync(model);
                if (result.Success)
                {
                    return RedirectToAction(nameof(Login));
                }
                ViewBag.UpdateUserError = result.Payload;
                return View("Profile");
            }
            ViewBag.UpdateUserError = validationResult.Errors[0];
            return View("Profile");
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserDto model)
        {
            var validator = new CreateUserValidation();
            var validationResult = await validator.ValidateAsync(model);
            if (validationResult.IsValid)
            {
                var result = await _userService.CreateNewUserAsync(model);
                if (result.Success)
                {
                    return RedirectToAction(nameof(GetAll));
                }
                ViewBag.UpdatePasswordError = result.Payload;
                return View(model);
            }
            ViewBag.UpdatePasswordError = validationResult.Errors[0];
            return View();
        }

        public async Task<IActionResult> Delete(string Id)
        {
            ServiceResponse response = await _userService.DeleteUserAsync(Id);
            if (response.Success)
            {
                return RedirectToAction(nameof(GetAll));
            }
            ViewBag.CreateUserError = response.Errors.Count() > 0 ? ((IdentityError)response.Errors.First()).Description : response.Message;
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var result = await _userService.ConfirmEmailAsync(userId, token);
            if (result.Success)
            {
                return Redirect(nameof(Login));
            }
            return Redirect(nameof(Login));
        }

        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var result = await _userService.ForgotPasswordAsync(email);
            if (result.Success)
            {
                ViewBag.AuthError = "Check your email.";
                return View(nameof(Login));
            }
            ViewBag.AuthError = "Something went wrong.";
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            ViewBag.Email = email;
            ViewBag.Token = token;
            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            var result = await _userService.ResetPasswordAsync(model);
            ViewBag.Message = result.Message;
            return View(nameof(Login));
        }

        public async Task<IActionResult> Edit(string id)
        {

            var result = await _userService.GetUserByIdAsync(id);
            if (result.Success)
            {
                await LoadRoles();
                return View(result.Payload);
            }

            return View(nameof(Index));
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Edit(UpdateUserDto model)
        {
            var validator = new UpdateUserValidation();
            var validationResult = await validator.ValidateAsync(model);
            if (validationResult.IsValid)
            {
                var result = await _userService.EditAsync(model);
                if (result.Success)
                {
                    return View(nameof(Index));
                }
                return View(nameof(Index));
            }

            await LoadRoles();
            ViewBag.AuthError = validationResult.Errors[0];
            return View(nameof(Edit));
        }

        private async Task LoadRoles()
        {
            var result = await _userService.GetAllRolesAsync();
            @ViewBag.RoleList = new SelectList(
          (System.Collections.IEnumerable)result, nameof(IdentityRole.Name),
              nameof(IdentityRole.Name)
              );
        }

    }
}
