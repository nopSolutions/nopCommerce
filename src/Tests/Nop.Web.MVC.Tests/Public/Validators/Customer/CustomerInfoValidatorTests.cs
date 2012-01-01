using FluentValidation.TestHelper;
using Nop.Web.Models.Customer;
using Nop.Web.Validators.Customer;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Public.Validators.Customer
{
    [TestFixture]
    public class CustomerInfoValidatorTests : BaseValidatorTests
    {
        private CustomerInfoValidator _validator;
        
        [SetUp]
        public new void Setup()
        {
            _validator = new CustomerInfoValidator(_localizationService);
        }
        
        [Test]
        public void Should_have_error_when_email_is_null_or_empty()
        {
            var model = new CustomerInfoModel();
            model.Email = null;
            _validator.ShouldHaveValidationErrorFor(x => x.Email, model);
            model.Email = "";
            _validator.ShouldHaveValidationErrorFor(x => x.Email, model);
        }

        [Test]
        public void Should_have_error_when_email_is_wrong_format()
        {
            var model = new CustomerInfoModel();
            model.Email = "adminexample.com";
            _validator.ShouldHaveValidationErrorFor(x => x.Email, model);
        }

        [Test]
        public void Should_not_have_error_when_email_is_correct_format()
        {
            var model = new CustomerInfoModel();
            model.Email = "admin@example.com";
            _validator.ShouldNotHaveValidationErrorFor(x => x.Email, model);
        }

        [Test]
        public void Should_have_error_when_firstName_is_null_or_empty()
        {
            var model = new CustomerInfoModel();
            model.FirstName = null;
            _validator.ShouldHaveValidationErrorFor(x => x.FirstName, model);
            model.FirstName = "";
            _validator.ShouldHaveValidationErrorFor(x => x.FirstName, model);
        }

        [Test]
        public void Should_not_have_error_when_firstName_is_specified()
        {
            var model = new CustomerInfoModel();
            model.FirstName = "John";
            _validator.ShouldNotHaveValidationErrorFor(x => x.FirstName, model);
        }

        [Test]
        public void Should_have_error_when_lastName_is_null_or_empty()
        {
            var model = new CustomerInfoModel();
            model.LastName = null;
            _validator.ShouldHaveValidationErrorFor(x => x.LastName, model);
            model.LastName = "";
            _validator.ShouldHaveValidationErrorFor(x => x.LastName, model);
        }

        [Test]
        public void Should_not_have_error_when_lastName_is_specified()
        {
            var model = new CustomerInfoModel();
            model.LastName = "Smith";
            _validator.ShouldNotHaveValidationErrorFor(x => x.LastName, model);
        }
    }
}
