using FluentValidation.TestHelper;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Models.Customer;
using Nop.Web.Validators.Customer;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Customer;

[TestFixture]
public class ChangePasswordValidatorTests : BaseNopTest
{
    private ChangePasswordValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
        _validator = new ChangePasswordValidator(GetService<ILocalizationService>(), GetService<CustomerSettings>()) ;
    }

    [Test]
    public void ShouldHaveErrorWhenOldPasswordIsNullOrEmpty()
    {
        var model = new ChangePasswordModel
        {
            OldPassword = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.OldPassword);
        model.OldPassword = string.Empty;
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.OldPassword);
    }

    [Test]
    public void ShouldNotHaveErrorWhenOldPasswordIsSpecified()
    {
        var model = new ChangePasswordModel
        {
            OldPassword = "old password"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.OldPassword);
    }

    [Test]
    public void ShouldHaveErrorWhenNewPasswordIsNullOrEmpty()
    {
        var model = new ChangePasswordModel
        {
            NewPassword = null
        };
        //we know that new password should equal confirmation password
        model.ConfirmNewPassword = model.NewPassword;
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.NewPassword);
        model.NewPassword = string.Empty;
        //we know that new password should equal confirmation password
        model.ConfirmNewPassword = model.NewPassword;
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.NewPassword);
    }

    [Test]
    public void ShouldNotHaveErrorWhenNewPasswordIsSpecified()
    {
        var model = new ChangePasswordModel
        {
            NewPassword = "new password"
        };
        //we know that new password should equal confirmation password
        model.ConfirmNewPassword = model.NewPassword;
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.NewPassword);
    }

    [Test]
    public void ShouldHaveErrorWhenConfirmNewPasswordIsNullOrEmpty()
    {
        var model = new ChangePasswordModel
        {
            ConfirmNewPassword = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ConfirmNewPassword);
        model.ConfirmNewPassword = string.Empty;
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ConfirmNewPassword);
    }

    [Test]
    public void ShouldNotHaveErrorWhenConfirmNewPasswordIsSpecified()
    {
        var model = new ChangePasswordModel
        {
            ConfirmNewPassword = "some password"
        };
        //we know that new password should equal confirmation password
        model.NewPassword = model.ConfirmNewPassword;
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ConfirmNewPassword);
    }

    [Test]
    public void ShouldHaveErrorWhenNewPasswordDoesNotEqualConfirmationPassword()
    {
        var model = new ChangePasswordModel
        {
            NewPassword = "some password",
            ConfirmNewPassword = "another password"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ConfirmNewPassword);
    }

    [Test]
    public void ShouldNotHaveErrorWhenNewPasswordEqualsConfirmationPassword()
    {
        var model = new ChangePasswordModel
        {
            NewPassword = "some password",
            ConfirmNewPassword = "some password"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.NewPassword);
    }
}