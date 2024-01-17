using FluentValidation.TestHelper;
using Nop.Web.Infrastructure.Installation;
using Nop.Web.Models.Install;
using Nop.Web.Validators.Install;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Install;

[TestFixture]
public class InstallValidatorTests : BaseNopTest
{
    private InstallValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
        _validator = new InstallValidator(GetService<IInstallationLocalizationService>());
    }

    [Test]
    public void ShouldHaveErrorWhenAdminEmailIsNullOrEmpty()
    {
        var model = new InstallModel
        {
            AdminEmail = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AdminEmail);
        model.AdminEmail = string.Empty;
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AdminEmail);
    }

    [Test]
    public void ShouldHaveErrorWhenAdminEmailIsWrongFormat()
    {
        var model = new InstallModel
        {
            AdminEmail = "adminexample.com"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AdminEmail);
    }

    [Test]
    public void ShouldNotHaveErrorWhenAdminEmailIsCorrectFormat()
    {
        var model = new InstallModel
        {
            AdminEmail = "admin@example.com"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AdminEmail);
    }

    [Test]
    public void ShouldHaveErrorWhenPasswordIsNullOrEmpty()
    {
        var model = new InstallModel
        {
            AdminPassword = null
        };
        //we know that password should equal confirmation password
        model.ConfirmPassword = model.AdminPassword;
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AdminPassword);
        model.AdminPassword = string.Empty;
        //we know that password should equal confirmation password
        model.ConfirmPassword = model.AdminPassword;
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AdminPassword);
    }

    [Test]
    public void ShouldNotHaveErrorWhenPasswordIsSpecified()
    {
        var model = new InstallModel
        {
            AdminPassword = "password"
        };
        //we know that password should equal confirmation password
        model.ConfirmPassword = model.AdminPassword;
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AdminPassword);
    }

    [Test]
    public void ShouldHaveErrorWhenConfirmPasswordIsNullOrEmpty()
    {
        var model = new InstallModel
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
        var model = new InstallModel
        {
            ConfirmPassword = "some password"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ConfirmPassword);
    }

    [Test]
    public void ShouldHaveErrorWhenPasswordDoesNotEqualConfirmationPassword()
    {
        var model = new InstallModel
        {
            AdminPassword = "some password",
            ConfirmPassword = "another password"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AdminPassword);
    }

    [Test]
    public void ShouldNotHaveErrorWhenPasswordEqualsConfirmationPassword()
    {
        var model = new InstallModel
        {
            AdminPassword = "some password",
            ConfirmPassword = "some password"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AdminPassword);
    }
}