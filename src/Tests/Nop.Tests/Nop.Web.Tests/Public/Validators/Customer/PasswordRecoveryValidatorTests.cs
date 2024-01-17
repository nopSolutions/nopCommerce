using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Models.Customer;
using Nop.Web.Validators.Customer;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Customer;

[TestFixture]
public class PasswordRecoveryValidatorTests : BaseNopTest
{
    private PasswordRecoveryValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
        _validator = new PasswordRecoveryValidator(GetService<ILocalizationService>());
    }

    [Test]
    public void ShouldHaveErrorWhenEmailIsNullOrEmpty()
    {
        var model = new PasswordRecoveryModel
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
        var model = new PasswordRecoveryModel
        {
            Email = "adminexample.com"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Test]
    public void ShouldNotHaveErrorWhenEmailIsCorrectFormat()
    {
        var model = new PasswordRecoveryModel
        {
            Email = "admin@example.com"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Email);
    }
}