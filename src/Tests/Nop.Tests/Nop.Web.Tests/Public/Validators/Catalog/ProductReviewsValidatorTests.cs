using FluentValidation.TestHelper;
using Nop.Web.Models.Catalog;
using Nop.Web.Validators.Catalog;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Catalog
{
    [TestFixture]
    public class ProductReviewsValidatorTests : BaseNopTest
    {
        private ProductReviewsValidator _validator;

        [OneTimeSetUp]
        public void Setup()
        {
            _validator = GetService<ProductReviewsValidator>();
        }

        [Test]
        public void ShouldHaveErrorWhenTitleIsNullOrEmpty()
        {
            var model = new ProductReviewsModel { AddProductReview = { Title = null } };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AddProductReview.Title);
            model.AddProductReview.Title = string.Empty;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AddProductReview.Title);
        }

        [Test]
        public void ShouldNotHaveErrorWhenTitleIsSpecified()
        {
            var model = new ProductReviewsModel { AddProductReview = { Title = "some comment" } };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AddProductReview.Title);
        }

        [Test]
        public void ShouldHaveErrorWhenReviewTextIsNullOrEmpty()
        {
            var model = new ProductReviewsModel { AddProductReview = { ReviewText = null } };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AddProductReview.ReviewText);
            model.AddProductReview.ReviewText = string.Empty;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AddProductReview.ReviewText);
        }

        [Test]
        public void ShouldNotHaveErrorWhenReviewTextIsSpecified()
        {
            var model = new ProductReviewsModel { AddProductReview = { ReviewText = "some comment" } };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AddProductReview.ReviewText);
        }
    }
}
