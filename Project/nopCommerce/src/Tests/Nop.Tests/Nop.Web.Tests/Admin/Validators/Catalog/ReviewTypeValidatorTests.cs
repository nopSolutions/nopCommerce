using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Validators.Catalog;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Admin.Validators.Catalog;

[TestFixture]
public class ReviewTypeValidatorTests : BaseNopTest
{
    private ReviewTypeValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
        _validator = new ReviewTypeValidator(GetService<ILocalizationService>());
    }

    #region Name Validation Tests

    [Test]
    public void ShouldHaveErrorWhenNameIsNull()
    {
        var model = new ReviewTypeModel
        {
            Name = null,
            Description = "Valid description"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldHaveErrorWhenNameIsEmpty()
    {
        var model = new ReviewTypeModel
        {
            Name = string.Empty,
            Description = "Valid description"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldHaveErrorWhenNameIsWhitespace()
    {
        var model = new ReviewTypeModel
        {
            Name = "   ",
            Description = "Valid description"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameIsSpecified()
    {
        var model = new ReviewTypeModel
        {
            Name = "Quality Rating",
            Description = "Valid description"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    #endregion

    #region Description Validation Tests

    [Test]
    public void ShouldHaveErrorWhenDescriptionIsNull()
    {
        var model = new ReviewTypeModel
        {
            Name = "Valid Name",
            Description = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Test]
    public void ShouldHaveErrorWhenDescriptionIsEmpty()
    {
        var model = new ReviewTypeModel
        {
            Name = "Valid Name",
            Description = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Test]
    public void ShouldHaveErrorWhenDescriptionIsWhitespace()
    {
        var model = new ReviewTypeModel
        {
            Name = "Valid Name",
            Description = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Test]
    public void ShouldNotHaveErrorWhenDescriptionIsSpecified()
    {
        var model = new ReviewTypeModel
        {
            Name = "Valid Name",
            Description = "Rate the overall quality of the product"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    #endregion

    #region Combined Validation Tests

    [Test]
    public void ShouldHaveMultipleErrorsWhenBothFieldsAreInvalid()
    {
        var model = new ReviewTypeModel
        {
            Name = null,
            Description = string.Empty
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Test]
    public void ShouldNotHaveErrorsWhenAllFieldsAreValid()
    {
        var model = new ReviewTypeModel
        {
            Name = "Service Quality",
            Description = "Rate the quality of customer service received",
            DisplayOrder = 1,
            VisibleToAllCustomers = true
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Test]
    public void ShouldNotHaveErrorForOtherPropertiesWhenRequiredFieldsAreValid()
    {
        var model = new ReviewTypeModel
        {
            Name = "Delivery Speed",
            Description = "Rate the speed of product delivery",
            DisplayOrder = 5,
            VisibleToAllCustomers = false
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.DisplayOrder);
        result.ShouldNotHaveValidationErrorFor(x => x.VisibleToAllCustomers);
    }

    #endregion

    #region Edge Cases

    [Test]
    public void ShouldNotHaveErrorWhenNameAndDescriptionAreMinimalValidValues()
    {
        var model = new ReviewTypeModel
        {
            Name = "A",
            Description = "B"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameAndDescriptionContainSpecialCharacters()
    {
        var model = new ReviewTypeModel
        {
            Name = "Price/Value Ratio",
            Description = "Rate the price-to-value ratio (1-5 stars)"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameAndDescriptionContainUnicodeCharacters()
    {
        var model = new ReviewTypeModel
        {
            Name = "Qualité",
            Description = "Évaluez la qualité du produit"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    #endregion

    #region Database Validation Rules Tests

    [Test]
    public void ShouldInheritDatabaseValidationRules()
    {
        // The validator calls SetDatabaseValidationRules<ReviewType>()
        // This test ensures the method is called and basic validation works
        var model = new ReviewTypeModel
        {
            Name = "Valid Review Type",
            Description = "Valid description for the review type"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion
}
