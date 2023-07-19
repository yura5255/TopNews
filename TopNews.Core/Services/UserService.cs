using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IMapper _mapper;
        public UserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
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
        public async Task<ServiceResponse> GetAllAsync()
        {
            List<AppUser> users = await _userManager.Users.ToListAsync();
            List<UserDTO> mappedUsers = users.Select(u => _mapper.Map<AppUser, UserDTO>(u)).ToList();
            for (int i = 0; i < users.Count; i++)
            {
                mappedUsers[i].Role = (await _userManager.GetRolesAsync(users[i])).FirstOrDefault();
            }
            return new ServiceResponse
            {
                Success = true,
                Message = "All users loaded!",
                Payload = mappedUsers,
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
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);
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

        public async Task<ServiceResponse> GetByIdAsync(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "User or password incorrect!"
                };
            }
            var mappedUser = _mapper.Map<AppUser, UpdateUserDto>(user);
            return new ServiceResponse
            {
                Success = true,
                Message = "User succesfully loaded!",
                Payload = mappedUser
            };
        }

        public async Task<ServiceResponse> UpdatePasswordASync(UpdatePasswordDto model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "User or password incorrect!"
                };
            }
            IdentityResult result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if(result.Succeeded)
            {
                await _signInManager.SignOutAsync(); 
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Password succefully updated!"
                };
            }
            List<IdentityError> errorList = result.Errors.ToList();
            string errors = "";
            foreach (var error in errorList)
            {
                errors = error + error.Description.ToString();
            }
            return new ServiceResponse
            {
                Success = false,
                Message = "Error",
                Payload = errors
            };
        }
    }
}
