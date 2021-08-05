using FluentValidation;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Data.Mapping;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Catalog
{
    public partial class ProductReviewValidator : BaseNopValidator<ProductReviewModel>
    {
        public ProductReviewValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor, IWorkContext workContext)
        {
            var isLoggedInAsVendor = workContext.GetCurrentVendorAsync().Result != null;
            //vendor can edit "Reply text" only
            if (!isLoggedInAsVendor)
            {
                RuleFor(x => x.Title).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.ProductReviews.Fields.Title.Required"));
                RuleFor(x => x.ReviewText).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.ProductReviews.Fields.ReviewText.Required"));
            }

            SetDatabaseValidationRules<ProductReview>(mappingEntityAccessor);
        }
    }
}