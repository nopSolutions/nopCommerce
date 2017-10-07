using FluentValidation.TestHelper;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Validators.Catalog;
using Nop.Web.MVC.Tests.Public.Validators;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Admin.Validators.Catalog
{
    [TestFixture]
    public class ProductAttributeValidatorTests : BaseValidatorTests
    {
        private ProductAttributeValidator _validator;

        [SetUp]
        public new void Setup()
        {
            _validator = new ProductAttributeValidator(_localizationService, null);
        }

        [Test]
        public void Should_have_error_when_name_is_empty()
        {
            var model = new ProductAttributeModel();
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
            var model = new ProductAttributeModel();
            model.Name = "Name";
            _validator.ShouldNotHaveValidationErrorFor(x => x.Name, model);
        }
    }
}