using FluentValidation.TestHelper;
using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Validators.Catalog;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Admin.Validators.Catalog;

[TestFixture]
public class ProductAttributeValueModelValidatorTests : BaseNopTest
{
    private ProductAttributeValueModelValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
        _validator = new ProductAttributeValueModelValidator(GetService<ILocalizationService>());
    }

    #region Name Validation Tests

    [Test]
    public void ShouldHaveErrorWhenNameIsNull()
    {
        var model = new ProductAttributeValueModel
        {
            Name = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldHaveErrorWhenNameIsEmpty()
    {
        var model = new ProductAttributeValueModel
        {
            Name = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldHaveErrorWhenNameIsWhitespace()
    {
        var model = new ProductAttributeValueModel
        {
            Name = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameIsSpecified()
    {
        var model = new ProductAttributeValueModel
        {
            Name = "Red"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    #endregion

    #region Quantity Validation Tests

    [Test]
    public void ShouldHaveErrorWhenQuantityIsLessThan1AndAttributeValueTypeIsAssociatedToProductAndCustomerDoesNotEnterQty()
    {
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            CustomerEntersQty = false,
            Quantity = 0
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Test]
    public void ShouldHaveErrorWhenQuantityIsNegativeAndAttributeValueTypeIsAssociatedToProductAndCustomerDoesNotEnterQty()
    {
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            CustomerEntersQty = false,
            Quantity = -1
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Test]
    public void ShouldNotHaveErrorWhenQuantityIs1AndAttributeValueTypeIsAssociatedToProductAndCustomerDoesNotEnterQty()
    {
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            CustomerEntersQty = false,
            Quantity = 1
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Quantity);
    }

    [Test]
    public void ShouldNotHaveErrorWhenQuantityIsGreaterThan1AndAttributeValueTypeIsAssociatedToProductAndCustomerDoesNotEnterQty()
    {
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            CustomerEntersQty = false,
            Quantity = 5
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Quantity);
    }

    [Test]
    public void ShouldNotHaveErrorWhenQuantityIsLessThan1AndAttributeValueTypeIsAssociatedToProductButCustomerEntersQty()
    {
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            CustomerEntersQty = true,
            Quantity = 0
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Quantity);
    }

    [Test]
    public void ShouldNotHaveErrorWhenQuantityIsLessThan1AndAttributeValueTypeIsNotAssociatedToProduct()
    {
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.Simple,
            CustomerEntersQty = false,
            Quantity = 0
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Quantity);
    }

    #endregion

    #region AssociatedProductId Validation Tests

    [Test]
    public void ShouldHaveErrorWhenAssociatedProductIdIsLessThan1AndAttributeValueTypeIsAssociatedToProduct()
    {
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            AssociatedProductId = 0
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AssociatedProductId);
    }

    [Test]
    public void ShouldHaveErrorWhenAssociatedProductIdIsNegativeAndAttributeValueTypeIsAssociatedToProduct()
    {
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            AssociatedProductId = -1
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AssociatedProductId);
    }

    [Test]
    public void ShouldNotHaveErrorWhenAssociatedProductIdIs1AndAttributeValueTypeIsAssociatedToProduct()
    {
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            AssociatedProductId = 1
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AssociatedProductId);
    }

    [Test]
    public void ShouldNotHaveErrorWhenAssociatedProductIdIsGreaterThan1AndAttributeValueTypeIsAssociatedToProduct()
    {
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            AssociatedProductId = 10
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AssociatedProductId);
    }

    [Test]
    public void ShouldNotHaveErrorWhenAssociatedProductIdIsLessThan1AndAttributeValueTypeIsNotAssociatedToProduct()
    {
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.Simple,
            AssociatedProductId = 0
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AssociatedProductId);
    }

    #endregion

    #region Combined Validation Tests

    [Test]
    public void ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalidForAssociatedProduct()
    {
        var model = new ProductAttributeValueModel
        {
            Name = null, // Invalid
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            CustomerEntersQty = false,
            Quantity = 0, // Invalid
            AssociatedProductId = 0 // Invalid
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
        result.ShouldHaveValidationErrorFor(x => x.AssociatedProductId);
    }

    [Test]
    public void ShouldNotHaveErrorsWhenAllFieldsAreValidForAssociatedProduct()
    {
        var model = new ProductAttributeValueModel
        {
            Name = "Bundle Product",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            CustomerEntersQty = false,
            Quantity = 2,
            AssociatedProductId = 5
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
        result.ShouldNotHaveValidationErrorFor(x => x.AssociatedProductId);
    }

    [Test]
    public void ShouldNotHaveErrorsWhenAllFieldsAreValidForSimpleAttribute()
    {
        var model = new ProductAttributeValueModel
        {
            Name = "Large Size",
            AttributeValueTypeId = (int)AttributeValueType.Simple,
            CustomerEntersQty = false,
            Quantity = 0, // Not validated for Simple type
            AssociatedProductId = 0 // Not validated for Simple type
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
        result.ShouldNotHaveValidationErrorFor(x => x.AssociatedProductId);
    }

    [Test]
    public void ShouldPassValidationWhenCustomerEntersQtyForAssociatedProduct()
    {
        var model = new ProductAttributeValueModel
        {
            Name = "Custom Bundle",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            CustomerEntersQty = true, // Customer enters quantity
            Quantity = 0, // Not validated when customer enters qty
            AssociatedProductId = 3
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
        result.ShouldNotHaveValidationErrorFor(x => x.AssociatedProductId);
    }

    #endregion
}
