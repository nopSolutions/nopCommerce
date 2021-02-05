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
            _validator.ShouldHaveValidationErrorFor(x => x.Email, model);
            model.Email = string.Empty;
            _validator.ShouldHaveValidationErrorFor(x => x.Email, model);
        }

        [Test]
        public void ShouldHaveErrorWhenEmailIsWrongFormat()
        {
            var model = new ContactUsModel
            {
                Email = "adminexample.com"
            };
            _validator.ShouldHaveValidationErrorFor(x => x.Email, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenEmailIsCorrectFormat()
        {
            var model = new ContactUsModel
            {
                Email = "admin@example.com"
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.Email, model);
        }

        [Test]
        public void ShouldHaveErrorWhenFullnameIsNullOrEmpty()
        {
            var model = new ContactUsModel
            {
                FullName = null
            };
            _validator.ShouldHaveValidationErrorFor(x => x.FullName, model);
            model.FullName = string.Empty;
            _validator.ShouldHaveValidationErrorFor(x => x.FullName, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenFullnameIsSpecified()
        {
            var model = new ContactUsModel
            {
                FullName = "John Smith"
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.FullName, model);
        }

        [Test]
        public void ShouldHaveErrorWhenEnquiryIsNullOrEmpty()
        {
            var model = new ContactUsModel
            {
                Enquiry = null
            };
            _validator.ShouldHaveValidationErrorFor(x => x.Enquiry, model);
            model.Enquiry = string.Empty;
            _validator.ShouldHaveValidationErrorFor(x => x.Enquiry, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenEnquiryIsSpecified()
        {
            var model = new ContactUsModel
            {
                Enquiry = "please call me back"
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.Enquiry, model);
        }
    }
}
