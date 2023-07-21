using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopNews.Core.DTOs.User;

namespace TopNews.Core.Validation.User
{
    public class UpdateUserInfoValidation : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserInfoValidation()
        {
            RuleFor(r => r.FirstName).NotEmpty().MinimumLength(4);
            RuleFor(r => r.LastName).NotEmpty().MinimumLength(4);
            RuleFor(r => r.Email).NotEmpty().EmailAddress();
            RuleFor(r => r.PhoneNumber).NotEmpty();
        }
    }
}
