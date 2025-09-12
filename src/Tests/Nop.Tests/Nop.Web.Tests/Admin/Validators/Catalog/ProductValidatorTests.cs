using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Validators.Catalog;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Admin.Validators.Catalog;

[TestFixture]
public class ProductValidatorTests : BaseNopTest
{
    private ProductValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
        _validator = new ProductValidator(GetService<ILocalizationService>());
    }

    #region Name Validation Tests

    [Test]
    public void ShouldHaveErrorWhenNameIsNull()
    {
        var model = new ProductModel
        {
            Name = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldHaveErrorWhenNameIsEmpty()
    {
        var model = new ProductModel
        {
            Name = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldHaveErrorWhenNameIsWhitespace()
    {
        var model = new ProductModel
        {
            Name = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameIsSpecified()
    {
        var model = new ProductModel
        {
            Name = "Sample Product"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    #endregion

    #region SeName Validation Tests

    [Test]
    public void ShouldNotHaveErrorWhenSeNameIsNull()
    {
        var model = new ProductModel
        {
            Name = "Valid Product",
            SeName = null
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.SeName);
    }

    [Test]
    public void ShouldNotHaveErrorWhenSeNameIsEmpty()
    {
        var model = new ProductModel
        {
            Name = "Valid Product",
            SeName = string.Empty
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.SeName);
    }

    [Test]
    public void ShouldNotHaveErrorWhenSeNameIsWithinMaxLength()
    {
        var model = new ProductModel
        {
            Name = "Valid Product",
            SeName = new string('a', NopSeoDefaults.SearchEngineNameLength) // 200 characters
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.SeName);
    }

    [Test]
    public void ShouldHaveErrorWhenSeNameExceedsMaxLength()
    {
        var model = new ProductModel
        {
            Name = "Valid Product",
            SeName = new string('a', NopSeoDefaults.SearchEngineNameLength + 1) // 201 characters
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.SeName);
    }

    [Test]
    public void ShouldNotHaveErrorWhenSeNameIsValidLength()
    {
        var model = new ProductModel
        {
            Name = "Valid Product",
            SeName = "sample-product-sename"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.SeName);
    }

    #endregion

    #region RentalPriceLength Validation Tests

    [Test]
    public void ShouldHaveErrorWhenRentalPriceLengthIsZeroAndIsRental()
    {
        var model = new ProductModel
        {
            Name = "Valid Product",
            IsRental = true,
            RentalPriceLength = 0
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.RentalPriceLength);
    }

    [Test]
    public void ShouldHaveErrorWhenRentalPriceLengthIsNegativeAndIsRental()
    {
        var model = new ProductModel
        {
            Name = "Valid Product",
            IsRental = true,
            RentalPriceLength = -1
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.RentalPriceLength);
    }

    [Test]
    public void ShouldNotHaveErrorWhenRentalPriceLengthIsPositiveAndIsRental()
    {
        var model = new ProductModel
        {
            Name = "Valid Product",
            IsRental = true,
            RentalPriceLength = 7
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.RentalPriceLength);
    }

    [Test]
    public void ShouldNotHaveErrorWhenRentalPriceLengthIsZeroAndIsNotRental()
    {
        var model = new ProductModel
        {
            Name = "Valid Product",
            IsRental = false,
            RentalPriceLength = 0
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.RentalPriceLength);
    }

    #endregion

    #region MinimumAgeToPurchase Validation Tests

    [Test]
    public void ShouldHaveErrorWhenMinimumAgeToPurchaseIsZeroAndAgeVerificationIsEnabled()
    {
        var model = new ProductModel
        {
            Name = "Valid Product",
            AgeVerification = true,
            MinimumAgeToPurchase = 0
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.MinimumAgeToPurchase);
    }

    [Test]
    public void ShouldHaveErrorWhenMinimumAgeToPurchaseIsNegativeAndAgeVerificationIsEnabled()
    {
        var model = new ProductModel
        {
            Name = "Valid Product",
            AgeVerification = true,
            MinimumAgeToPurchase = -1
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.MinimumAgeToPurchase);
    }

    [Test]
    public void ShouldNotHaveErrorWhenMinimumAgeToPurchaseIsPositiveAndAgeVerificationIsEnabled()
    {
        var model = new ProductModel
        {
            Name = "Valid Product",
            AgeVerification = true,
            MinimumAgeToPurchase = 18
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.MinimumAgeToPurchase);
    }

    [Test]
    public void ShouldNotHaveErrorWhenMinimumAgeToPurchaseIsZeroAndAgeVerificationIsDisabled()
    {
        var model = new ProductModel
        {
            Name = "Valid Product",
            AgeVerification = false,
            MinimumAgeToPurchase = 0
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.MinimumAgeToPurchase);
    }

    #endregion

    #region Combined Validation Tests

    [Test]
    public void ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalid()
    {
        var model = new ProductModel
        {
            Name = null, // Invalid
            SeName = new string('a', NopSeoDefaults.SearchEngineNameLength + 1), // Invalid
            IsRental = true,
            RentalPriceLength = 0, // Invalid
            AgeVerification = true,
            MinimumAgeToPurchase = 0 // Invalid
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.SeName);
        result.ShouldHaveValidationErrorFor(x => x.RentalPriceLength);
        result.ShouldHaveValidationErrorFor(x => x.MinimumAgeToPurchase);
    }

    [Test]
    public void ShouldNotHaveErrorsWhenAllFieldsAreValid()
    {
        var model = new ProductModel
        {
            Name = "Premium Product",
            SeName = "premium-product",
            IsRental = true,
            RentalPriceLength = 30,
            AgeVerification = true,
            MinimumAgeToPurchase = 21
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.SeName);
        result.ShouldNotHaveValidationErrorFor(x => x.RentalPriceLength);
        result.ShouldNotHaveValidationErrorFor(x => x.MinimumAgeToPurchase);
    }

    [Test]
    public void ShouldNotHaveErrorsWhenOptionalValidationsAreDisabled()
    {
        var model = new ProductModel
        {
            Name = "Standard Product",
            SeName = "standard-product",
            IsRental = false, // Rental validation disabled
            RentalPriceLength = 0,
            AgeVerification = false, // Age verification disabled
            MinimumAgeToPurchase = 0
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.SeName);
        result.ShouldNotHaveValidationErrorFor(x => x.RentalPriceLength);
        result.ShouldNotHaveValidationErrorFor(x => x.MinimumAgeToPurchase);
    }

    #endregion
}
