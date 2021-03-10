using FluentValidation.TestHelper;
using Nop.Core.Domain.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;
using Nop.Web.Validators.Customer;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators
{
    [TestFixture]
    public class PasswordValidatorTests : BaseNopTest
    {
        private Person _person;
        private ChangePasswordValidator _changePasswordValidator;
        private PasswordRecoveryConfirmValidator _passwordRecoveryConfirmValidator;
        private RegisterValidator _registerValidator;
        private ILocalizationService _localizationService;
        private IStateProvinceService _stateProvinceService;
        private CustomerSettings _customerSettings;

        [OneTimeSetUp]
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

            _person = new Person();
        }

        [Test]
        public void IsValidTestsLowercase()
        {
            var validator = new TestValidator();

            var cs = new CustomerSettings
            {
                PasswordMinLength = 3,
                PasswordRequireLowercase = true
            };

            validator.RuleFor(x => x.Password).IsPassword(_localizationService, cs);

            //ShouldHaveValidationError
            validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "NOP123");

            //ShouldNotHaveValidationError
            validator.ShouldNotHaveValidationErrorFor(x => x.Password, _person.Password = "nop123");
        }

        [Test]
        public void IsValidTestsUppercase()
        {
            var validator = new TestValidator();

            var cs = new CustomerSettings
            {
                PasswordMinLength = 3,
                PasswordRequireUppercase = true                
            };

            validator.RuleFor(x => x.Password).IsPassword(_localizationService, cs);

            //ShouldHaveValidationError
            validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "nop");           

            //ShouldNotHaveValidationError
            validator.ShouldNotHaveValidationErrorFor(x => x.Password, _person.Password = "Nop");
        }

        [Test]
        public void IsValidTestsDigit()
        {
            var validator = new TestValidator();

            var cs = new CustomerSettings
            {
                PasswordMinLength = 3,
                PasswordRequireDigit = true
            };

            validator.RuleFor(x => x.Password).IsPassword(_localizationService, cs);

            //ShouldHaveValidationError
            validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "nop");

            //ShouldNotHaveValidationError
            validator.ShouldNotHaveValidationErrorFor(x => x.Password, _person.Password = "Nop1");
        }

        [Test]
        public void IsValidTestsNonAlphanumeric()
        {
            var validator = new TestValidator();

            var cs = new CustomerSettings
            {
                PasswordMinLength = 3,
                PasswordRequireNonAlphanumeric = true
            };

            validator.RuleFor(x => x.Password).IsPassword(_localizationService, cs);
            
            //ShouldHaveValidationError
            validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "nop");

            //ShouldNotHaveValidationError
            validator.ShouldNotHaveValidationErrorFor(x => x.Password, _person.Password = "Nop#");
        }

        [Test]
        public void IsValidTestsAllRules()
        {
            var validator = new TestValidator();

            //Example:  (?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{6,}$
            validator.RuleFor(x => x.Password).IsPassword(_localizationService, _customerSettings);

            //ShouldHaveValidationError
            validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = string.Empty);
            validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "123");
            validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "12345678");
            validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "nopcommerce");
            validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "nopCommerce");
            validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "nopcommerce123");
            validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "nopCommerce123");
            validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "nopcommerce123$");
            validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "NOPCOMMERCE123$");
            validator.ShouldHaveValidationErrorFor(x => x.Password, _person.Password = "nopCommerce123~");

            //ShouldNotHaveValidationError
            validator.ShouldNotHaveValidationErrorFor(x => x.Password, _person.Password = "nopCommerce123$");            
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
