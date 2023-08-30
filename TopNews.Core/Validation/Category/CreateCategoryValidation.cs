using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopNews.Core.DTOs.Category;

namespace TopNews.Core.Validation.Category
{
    public class CreateCategoryValidation : AbstractValidator<CategoryDto>
    {
        public CreateCategoryValidation()
        {
            RuleFor(r => r.Name).NotEmpty();
        }
    }
}
