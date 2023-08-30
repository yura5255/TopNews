using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopNews.Core.DTOs.Post;

namespace TopNews.Core.Validation.Post
{
    public class CreatePostValidation : AbstractValidator<PostDto>
    {
        public CreatePostValidation()
        {
            RuleFor(r => r.Title).NotEmpty();
            RuleFor(r => r.Description).NotEmpty();
            RuleFor(r => r.FullText).NotEmpty();
            RuleFor(r => r.CategoryId).NotEmpty();
        }
    }
}
