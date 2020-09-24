using FluentValidation.TestHelper;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Tests;
using Nop.Web.Models.Customer;
using Nop.Web.Validators.Customer;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Public.Validators.Customer
{
    [TestFixture]
    public class LoginValidatorTests : BaseNopTest
    {
        private LoginValidator _validator;
        
        [SetUp]
        public void Setup()
        {
            _validator = GetService<LoginValidator>();
        }
        
        [Test]
        public void ShouldHaveErrorWhenEmailIsNullOrEmpty()
        {
            var model = new LoginModel
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
            var model = new LoginModel
            {
                Email = "adminexample.com"
            };
            _validator.ShouldHaveValidationErrorFor(x => x.Email, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenEmailIsCorrectFormat()
        {
            var model = new LoginModel
            {
                Email = "admin@example.com"
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.Email, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenEmailIsNullButUsernamesAreEnabled()
        {
            var customerSettings = new CustomerSettings
            {
                UsernamesEnabled = true
            };
            _validator = new LoginValidator(GetService<ILocalizationService>(), customerSettings);

            var model = new LoginModel
            {
                Email = null
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.Email, model);
        }
    }
}
