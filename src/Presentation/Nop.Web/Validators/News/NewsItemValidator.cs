using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.News;

namespace Nop.Web.Validators.News
{
    public partial class NewsItemValidator : BaseNopValidator<NewsItemModel>
    {
        public NewsItemValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.AddNewComment.CommentTitle).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("News.Comments.CommentTitle.Required")).When(x => x.AddNewComment != null);
            RuleFor(x => x.AddNewComment.CommentTitle).Length(1, 200).WithMessageAwait(localizationService.GetResourceAsync("News.Comments.CommentTitle.MaxLengthValidation"), 200).When(x => x.AddNewComment != null && !string.IsNullOrEmpty(x.AddNewComment.CommentTitle));
            RuleFor(x => x.AddNewComment.CommentText).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("News.Comments.CommentText.Required")).When(x => x.AddNewComment != null);
        }
    }
}