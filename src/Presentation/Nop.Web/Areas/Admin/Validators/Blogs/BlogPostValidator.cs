using FluentValidation;
using Nop.Core.Domain.Blogs;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Models.Blogs;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Blogs;

public partial class BlogPostValidator : BaseNopValidator<BlogPostModel>
{
    public BlogPostValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Admin.ContentManagement.Blog.BlogPosts.Fields.Title.Required");

        RuleFor(x => x.Body)
            .NotEmpty()
            .WithMessage("Admin.ContentManagement.Blog.BlogPosts.Fields.Body.Required");

        //blog tags should not contain dots
        //current implementation does not support it because it can be handled as file extension
        RuleFor(x => x.Tags)
            .Must(x => x == null || !x.Contains('.'))
            .WithMessage("Admin.ContentManagement.Blog.BlogPosts.Fields.Tags.NoDots");

        RuleFor(x => x.SeName).Length(0, NopSeoDefaults.SearchEngineNameLength)
            .WithMessage(string.Format("Admin.SEO.SeName.MaxLengthValidation", NopSeoDefaults.SearchEngineNameLength));

        SetDatabaseValidationRules<BlogPost>();
    }
}