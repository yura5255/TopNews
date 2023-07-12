using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopNews.Core.DTOs.User;

namespace TopNews.Core.Validation.User
{
    public class LoginUserValidation : AbstractValidator<DTOUserLogin>
    {
        public LoginUserValidation()
        {
            RuleFor(r => r.Email).NotEmpty().WithMessage("Field must not be empty!").EmailAddress().WithMessage("Invalid email format!");
            RuleFor(r => r.Password).NotEmpty().WithMessage("Field must not be empty!").MinimumLength(6).WithMessage("Password must at least 6 characters!").MaximumLength(128);
        }
    }
}
