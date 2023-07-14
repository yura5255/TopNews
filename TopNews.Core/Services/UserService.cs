﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopNews.Core.DTOs.User;
using TopNews.Core.Entities.User;

namespace TopNews.Core.Services
{
    public class UserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public UserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<ServiceResponse> SingOutUserAsync()
        {
            await _signInManager.SignOutAsync();
            return new ServiceResponse
            {
                Success = true,
                Message = "Singed out successfully"
            };
        }
        public async Task<ServiceResponse> LoginUserAsync(DTOUserLogin model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "User or password incorrect!"
                };
            }
            else
            {
                SignInResult result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);
                if(result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, model.RememberMe);
                    return new ServiceResponse
                    {
                        Success = true,
                        Message = "User succesfully logged in!"
                    };
                }
                if(result.IsNotAllowed)
                {
                    await _signInManager.SignInAsync(user, model.RememberMe);
                    return new ServiceResponse
                    {
                        Success = false,
                        Message = "Confirm your email please!"
                    };
                }
                if(result.IsLockedOut)
                {
                    await _signInManager.SignInAsync(user, model.RememberMe);
                    return new ServiceResponse
                    {
                        Success = false,
                        Message = "User is locked. Connect with your site administator!"
                    };
                }
                return new ServiceResponse
                {
                    Success = false,
                    Message = "User or password incorrect!"
                };
            }
        }
    }
}
