using FluentValidation.TestHelper;
using Nop.Core.Domain.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Tests;
using Nop.Web.Models.Customer;
using Nop.Web.Validators.Customer;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Public.Validators.Customer
{
    [TestFixture]
    public class RegisterValidatorTests : BaseNopTest
    {
        private RegisterValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = GetService<RegisterValidator>();
        }

        [Test]
        public void ShouldHaveErrorWhenEmailIsNullOrEmpty()
        {
            var model = new RegisterModel
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
            var model = new RegisterModel
            {
                Email = "adminexample.com"
            };
            _validator.ShouldHaveValidationErrorFor(x => x.Email, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenEmailIsCorrectFormat()
        {
            var model = new RegisterModel
            {
                Email = "admin@example.com"
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.Email, model);
        }

        [Test]
        public void ShouldHaveErrorWhenFirstnameIsNullOrEmpty()
        {
            var customerSettings = new CustomerSettings
            {
                FirstNameEnabled = true,
                FirstNameRequired = true
            };

            var validator = new RegisterValidator(GetService<ILocalizationService>(), GetService<IStateProvinceService>(), customerSettings);
            var model = new RegisterModel
            {
                FirstName = null
            };
            validator.ShouldHaveValidationErrorFor(x => x.FirstName, model);
            model.FirstName = string.Empty;
            validator.ShouldHaveValidationErrorFor(x => x.FirstName, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenFirstnameIsSpecified()
        {
            var customerSettings = new CustomerSettings
            {
                FirstNameEnabled = true
            };

            var validator = new RegisterValidator(GetService<ILocalizationService>(), GetService<IStateProvinceService>(), customerSettings);

            var model = new RegisterModel
            {
                FirstName = "John"
            };
            validator.ShouldNotHaveValidationErrorFor(x => x.FirstName, model);
        }

        [Test]
        public void ShouldHaveErrorWhenLastNameIsNullOrEmpty()
        {
            var customerSettings = new CustomerSettings
            {
                LastNameEnabled = true,
                LastNameRequired = true
            };

            var validator = new RegisterValidator(GetService<ILocalizationService>(), GetService<IStateProvinceService>(), customerSettings);

            var model = new RegisterModel
            {
                LastName = null
            };

            validator.ShouldHaveValidationErrorFor(x => x.LastName, model);
            model.LastName = string.Empty;
            validator.ShouldHaveValidationErrorFor(x => x.LastName, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenLastNameIsSpecified()
        {
            var customerSettings = new CustomerSettings
            {
                LastNameEnabled = true
            };

            var validator = new RegisterValidator(GetService<ILocalizationService>(), GetService<IStateProvinceService>(), customerSettings);

            var model = new RegisterModel
            {
                LastName = "Smith"
            };
            validator.ShouldNotHaveValidationErrorFor(x => x.LastName, model);
        }

        [Test]
        public void ShouldHaveErrorWhenPasswordIsNullOrEmpty()
        {
            var model = new RegisterModel
            {
                Password = null
            };
            //we know that password should equal confirmation password
            model.ConfirmPassword = model.Password;
            _validator.ShouldHaveValidationErrorFor(x => x.Password, model);
            model.Password = string.Empty;
            //we know that password should equal confirmation password
            model.ConfirmPassword = model.Password;
            _validator.ShouldHaveValidationErrorFor(x => x.Password, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenPasswordIsSpecified()
        {
            var model = new RegisterModel
            {
                Password = "password"
            };
            //we know that password should equal confirmation password
            model.ConfirmPassword = model.Password;
            _validator.ShouldNotHaveValidationErrorFor(x => x.Password, model);
        }

        [Test]
        public void ShouldHaveErrorWhenConfirmPasswordIsNullOrEmpty()
        {
            var model = new RegisterModel
            {
                ConfirmPassword = null
            };
            _validator.ShouldHaveValidationErrorFor(x => x.ConfirmPassword, model);
            model.ConfirmPassword = string.Empty;
            _validator.ShouldHaveValidationErrorFor(x => x.ConfirmPassword, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenConfirmPasswordIsSpecified()
        {
            var model = new RegisterModel
            {
                ConfirmPassword = "some password"
            };
            //we know that new password should equal confirmation password
            model.Password = model.ConfirmPassword;
            _validator.ShouldNotHaveValidationErrorFor(x => x.ConfirmPassword, model);
        }

        [Test]
        public void ShouldHaveErrorWhenPasswordDoesNotEqualConfirmationPassword()
        {
            var model = new RegisterModel
            {
                Password = "some password",
                ConfirmPassword = "another password"
            };
            _validator.ShouldHaveValidationErrorFor(x => x.ConfirmPassword, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenPasswordEqualsConfirmationPassword()
        {
            var model = new RegisterModel
            {
                Password = "some password",
                ConfirmPassword = "some password"
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.Password, model);
        }        
    }
}
