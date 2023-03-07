using FluentValidation.TestHelper;
using Nop.Web.Models.Common;
using Nop.Web.Validators.Common;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Common
{
    [TestFixture]
    public class ContactUsValidatorTests : BaseNopTest
    {
        private ContactUsValidator _validator;

        [OneTimeSetUp]
        public void Setup()
        {
            _validator = GetService<ContactUsValidator>();
        }

        [Test]
        public void ShouldHaveErrorWhenEmailIsNullOrEmpty()
        {
            var model = new ContactUsModel
            {
                Email = null
            };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
            model.Email = string.Empty;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void ShouldHaveErrorWhenEmailIsWrongFormat()
        {
            var model = new ContactUsModel
            {
                Email = "adminexample.com"
            };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void ShouldNotHaveErrorWhenEmailIsCorrectFormat()
        {
            var model = new ContactUsModel
            {
                Email = "admin@example.com"
            };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void ShouldHaveErrorWhenFullnameIsNullOrEmpty()
        {
            var model = new ContactUsModel
            {
                FullName = null
            };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FullName);
            model.FullName = string.Empty;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FullName);
        }

        [Test]
        public void ShouldNotHaveErrorWhenFullnameIsSpecified()
        {
            var model = new ContactUsModel
            {
                FullName = "John Smith"
            };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.FullName);
        }

        [Test]
        public void ShouldHaveErrorWhenEnquiryIsNullOrEmpty()
        {
            var model = new ContactUsModel
            {
                Enquiry = null
            };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Enquiry);
            model.Enquiry = string.Empty;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Enquiry);
        }

        [Test]
        public void ShouldNotHaveErrorWhenEnquiryIsSpecified()
        {
            var model = new ContactUsModel
            {
                Enquiry = "please call me back"
            };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Enquiry);
        }
    }
}
