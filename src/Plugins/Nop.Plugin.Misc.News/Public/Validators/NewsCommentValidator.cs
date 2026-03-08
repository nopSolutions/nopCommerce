using FluentValidation;
using Nop.Plugin.Misc.News.Public.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Misc.News.Public.Validators;

/// <summary>
/// Represents news item model validator
/// </summary>
public class NewsCommentValidator : BaseNopValidator<NewsItemModel>
{
    public NewsCommentValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.AddNewComment.CommentTitle)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.News.Comments.CommentTitle.Required"))
            .When(x => x.AddNewComment != null);

        RuleFor(x => x.AddNewComment.CommentTitle)
            .Length(1, 200)
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.News.Comments.CommentTitle.MaxLengthValidation"), 200)
            .When(x => x.AddNewComment != null && !string.IsNullOrEmpty(x.AddNewComment.CommentTitle));

        RuleFor(x => x.AddNewComment.CommentText)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.News.Comments.CommentText.Required"))
            .When(x => x.AddNewComment != null);
    }
}