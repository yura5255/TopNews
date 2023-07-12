﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopNews.Core.DTOs.User;
using TopNews.Core.Validation.User;

namespace TopNews.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
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
        public IActionResult Login(DTOUserLogin model)
        {
            var validator = new LoginUserValidation();
            var validationResult = validator.Validate(model);
            if (validationResult.IsValid) 
            {
                return View(model);
            }
            ViewBag.AuthError = validationResult.Errors[0];
            return View(model);
        }
    }
}
