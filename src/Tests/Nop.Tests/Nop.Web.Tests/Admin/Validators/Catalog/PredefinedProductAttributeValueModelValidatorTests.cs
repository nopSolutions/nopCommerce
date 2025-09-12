using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Validators.Catalog;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Admin.Validators.Catalog;

[TestFixture]
public class PredefinedProductAttributeValueModelValidatorTests : BaseNopTest
{
    private PredefinedProductAttributeValueModelValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
        _validator = new PredefinedProductAttributeValueModelValidator(GetService<ILocalizationService>());
    }

    #region Name Validation Tests

    [Test]
    public void ShouldHaveErrorWhenNameIsNull()
    {
        var model = new PredefinedProductAttributeValueModel
        {
            Name = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldHaveErrorWhenNameIsEmpty()
    {
        var model = new PredefinedProductAttributeValueModel
        {
            Name = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldHaveErrorWhenNameIsWhitespace()
    {
        var model = new PredefinedProductAttributeValueModel
        {
            Name = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameIsSpecified()
    {
        var model = new PredefinedProductAttributeValueModel
        {
            Name = "Small"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameHasValidValue()
    {
        var model = new PredefinedProductAttributeValueModel
        {
            Name = "Large Size"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    #endregion

    #region Other Properties Validation Tests

    [Test]
    public void ShouldNotHaveErrorForOtherPropertiesWhenNameIsValid()
    {
        var model = new PredefinedProductAttributeValueModel
        {
            Name = "Medium",
            ProductAttributeId = 1,
            PriceAdjustment = 10.50m,
            PriceAdjustmentUsePercentage = true,
            WeightAdjustment = 0.5m,
            Cost = 5.25m,
            IsPreSelected = false,
            DisplayOrder = 1
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.ProductAttributeId);
        result.ShouldNotHaveValidationErrorFor(x => x.PriceAdjustment);
        result.ShouldNotHaveValidationErrorFor(x => x.PriceAdjustmentUsePercentage);
        result.ShouldNotHaveValidationErrorFor(x => x.WeightAdjustment);
        result.ShouldNotHaveValidationErrorFor(x => x.Cost);
        result.ShouldNotHaveValidationErrorFor(x => x.IsPreSelected);
        result.ShouldNotHaveValidationErrorFor(x => x.DisplayOrder);
    }

    #endregion

    #region Edge Cases

    [Test]
    public void ShouldNotHaveErrorWhenNameIsMinimalValidValue()
    {
        var model = new PredefinedProductAttributeValueModel
        {
            Name = "A"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameIsLongValidValue()
    {
        var model = new PredefinedProductAttributeValueModel
        {
            Name = new string('A', 400) // Maximum length based on database schema
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameContainsSpecialCharacters()
    {
        var model = new PredefinedProductAttributeValueModel
        {
            Name = "Size: XL (Extra Large) - 100%"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameContainsNumbers()
    {
        var model = new PredefinedProductAttributeValueModel
        {
            Name = "Size 42"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameContainsUnicodeCharacters()
    {
        var model = new PredefinedProductAttributeValueModel
        {
            Name = "Größe: Groß"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    #endregion

    #region Complete Model Validation Tests

    [Test]
    public void ShouldPassValidationWhenAllPropertiesAreValid()
    {
        var model = new PredefinedProductAttributeValueModel
        {
            Id = 1,
            Name = "Extra Large",
            ProductAttributeId = 5,
            PriceAdjustment = 15.99m,
            PriceAdjustmentUsePercentage = false,
            WeightAdjustment = 1.2m,
            Cost = 8.50m,
            IsPreSelected = true,
            DisplayOrder = 10
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void ShouldFailValidationOnlyForNameWhenNameIsInvalid()
    {
        var model = new PredefinedProductAttributeValueModel
        {
            Id = 1,
            Name = "", // Invalid
            ProductAttributeId = 5,
            PriceAdjustment = 15.99m,
            PriceAdjustmentUsePercentage = false,
            WeightAdjustment = 1.2m,
            Cost = 8.50m,
            IsPreSelected = true,
            DisplayOrder = 10
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.ProductAttributeId);
        result.ShouldNotHaveValidationErrorFor(x => x.PriceAdjustment);
        result.ShouldNotHaveValidationErrorFor(x => x.PriceAdjustmentUsePercentage);
        result.ShouldNotHaveValidationErrorFor(x => x.WeightAdjustment);
        result.ShouldNotHaveValidationErrorFor(x => x.Cost);
        result.ShouldNotHaveValidationErrorFor(x => x.IsPreSelected);
        result.ShouldNotHaveValidationErrorFor(x => x.DisplayOrder);
    }

    #endregion
}
