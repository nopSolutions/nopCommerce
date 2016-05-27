using FluentValidation;
using Nop.Admin.Models.Blogs;
using Nop.Core.Domain.Blogs;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Blogs
{
    public partial class BlogPostValidator : BaseNopValidator<BlogPostModel>
    {
        public BlogPostValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.ContentManagement.Blog.BlogPosts.Fields.Title.Required"));

            RuleFor(x => x.Body)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.ContentManagement.Blog.BlogPosts.Fields.Body.Required"));

            //blog tags should not contain dots
            //current implementation does not support it because it can be handled as file extension
            RuleFor(x => x.Tags)
                .Must(x => x == null || !x.Contains("."))
                .WithMessage(localizationService.GetResource("Admin.ContentManagement.Blog.BlogPosts.Fields.Tags.NoDots"));

            SetStringPropertiesMaxLength<BlogPost>(dbContext);

        }
    }
}