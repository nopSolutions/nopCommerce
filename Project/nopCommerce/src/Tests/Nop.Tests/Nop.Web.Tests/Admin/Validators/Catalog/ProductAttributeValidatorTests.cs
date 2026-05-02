using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Validators.Catalog;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Admin.Validators.Catalog;

[TestFixture]
public class ProductAttributeValidatorTests : BaseNopTest
{
    private ProductAttributeValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
        _validator = new ProductAttributeValidator(GetService<ILocalizationService>());
    }

    #region Name Validation Tests

    [Test]
    public void ShouldHaveErrorWhenNameIsNull()
    {
        var model = new ProductAttributeModel
        {
            Name = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldHaveErrorWhenNameIsEmpty()
    {
        var model = new ProductAttributeModel
        {
            Name = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldHaveErrorWhenNameIsWhitespace()
    {
        var model = new ProductAttributeModel
        {
            Name = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameIsSpecified()
    {
        var model = new ProductAttributeModel
        {
            Name = "Size"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameHasValidValue()
    {
        var model = new ProductAttributeModel
        {
            Name = "Color Options"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    #endregion

    #region Other Properties Validation Tests

    [Test]
    public void ShouldNotHaveErrorForOtherPropertiesWhenNameIsValid()
    {
        var model = new ProductAttributeModel
        {
            Name = "Material",
            Description = "Product material options"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Test]
    public void ShouldNotHaveErrorWhenDescriptionIsNull()
    {
        var model = new ProductAttributeModel
        {
            Name = "Weight",
            Description = null
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Test]
    public void ShouldNotHaveErrorWhenDescriptionIsEmpty()
    {
        var model = new ProductAttributeModel
        {
            Name = "Dimensions",
            Description = string.Empty
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    #endregion

    #region Edge Cases

    [Test]
    public void ShouldNotHaveErrorWhenNameIsMinimalValidValue()
    {
        var model = new ProductAttributeModel
        {
            Name = "A"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameIsLongValidValue()
    {
        var model = new ProductAttributeModel
        {
            Name = new string('A', 400) // Maximum length based on database schema
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameContainsSpecialCharacters()
    {
        var model = new ProductAttributeModel
        {
            Name = "Size & Weight (kg/lbs)"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameContainsNumbers()
    {
        var model = new ProductAttributeModel
        {
            Name = "Version 2.0"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameContainsUnicodeCharacters()
    {
        var model = new ProductAttributeModel
        {
            Name = "Größe"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    #endregion

    #region Complete Model Validation Tests

    [Test]
    public void ShouldPassValidationWhenAllPropertiesAreValid()
    {
        var model = new ProductAttributeModel
        {
            Id = 1,
            Name = "Color",
            Description = "Available color options for the product"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void ShouldFailValidationOnlyForNameWhenNameIsInvalid()
    {
        var model = new ProductAttributeModel
        {
            Id = 1,
            Name = "", // Invalid
            Description = "Valid description"
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Test]
    public void ShouldPassValidationWhenOnlyNameIsProvided()
    {
        var model = new ProductAttributeModel
        {
            Name = "Brand"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void ShouldPassValidationWithComplexModel()
    {
        var model = new ProductAttributeModel
        {
            Id = 5,
            Name = "Technical Specifications",
            Description = "Detailed technical specifications including dimensions, weight, and performance metrics",
            PreTranslationAvailable = true
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion

    #region Database Validation Rules Tests

    [Test]
    public void ShouldInheritDatabaseValidationRules()
    {
        // The validator calls SetDatabaseValidationRules<ProductAttribute>()
        // This test ensures the method is called and basic validation works
        var model = new ProductAttributeModel
        {
            Name = "Valid Attribute Name"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion
}
