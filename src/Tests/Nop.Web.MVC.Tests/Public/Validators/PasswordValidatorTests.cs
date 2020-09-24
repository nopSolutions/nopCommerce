using FluentValidation.TestHelper;
using Nop.Core.Domain.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Tests;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;
using Nop.Web.Validators.Customer;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Public.Validators
{
    [TestFixture]
    public class PasswordValidatorTests : BaseNopTest
    {
        private TestValidator _validator;
        private Person _person;
        private ChangePasswordValidator _changePasswordValidator;
        private PasswordRecoveryConfirmValidator _passwordRecoveryConfirmValidator;
        private RegisterValidator _registerValidator;
        private ILocalizationService _localizationService;
        private IStateProvinceService _stateProvinceService;
        private CustomerSettings _customerSettings;

        [SetUp]
        public void Setup()
        {
            _customerSettings = new CustomerSettings
            {
                PasswordMinLength = 8,
                PasswordRequireUppercase = true,
                PasswordRequireLowercase = true,
                PasswordRequireDigit = true,
                PasswordRequireNonAlphanumeric = true
            };

            _localizationService = GetService<ILocalizationService>();
            _stateProvinceService = GetService<IStateProvinceService>();
            _changePasswordValidator = new ChangePasswordValidator(_localizationService, _customerSettings);
            _registerValidator = new RegisterValidator(_localizationService, _stateProvinceService, _customerSettings);
            _passwordRecoveryConfirmValidator = new PasswordRecoveryConfirmValidator(_localizationService, _customerSettings);

            _validator = new TestValidator();
            _person = new Person();
        }

        [Test]
        public void IsValidTestsLowercase()
        {
            var cs = new CustomerSettings
            {
                PasswordMinLength = 3,
                PasswordRequireLowercase = true
            };

            _validator.RuleFor(x => x.Password).IsPassword(_localizationService, cs);

            //ShouldHaveValidationError
            _validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "NOP123");

            //ShouldNotHaveValidationError
            _validator.ShouldNotHaveValidationErrorFor(x => x.Password, _person.Password = "nop123");
        }

        [Test]
        public void IsValidTestsUppercase()
        {
            var cs = new CustomerSettings
            {
                PasswordMinLength = 3,
                PasswordRequireUppercase = true                
            };

            _validator.RuleFor(x => x.Password).IsPassword(_localizationService, cs);

            //ShouldHaveValidationError
            _validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "nop");           

            //ShouldNotHaveValidationError
            _validator.ShouldNotHaveValidationErrorFor(x => x.Password, _person.Password = "Nop");
        }

        [Test]
        public void IsValidTestsDigit()
        {
            var cs = new CustomerSettings
            {
                PasswordMinLength = 3,
                PasswordRequireDigit = true
            };

            _validator.RuleFor(x => x.Password).IsPassword(_localizationService, cs);

            //ShouldHaveValidationError
            _validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "nop");

            //ShouldNotHaveValidationError
            _validator.ShouldNotHaveValidationErrorFor(x => x.Password, _person.Password = "Nop1");
        }

        [Test]
        public void IsValidTestsNonAlphanumeric()
        {
            var cs = new CustomerSettings
            {
                PasswordMinLength = 3,
                PasswordRequireNonAlphanumeric = true
            };

            _validator.RuleFor(x => x.Password).IsPassword(_localizationService, cs);
            
            //ShouldHaveValidationError
            _validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "nop");

            //ShouldNotHaveValidationError
            _validator.ShouldNotHaveValidationErrorFor(x => x.Password, _person.Password = "Nop#");
        }

        [Test]
        public void IsValidTestsAllRules()
        {
            //Example:  (?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{6,}$
            _validator.RuleFor(x => x.Password).IsPassword(_localizationService, _customerSettings);

            //ShouldHaveValidationError
            _validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = string.Empty);
            _validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "123");
            _validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "12345678");
            _validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "nopcommerce");
            _validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "nopCommerce");
            _validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "nopcommerce123");
            _validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "nopCommerce123");
            _validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "nopcommerce123$");
            _validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "NOPCOMMERCE123$");
            _validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "nopCommerce123~");

            //ShouldNotHaveValidationError
            _validator.ShouldNotHaveValidationErrorFor(x => x.Password, _person.Password = "nopCommerce123$");            
        }
        
        [Test]
        public void ShouldValidateOnChangePasswordModelIsAllRule()
        {            
            var model = new ChangePasswordModel
            {
                NewPassword = "1234"
            };
            //we know that new password should equal confirmation password
            model.ConfirmNewPassword = model.NewPassword;
            _changePasswordValidator.ShouldHaveValidationErrorFor(x => x.NewPassword, model);
            model.NewPassword = "nopCommerce123$";
            //we know that new password should equal confirmation password
            model.ConfirmNewPassword = model.NewPassword;
            _changePasswordValidator.ShouldNotHaveValidationErrorFor(x => x.NewPassword, model);
        }

        [Test]
        public void ShouldValidateOnPasswordRecoveryConfirmModelIsAllRule()
        {            
            var model = new PasswordRecoveryConfirmModel
            {
                NewPassword = "1234"
            };
            //we know that new password should equal confirmation password
            model.ConfirmNewPassword = model.NewPassword;
            _passwordRecoveryConfirmValidator.ShouldHaveValidationErrorFor(x => x.NewPassword, model);
            model.NewPassword = "nopCommerce123$";
            //we know that new password should equal confirmation password
            model.ConfirmNewPassword = model.NewPassword;
            _passwordRecoveryConfirmValidator.ShouldNotHaveValidationErrorFor(x => x.NewPassword, model);
        }

        [Test]
        public void ShouldValidateOnRegisterModelIsAllRule()
        {   
            var model = new RegisterModel
            {
                Password = "1234"
            };
            //we know that password should equal confirmation password
            model.ConfirmPassword = model.Password;
            _registerValidator.ShouldHaveValidationErrorFor(x => x.Password, model);
            model.Password = "nopCommerce123$";
            //we know that password should equal confirmation password
            model.ConfirmPassword = model.Password;
            _registerValidator.ShouldNotHaveValidationErrorFor(x => x.Password, model);
        }        
    }
}
