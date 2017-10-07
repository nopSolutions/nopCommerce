using FluentValidation.TestHelper;
using Nop.Core.Domain.Catalog;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Validators.Catalog;
using Nop.Web.MVC.Tests.Public.Validators;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Admin.Validators.Catalog
{
    public class ProductAttributeValueModelValidatorTests : BaseValidatorTests
    {
        private ProductAttributeValueModelValidator _validator;

        [SetUp]
        public new void Setup()
        {
            _validator = new ProductAttributeValueModelValidator(_localizationService, null);
        }

        [Test]
        public void Should_have_error_when_name_is_empty()
        {
            var model = new ProductModel.ProductAttributeValueModel();
            model.Name = string.Empty;
            _validator.ShouldHaveValidationErrorFor(x => x.Name, model);

            model.Name = null;
            _validator.ShouldHaveValidationErrorFor(x => x.Name, model);

            model.Name = " ";
            _validator.ShouldHaveValidationErrorFor(x => x.Name, model);
        }

        [Test]
        public void Should_not_have_error_when_name_is_not_empty()
        {
            var model = new ProductModel.ProductAttributeValueModel();
            model.Name = "Name";
            _validator.ShouldNotHaveValidationErrorFor(x => x.Name, model);
        }

        [Test]
        public void Should_have_error_when_quantity_is_less_than_one_and_customer_does_not_enters_qty_and_it_is_associated_to_product()
        {
            var model = new ProductModel.ProductAttributeValueModel();
            model.Quantity = 0;
            model.AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct;
            model.CustomerEntersQty = false;
            _validator.ShouldHaveValidationErrorFor(x => x.Quantity, model);
        }

        [Test]
        public void Should_not_have_error_when_quantity_is_equal_or_greater_than_one_and_customer_does_not_enters_qty_and_it_is_associated_to_product()
        {
            var model = new ProductModel.ProductAttributeValueModel();
            model.CustomerEntersQty = false;
            model.AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct;

            model.Quantity = 1;
            _validator.ShouldNotHaveValidationErrorFor(x => x.Quantity, model);

            model.Quantity = 2;
            _validator.ShouldNotHaveValidationErrorFor(x => x.Quantity, model);
        }

        [Test]
        public void Should_have_error_when_quantity_is_less_than_one_and_customer_enters_qty_and_it_is_associated_to_product()
        {
            var model = new ProductModel.ProductAttributeValueModel();
            model.CustomerEntersQty = true;
            model.AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct;

            model.Quantity = 0;
            _validator.ShouldNotHaveValidationErrorFor(x => x.Quantity, model);
        }

        [Test]
        public void Should_not_have_error_when_associated_product_id_is_equal_or_greater_then_one_when_type_is_associated_to_product()
        {
            var model = new ProductModel.ProductAttributeValueModel();
            model.AssociatedProductId = 1;
            model.AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct;
            _validator.ShouldNotHaveValidationErrorFor(x => x.AssociatedProductId, model);
        }

        [Test]
        public void Should_have_error_when_associated_product_id_is_less_then_one_when_type_is_associated_to_product()
        {
            var model = new ProductModel.ProductAttributeValueModel();
            model.AssociatedProductId = 0;
            model.AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct;
            _validator.ShouldHaveValidationErrorFor(x => x.AssociatedProductId, model);
        }

        [Test]
        public void Should_not_have_error_when_associated_product_id_is_less_then_one_when_type_is_not_associated_to_product()
        {
            var model = new ProductModel.ProductAttributeValueModel();
            model.AssociatedProductId = 0;
            _validator.ShouldNotHaveValidationErrorFor(x => x.AssociatedProductId, model);
        }
    }
}