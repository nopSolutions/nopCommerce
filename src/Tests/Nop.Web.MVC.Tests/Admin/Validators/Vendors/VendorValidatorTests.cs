using FluentValidation.TestHelper;
using Nop.Tests;
using Nop.Web.Areas.Admin.Models.Vendors;
using Nop.Web.Areas.Admin.Validators.Vendors;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Admin.Validators.Vendors
{
    [TestFixture]
    public class VendorValidatorTests : BaseNopTest
    {
        private VendorValidator _validator;

        [SetUp]
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
            _validator.ShouldHaveValidationErrorFor(x => x.PageSizeOptions, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenPageSizeOptionsHasNotDuplicateItems()
        {
            var model = new VendorModel
            {
                PageSizeOptions = "1, 2, 3, 5, 9"
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.PageSizeOptions, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenPageSizeOptionsIsNullOrEmpty()
        {
            var model = new VendorModel
            {
                PageSizeOptions = null
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.PageSizeOptions, model);
            model.PageSizeOptions = string.Empty;
            _validator.ShouldNotHaveValidationErrorFor(x => x.PageSizeOptions, model);
        }
    }
}