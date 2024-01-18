using FluentValidation.Internal;
using FluentValidation.TestHelper;
using Moq;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Localization;
using Nop.Web.Areas.Admin.Validators.Localization;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Localization;

[TestFixture]
public class LanguageResourceValidatorTests : BaseNopTest
{
    private LanguageResourceValidator _validator;

    [SetUp]
    public void SetUp()
    {
        var localizationServiceMock = new Mock<ILocalizationService>();
        localizationServiceMock.Setup(x => x.GetResourceAsync(It.IsAny<string>())).ReturnsAsync("Mocked message");
        _validator = new LanguageResourceValidator(localizationServiceMock.Object);
    }

    [Test]
    public void ShouldHaveErrorWhenResourceNameIsEmpty()
    {
        var model = new LocaleResourceModel { ResourceName = string.Empty, ResourceValue = "Value" };
        var result = _validator.TestValidate(model, DefaultValidationOptions());
        result.ShouldHaveValidationErrorFor(x => x.ResourceName);
    }

    [Test]
    public void ShouldNotHaveErrorWhenResourceNameIsSpecified()
    {
        var model = new LocaleResourceModel { ResourceName = "Name", ResourceValue = "Value" };
        var result = _validator.TestValidate(model, DefaultValidationOptions());
        result.ShouldNotHaveValidationErrorFor(x => x.ResourceName);
    }

    [Test]
    public void ShouldHaveErrorWhenResourceValueIsNull()
    {
        var model = new LocaleResourceModel { ResourceName = "Name", ResourceValue = null };
        var result = _validator.TestValidate(model, DefaultValidationOptions());
        result.ShouldHaveValidationErrorFor(x => x.ResourceValue);
    }

    [Test]
    public void ShouldNotHaveErrorWhenResourceValueIsSpecified()
    {
        var model = new LocaleResourceModel { ResourceName = "Name", ResourceValue = "Value" };
        var result = _validator.TestValidate(model, DefaultValidationOptions());
        result.ShouldNotHaveValidationErrorFor(x => x.ResourceValue);
    }

    private static Action<ValidationStrategy<LocaleResourceModel>> DefaultValidationOptions()
    {
        return options=>options.IncludeAllRuleSets();
    }
}
