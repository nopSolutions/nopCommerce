using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Models.PrivateMessages;
using Nop.Web.Validators.PrivateMessages;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.PrivateMessages;

[TestFixture]
public class SendPrivateMessageValidatorTests : BaseNopTest
{
    private SendPrivateMessageValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
        _validator = new SendPrivateMessageValidator(GetService<ILocalizationService>());
    }

    [Test]
    public void ShouldHaveErrorWhenSubjectIsNullOrEmpty()
    {
        var model = new SendPrivateMessageModel
        {
            Subject = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Subject);
        model.Subject = string.Empty;
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Subject);
    }

    [Test]
    public void ShouldNotHaveErrorWhenSubjectIsSpecified()
    {
        var model = new SendPrivateMessageModel
        {
            Subject = "some comment"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Subject);
    }

    [Test]
    public void ShouldHaveErrorWhenMessageIsNullOrEmpty()
    {
        var model = new SendPrivateMessageModel
        {
            Message = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Message);
        model.Message = string.Empty;
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Message);
    }

    [Test]
    public void ShouldNotHaveErrorWhenMessageIsSpecified()
    {
        var model = new SendPrivateMessageModel
        {
            Message = "some comment"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Message);
    }
}