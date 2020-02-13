using FluentValidation.TestHelper;
using Moq;
using Nop.Core.Domain.Customers;
using Nop.Services.Directory;
using Nop.Web.Models.Customer;
using Nop.Web.Validators.Customer;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Public.Validators.Customer
{
    [TestFixture]
    public class RegisterValidatorTests : BaseValidatorTests
    {
        [Test]
        public void Should_have_error_when_email_is_null_or_empty()
        {
            var _customerSettings = new CustomerSettings();
            var _stateProvinceService = new Mock<IStateProvinceService>();
            var _validator = new RegisterValidator(_localizationService, _stateProvinceService.Object, _customerSettings);
            var model = new RegisterModel
            {
                Email = null
            };
            _validator.ShouldHaveValidationErrorFor(x => x.Email, model);
            model.Email = "";
            _validator.ShouldHaveValidationErrorFor(x => x.Email, model);
        }

        [Test]
        public void Should_have_error_when_email_is_wrong_format()
        {
            var _customerSettings = new CustomerSettings();
            var _stateProvinceService = new Mock<IStateProvinceService>();
            var _validator = new RegisterValidator(_localizationService, _stateProvinceService.Object, _customerSettings);
            var model = new RegisterModel
            {
                Email = "adminexample.com"
            };
            _validator.ShouldHaveValidationErrorFor(x => x.Email, model);
        }

        [Test]
        public void Should_not_have_error_when_email_is_correct_format()
        {
            var _customerSettings = new CustomerSettings();
            var _stateProvinceService = new Mock<IStateProvinceService>();
            var _validator = new RegisterValidator(_localizationService, _stateProvinceService.Object, _customerSettings);
            var model = new RegisterModel
            {
                Email = "admin@example.com"
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.Email, model);
        }

        [Test]
        public void Should_have_error_when_firstName_is_null_or_empty()
        {
            var _customerSettings = new CustomerSettings
            {
                FirstNameEnabled = true,
                FirstNameRequired = true
            };
            var _stateProvinceService = new Mock<IStateProvinceService>();
            var _validator = new RegisterValidator(_localizationService, _stateProvinceService.Object, _customerSettings);
            var model = new RegisterModel
            {
                FirstName = null
            };
            _validator.ShouldHaveValidationErrorFor(x => x.FirstName, model);
            model.FirstName = "";
            _validator.ShouldHaveValidationErrorFor(x => x.FirstName, model);
        }

        [Test]
        public void Should_not_have_error_when_firstName_is_specified()
        {
            var _customerSettings = new CustomerSettings
            {
                FirstNameEnabled = true
            };
            var _stateProvinceService = new Mock<IStateProvinceService>();
            var _validator = new RegisterValidator(_localizationService, _stateProvinceService.Object, _customerSettings);
            var model = new RegisterModel
            {
                FirstName = "John"
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.FirstName, model);
        }

        [Test]
        public void Should_have_error_when_lastName_is_null_or_empty()
        {
            var _customerSettings = new CustomerSettings
            {
                LastNameEnabled = true,
                LastNameRequired = true
            };
            var _stateProvinceService = new Mock<IStateProvinceService>();
            var _validator = new RegisterValidator(_localizationService, _stateProvinceService.Object, _customerSettings);
            var model = new RegisterModel
            {
                LastName = null
            };
            _validator.ShouldHaveValidationErrorFor(x => x.LastName, model);
            model.LastName = "";
            _validator.ShouldHaveValidationErrorFor(x => x.LastName, model);
        }

        [Test]
        public void Should_not_have_error_when_lastName_is_specified()
        {
            var _customerSettings = new CustomerSettings
            {
                LastNameEnabled = true
            };
            var _stateProvinceService = new Mock<IStateProvinceService>();
            var _validator = new RegisterValidator(_localizationService, _stateProvinceService.Object, _customerSettings);
            var model = new RegisterModel
            {
                LastName = "Smith"
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.LastName, model);
        }

        [Test]
        public void Should_have_error_when_password_is_null_or_empty()
        {
            var _customerSettings = new CustomerSettings();
            var _stateProvinceService = new Mock<IStateProvinceService>();
            var _validator = new RegisterValidator(_localizationService, _stateProvinceService.Object, _customerSettings);
            var model = new RegisterModel
            {
                Password = null
            };
            //we know that password should equal confirmation password
            model.ConfirmPassword = model.Password;
            _validator.ShouldHaveValidationErrorFor(x => x.Password, model);
            model.Password = "";
            //we know that password should equal confirmation password
            model.ConfirmPassword = model.Password;
            _validator.ShouldHaveValidationErrorFor(x => x.Password, model);
        }

        [Test]
        public void Should_not_have_error_when_password_is_specified()
        {
            var _customerSettings = new CustomerSettings();
            var _stateProvinceService = new Mock<IStateProvinceService>();
            var _validator = new RegisterValidator(_localizationService, _stateProvinceService.Object, _customerSettings);
            var model = new RegisterModel
            {
                Password = "password"
            };
            //we know that password should equal confirmation password
            model.ConfirmPassword = model.Password;
            _validator.ShouldNotHaveValidationErrorFor(x => x.Password, model);
        }

        [Test]
        public void Should_have_error_when_confirmPassword_is_null_or_empty()
        {
            var _customerSettings = new CustomerSettings();
            var _stateProvinceService = new Mock<IStateProvinceService>();
            var _validator = new RegisterValidator(_localizationService, _stateProvinceService.Object, _customerSettings);
            var model = new RegisterModel
            {
                ConfirmPassword = null
            };
            _validator.ShouldHaveValidationErrorFor(x => x.ConfirmPassword, model);
            model.ConfirmPassword = "";
            _validator.ShouldHaveValidationErrorFor(x => x.ConfirmPassword, model);
        }

        [Test]
        public void Should_not_have_error_when_confirmPassword_is_specified()
        {
            var _customerSettings = new CustomerSettings();
            var _stateProvinceService = new Mock<IStateProvinceService>();
            var _validator = new RegisterValidator(_localizationService, _stateProvinceService.Object, _customerSettings);
            var model = new RegisterModel
            {
                ConfirmPassword = "some password"
            };
            //we know that new password should equal confirmation password
            model.Password = model.ConfirmPassword;
            _validator.ShouldNotHaveValidationErrorFor(x => x.ConfirmPassword, model);
        }

        [Test]
        public void Should_have_error_when_password_doesnot_equal_confirmationPassword()
        {
            var _customerSettings = new CustomerSettings();
            var _stateProvinceService = new Mock<IStateProvinceService>();
            var _validator = new RegisterValidator(_localizationService, _stateProvinceService.Object, _customerSettings);
            var model = new RegisterModel
            {
                Password = "some password",
                ConfirmPassword = "another password"
            };
            _validator.ShouldHaveValidationErrorFor(x => x.ConfirmPassword, model);
        }

        [Test]
        public void Should_not_have_error_when_password_equals_confirmationPassword()
        {
            var _customerSettings = new CustomerSettings();
            var _stateProvinceService = new Mock<IStateProvinceService>();
            var _validator = new RegisterValidator(_localizationService, _stateProvinceService.Object, _customerSettings);
            var model = new RegisterModel
            {
                Password = "some password",
                ConfirmPassword = "some password"
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.Password, model);
        }        
    }
}
