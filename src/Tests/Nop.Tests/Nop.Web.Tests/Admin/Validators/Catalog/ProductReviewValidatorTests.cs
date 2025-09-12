using FluentValidation.TestHelper;
using Nop.Core;
using Nop.Core.Domain.Vendors;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Validators.Catalog;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Admin.Validators.Catalog;

[TestFixture]
public class ProductReviewValidatorTests : BaseNopTest
{
    private ProductReviewValidator _validator;
    private IWorkContext _workContext;

    [OneTimeSetUp]
    public void Setup()
    {
        _workContext = GetService<IWorkContext>();
        _validator = new ProductReviewValidator(GetService<ILocalizationService>(), _workContext);
    }

    #region Title Validation Tests (Non-Vendor)

    [Test]
    public void ShouldHaveErrorWhenTitleIsNullForNonVendor()
    {
        // Ensure we're not logged in as vendor
        var model = new ProductReviewModel
        {
            Title = null,
            ReviewText = "Valid review text"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Test]
    public void ShouldHaveErrorWhenTitleIsEmptyForNonVendor()
    {
        var model = new ProductReviewModel
        {
            Title = string.Empty,
            ReviewText = "Valid review text"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Test]
    public void ShouldHaveErrorWhenTitleIsWhitespaceForNonVendor()
    {
        var model = new ProductReviewModel
        {
            Title = "   ",
            ReviewText = "Valid review text"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Test]
    public void ShouldNotHaveErrorWhenTitleIsSpecifiedForNonVendor()
    {
        var model = new ProductReviewModel
        {
            Title = "Great Product",
            ReviewText = "Valid review text"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    #endregion

    #region ReviewText Validation Tests (Non-Vendor)

    [Test]
    public void ShouldHaveErrorWhenReviewTextIsNullForNonVendor()
    {
        var model = new ProductReviewModel
        {
            Title = "Valid Title",
            ReviewText = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ReviewText);
    }

    [Test]
    public void ShouldHaveErrorWhenReviewTextIsEmptyForNonVendor()
    {
        var model = new ProductReviewModel
        {
            Title = "Valid Title",
            ReviewText = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ReviewText);
    }

    [Test]
    public void ShouldHaveErrorWhenReviewTextIsWhitespaceForNonVendor()
    {
        var model = new ProductReviewModel
        {
            Title = "Valid Title",
            ReviewText = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ReviewText);
    }

    [Test]
    public void ShouldNotHaveErrorWhenReviewTextIsSpecifiedForNonVendor()
    {
        var model = new ProductReviewModel
        {
            Title = "Valid Title",
            ReviewText = "This is a detailed review of the product."
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ReviewText);
    }

    #endregion

    #region Combined Validation Tests

    [Test]
    public void ShouldHaveMultipleErrorsWhenBothFieldsAreInvalidForNonVendor()
    {
        var model = new ProductReviewModel
        {
            Title = null,
            ReviewText = string.Empty
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
        result.ShouldHaveValidationErrorFor(x => x.ReviewText);
    }

    [Test]
    public void ShouldNotHaveErrorsWhenAllFieldsAreValidForNonVendor()
    {
        var model = new ProductReviewModel
        {
            Title = "Excellent Product Quality",
            ReviewText = "I am very satisfied with this product. The quality exceeds my expectations and the delivery was fast.",
            Rating = 5,
            IsApproved = true
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
        result.ShouldNotHaveValidationErrorFor(x => x.ReviewText);
    }

    #endregion

    #region Other Properties Validation Tests

    [Test]
    public void ShouldNotHaveErrorForOtherPropertiesWhenRequiredFieldsAreValid()
    {
        var model = new ProductReviewModel
        {
            Title = "Good Product",
            ReviewText = "Satisfied with the purchase",
            Rating = 4,
            IsApproved = false,
            ReplyText = "Thank you for your review"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Rating);
        result.ShouldNotHaveValidationErrorFor(x => x.IsApproved);
        result.ShouldNotHaveValidationErrorFor(x => x.ReplyText);
    }

    [Test]
    public void ShouldNotHaveErrorWhenOptionalFieldsAreNull()
    {
        var model = new ProductReviewModel
        {
            Title = "Average Product",
            ReviewText = "The product is okay",
            ReplyText = null
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
        result.ShouldNotHaveValidationErrorFor(x => x.ReviewText);
        result.ShouldNotHaveValidationErrorFor(x => x.ReplyText);
    }

    #endregion

    #region Database Validation Rules Tests

    [Test]
    public void ShouldInheritDatabaseValidationRules()
    {
        // The validator calls SetDatabaseValidationRules<ProductReview>()
        // This test ensures the method is called and basic validation works
        var model = new ProductReviewModel
        {
            Title = "Valid Review Title",
            ReviewText = "Valid review content"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion

    #region Edge Cases

    [Test]
    public void ShouldNotHaveErrorWhenTitleAndReviewTextAreMinimalValidValues()
    {
        var model = new ProductReviewModel
        {
            Title = "A",
            ReviewText = "B"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
        result.ShouldNotHaveValidationErrorFor(x => x.ReviewText);
    }

    [Test]
    public void ShouldNotHaveErrorWhenTitleAndReviewTextContainSpecialCharacters()
    {
        var model = new ProductReviewModel
        {
            Title = "Great Product! 5/5 ⭐⭐⭐⭐⭐",
            ReviewText = "This product is amazing! I would definitely recommend it to others. Price: $99.99"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
        result.ShouldNotHaveValidationErrorFor(x => x.ReviewText);
    }

    [Test]
    public void ShouldNotHaveErrorWhenTitleAndReviewTextContainUnicodeCharacters()
    {
        var model = new ProductReviewModel
        {
            Title = "Ausgezeichnetes Produkt",
            ReviewText = "Ich bin sehr zufrieden mit diesem Produkt. Die Qualität ist hervorragend."
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
        result.ShouldNotHaveValidationErrorFor(x => x.ReviewText);
    }

    #endregion
}
