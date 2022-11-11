using FluentValidation.TestHelper;
using Nop.Core.Domain.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Models.Customer;
using Nop.Web.Validators.Customer;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Customer
{
    [TestFixture]
    public class RegisterValidatorTests : BaseNopTest
    {
        private RegisterValidator _validator;

        [OneTimeSetUp]
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
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
            model.Email = string.Empty;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void ShouldHaveErrorWhenEmailIsWrongFormat()
        {
            var model = new RegisterModel
            {
                Email = "adminexample.com"
            };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void ShouldNotHaveErrorWhenEmailIsCorrectFormat()
        {
            var model = new RegisterModel
            {
                Email = "admin@example.com"
            };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Email);
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
            validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FirstName);
            model.FirstName = string.Empty;
            validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FirstName);
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
            validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.FirstName);
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

            validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.LastName);
            model.LastName = string.Empty;
            validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.LastName);
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
            validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.LastName);
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
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Password);
            model.Password = string.Empty;
            //we know that password should equal confirmation password
            model.ConfirmPassword = model.Password;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Password);
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
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Password);
        }

        [Test]
        public void ShouldHaveErrorWhenConfirmPasswordIsNullOrEmpty()
        {
            var model = new RegisterModel
            {
                ConfirmPassword = null
            };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
            model.ConfirmPassword = string.Empty;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
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
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ConfirmPassword);
        }

        [Test]
        public void ShouldHaveErrorWhenPasswordDoesNotEqualConfirmationPassword()
        {
            var model = new RegisterModel
            {
                Password = "some password",
                ConfirmPassword = "another password"
            };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
        }

        [Test]
        public void ShouldNotHaveErrorWhenPasswordEqualsConfirmationPassword()
        {
            var model = new RegisterModel
            {
                Password = "some password",
                ConfirmPassword = "some password"
            };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Password);
        }        
    }
}
