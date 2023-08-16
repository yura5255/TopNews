using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        public UserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMapper mapper, EmailService emailService, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _emailService = emailService;
            _configuration = configuration;
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

        public async Task<ServiceResponse> UpdateUserInfoAsync(UpdateUserDto model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Something gone wrong!"
                };
            }
            IdentityResult result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync();
                return new ServiceResponse
                {
                    Success = true,
                    Message = "User succesfully updated!"
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

        public async Task<ServiceResponse> CreateNewUserAsync(CreateUserDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "User already exists!"
                };
            }
            AppUser mappedUser = _mapper.Map<CreateUserDto, AppUser>(model);
            IdentityResult result = await _userManager.CreateAsync(mappedUser, model.Password);
            if (result.Succeeded)
            {
                _userManager.AddToRoleAsync(mappedUser, model.Role).Wait();
                //await _emailService.SendEmail(model.Email, "Hello!", "Hello World!");
                await SendConfirmationEmailAsync(mappedUser);
                return new ServiceResponse
                {
                    Success = true,
                    Message = "User succesfully created!"
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

        public async Task<ServiceResponse> DeleteUserAsync(string Id)
        {
            AppUser userdelete = await _userManager.FindByIdAsync(Id);
            if (userdelete != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(userdelete);
                if (result.Succeeded)
                {
                    return new ServiceResponse
                    {
                        Success = true
                    };
                }
                else
                {
                    return new ServiceResponse
                    {
                        Success = false,
                        Message = "something went wrong",
                    };
                }
            }
            return new ServiceResponse
            {
                Success = false,
                Message = "User a was found" 
            };
        }

        public async Task SendConfirmationEmailAsync(AppUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validEmailToken = WebEncoders.Base64UrlEncode(encodedToken);
            string url = $"{_configuration["HostSettings:URL"]}/Dashboard/ConfirmEmail?userId={user.Id}&token={validEmailToken}";
            string emailBody = $"<h1>Confirm your email please!</h1><a href='{url}'>Confirm now</a>";
            await _emailService.SendEmail(user.Email, "Email confirmation!", emailBody);
        }

        public async Task<ServiceResponse> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "User not found!"
                };
            }
            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);
            var result = await _userManager.ConfirmEmailAsync(user, normalToken);
            if(result.Succeeded)
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Email succesfully confirmed!"
                };
            }
            return new ServiceResponse
            {
                Success = false,
                Message = "Email not confirmed!",
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<ServiceResponse> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ServiceResponse
                {
                    Message = "User exists.",
                    Success = false,
                };
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validEmailToken = WebEncoders.Base64UrlEncode(encodedToken);

            var url = $"{_configuration["HostSettings:URL"]}/Dashboard/ResetPassword?email={email}&token={validEmailToken}";

            string emailBody = $"<h1>Follow the instruction for reset password.</h1><a href='{url}'>Reset now!</a>";
            await _emailService.SendEmail(email, "Reset password for TopNews.", emailBody);

            return new ServiceResponse
            {
                Success = true,
                Message = "Email successfully sent."
            };
        }

        public async Task<ServiceResponse> ResetPasswordAsync(ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new ServiceResponse
                {
                    Message = "Error.",
                    Success = false,
                };
            }

            var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);
            var result = await _userManager.ResetPasswordAsync(user, normalToken, model.Password);

            return new ServiceResponse
            {
                Success = true,
                Message = "Password succesfully reseted."
            };
        }
    }
}
