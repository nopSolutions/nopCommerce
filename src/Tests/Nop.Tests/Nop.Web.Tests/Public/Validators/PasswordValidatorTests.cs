using FluentValidation.TestHelper;
using Nop.Core.Domain.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;
using Nop.Web.Validators.Customer;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators;

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
            PasswordMaxLength = 20,
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
            PasswordMaxLength = 32,
            PasswordRequireLowercase = true
        };

        validator.RuleFor(x => x.Password).IsPassword(_localizationService, cs);

        //ShouldHaveValidationError
        _person.Password = "NOP123";
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);

        //ShouldNotHaveValidationError
        _person.Password = "nop123";
        validator.TestValidate(_person).ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Test]
    public void IsValidTestsUppercase()
    {
        var validator = new TestValidator();

        var cs = new CustomerSettings
        {
            PasswordMinLength = 3,
            PasswordMaxLength = 32,
            PasswordRequireUppercase = true
        };

        validator.RuleFor(x => x.Password).IsPassword(_localizationService, cs);

        //ShouldHaveValidationError
        _person.Password = "nop";
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);

        //ShouldNotHaveValidationError
        _person.Password = "Nop";
        validator.TestValidate(_person).ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Test]
    public void IsValidTestsDigit()
    {
        var validator = new TestValidator();

        var cs = new CustomerSettings
        {
            PasswordMinLength = 3,
            PasswordMaxLength = 32,
            PasswordRequireDigit = true
        };

        validator.RuleFor(x => x.Password).IsPassword(_localizationService, cs);

        //ShouldHaveValidationError
        _person.Password = "nop";
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);

        //ShouldNotHaveValidationError
        _person.Password = "Nop1";
        validator.TestValidate(_person).ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Test]
    public void IsValidTestsNonAlphanumeric()
    {
        var validator = new TestValidator();

        var cs = new CustomerSettings
        {
            PasswordMinLength = 3,
            PasswordMaxLength = 32,
            PasswordRequireNonAlphanumeric = true
        };

        validator.RuleFor(x => x.Password).IsPassword(_localizationService, cs);

        //ShouldHaveValidationError
        _person.Password = "nop";
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);

        //ShouldNotHaveValidationError
        _person.Password = "Nop#";
        validator.TestValidate(_person).ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Test]
    public void IsValidTestsAllRules()
    {
        var validator = new TestValidator();

        //Example:  (?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{6,}$
        validator.RuleFor(x => x.Password).IsPassword(_localizationService, _customerSettings);

        //ShouldHaveValidationError
        _person.Password = string.Empty;
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
        _person.Password = "123";
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
        _person.Password = "12345678";
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
        _person.Password = "nopcommerce";
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
        _person.Password = "nopCommerce";
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
        _person.Password = "nopcommerce123";
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
        _person.Password = "nopCommerce123";
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
        _person.Password = "nopcommerce123$";
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
        _person.Password = "NOPCOMMERCE123$";
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
        _person.Password = "nopCommerce123~";
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
        _person.Password = "nopCommerce123$nopCommerce123$";
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);

        //ShouldNotHaveValidationError
        _person.Password = "nopCommerce123$";
        validator.TestValidate(_person).ShouldNotHaveValidationErrorFor(x => x.Password);
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
        _changePasswordValidator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.NewPassword);
        model.NewPassword = "nopCommerce123$";
        //we know that new password should equal confirmation password
        model.ConfirmNewPassword = model.NewPassword;
        _changePasswordValidator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.NewPassword);
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
        _passwordRecoveryConfirmValidator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.NewPassword);
        model.NewPassword = "nopCommerce123$";
        //we know that new password should equal confirmation password
        model.ConfirmNewPassword = model.NewPassword;
        _passwordRecoveryConfirmValidator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.NewPassword);
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
        _registerValidator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Password);
        model.Password = "nopCommerce123$";
        //we know that password should equal confirmation password
        model.ConfirmPassword = model.Password;
        _registerValidator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Password);
    }
}