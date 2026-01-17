using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Validators.Catalog;

public partial class ProductReviewsValidator : BaseNopValidator<ProductReviewsModel>
{
    public ProductReviewsValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.AddProductReview.Title).NotEmpty().WithMessage("Reviews.Fields.Title.Required").When(x => x.AddProductReview != null);
        RuleFor(x => x.AddProductReview.Title).Length(1, 200).WithMessage(string.Format("Reviews.Fields.Title.MaxLengthValidation", 200)).When(x => x.AddProductReview != null && !string.IsNullOrEmpty(x.AddProductReview.Title));
        RuleFor(x => x.AddProductReview.ReviewText).NotEmpty().WithMessage("Reviews.Fields.ReviewText.Required").When(x => x.AddProductReview != null);
    }
}