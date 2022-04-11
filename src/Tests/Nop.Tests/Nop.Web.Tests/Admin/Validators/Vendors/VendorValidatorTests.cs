using FluentValidation.TestHelper;
using Nop.Web.Areas.Admin.Models.Vendors;
using Nop.Web.Areas.Admin.Validators.Vendors;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Admin.Validators.Vendors
{
    [TestFixture]
    public class VendorValidatorTests : BaseNopTest
    {
        private VendorValidator _validator;

        [OneTimeSetUp]
        public void Setup()
        {
            _validator = GetService<VendorValidator>();
        }

        [Test]
        public void ShouldHaveErrorWhenPageSizeOptionsHasDuplicateItems()
        {
            var model = new VendorModel
            {
                PageSizeOptions = "1, 2, 3, 5, 2"
            };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.PageSizeOptions);
        }

        [Test]
        public void ShouldNotHaveErrorWhenPageSizeOptionsHasNotDuplicateItems()
        {
            var model = new VendorModel
            {
                PageSizeOptions = "1, 2, 3, 5, 9"
            };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.PageSizeOptions);
        }

        [Test]
        public void ShouldNotHaveErrorWhenPageSizeOptionsIsNullOrEmpty()
        {
            var model = new VendorModel
            {
                PageSizeOptions = null
            };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.PageSizeOptions);
            model.PageSizeOptions = string.Empty;
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.PageSizeOptions);
        }
    }
}