using FluentValidation.TestHelper;
using Nop.Core.Domain.Vendors;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Validators.Catalog;
using Nop.Web.MVC.Tests.Public.Validators;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Web.MVC.Tests.Admin.Validators.Catalog
{
    public class ProductReviewValidatorTests : BaseValidatorTests
    {
        private ProductReviewValidator _validator;

        [Test]
        public void Should_have_error_when_title_is_empty_and_user_is_not_logged_in_as_vendor()
        {
            _workContext.Stub(x => x.CurrentVendor).Return(null);
            _validator = new ProductReviewValidator(_localizationService, null, _workContext);

            var model = new ProductReviewModel { Title = string.Empty };
            _validator.ShouldHaveValidationErrorFor(x => x.Title, model);
        }

        [Test]
        public void Should_not_have_error_when_title_is_not_empty_and_user_is_not_logged_in_as_vendor()
        {
            _workContext.Stub(x => x.CurrentVendor).Return(null);
            _validator = new ProductReviewValidator(_localizationService, null, _workContext);

            var model = new ProductReviewModel { Title = "Title" };
            _validator.ShouldNotHaveValidationErrorFor(x => x.Title, model);
        }

        [Test]
        public void Should_not_have_error_when_title_is_empty_but_user_is_logged_in_as_vendor()
        {
            _workContext.Stub(x => x.CurrentVendor).Return(new Vendor());
            _validator = new ProductReviewValidator(_localizationService, null, _workContext);

            var model = new ProductReviewModel { Title = string.Empty };
            _validator.ShouldNotHaveValidationErrorFor(x => x.Title, model);
        }

        [Test]
        public void Should_not_have_error_when_name_is_not_empty_but_user_is_logged_in_as_vendor()
        {
            _workContext.Stub(x => x.CurrentVendor).Return(new Vendor());
            _validator = new ProductReviewValidator(_localizationService, null, _workContext);

            var model = new ProductReviewModel { Title = "Title" };
            _validator.ShouldNotHaveValidationErrorFor(x => x.Title, model);
        }

        [Test]
        public void Should_have_error_when_review_text_is_empty_and_user_is_not_logged_in_as_vendor()
        {
            _workContext.Stub(x => x.CurrentVendor).Return(null);
            _validator = new ProductReviewValidator(_localizationService, null, _workContext);

            var model = new ProductReviewModel { ReviewText = string.Empty };
            _validator.ShouldHaveValidationErrorFor(x => x.ReviewText, model);
        }

        [Test]
        public void Should_not_have_error_when_review_text_is_not_empty_and_user_is_not_logged_in_as_vendor()
        {
            _workContext.Stub(x => x.CurrentVendor).Return(null);
            _validator = new ProductReviewValidator(_localizationService, null, _workContext);

            var model = new ProductReviewModel { ReviewText = "ReviewText" };
            _validator.ShouldNotHaveValidationErrorFor(x => x.ReviewText, model);
        }

        [Test]
        public void Should_not_have_error_when_review_text_is_empty_but_user_is_logged_in_as_vendor()
        {
            _workContext.Stub(x => x.CurrentVendor).Return(new Vendor());
            _validator = new ProductReviewValidator(_localizationService, null, _workContext);

            var model = new ProductReviewModel {ReviewText = string.Empty};
            _validator.ShouldNotHaveValidationErrorFor(x => x.ReviewText, model);
        }

        [Test]
        public void Should_not_have_error_when_review_text_is_not_empty_but_user_is_logged_in_as_vendor()
        {
            _workContext.Stub(x => x.CurrentVendor).Return(new Vendor());
            _validator = new ProductReviewValidator(_localizationService, null, _workContext);

            var model = new ProductReviewModel {ReviewText = "ReviewText" };
            _validator.ShouldNotHaveValidationErrorFor(x => x.ReviewText, model);
        }
    }
}