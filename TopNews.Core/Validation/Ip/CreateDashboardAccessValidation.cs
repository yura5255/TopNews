using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopNews.Core.DTOs.Ip;

namespace TopNews.Core.Validation.Ip
{
    public class CreateDashboardAccessValidation : AbstractValidator<DashboardAccessDto>
    {
        public CreateDashboardAccessValidation() 
        {
            RuleFor(da => da.IpAddress).NotEmpty();
        }
    }
}
