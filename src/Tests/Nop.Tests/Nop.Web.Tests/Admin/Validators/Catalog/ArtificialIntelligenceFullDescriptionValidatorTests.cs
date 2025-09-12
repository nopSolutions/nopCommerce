using FluentValidation.TestHelper;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Validators.Catalog;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Admin.Validators.Catalog;

[TestFixture]
public class ArtificialIntelligenceFullDescriptionValidatorTests : BaseNopTest
{
    private ArtificialIntelligenceFullDescriptionValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
        _validator = new ArtificialIntelligenceFullDescriptionValidator(GetService<ILocalizationService>());
    }

    #region ProductName Validation Tests

    [Test]
    public void ShouldHaveErrorWhenProductNameIsNull()
    {
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ProductName = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ProductName);
    }

    [Test]
    public void ShouldHaveErrorWhenProductNameIsEmpty()
    {
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ProductName = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ProductName);
    }

    [Test]
    public void ShouldHaveErrorWhenProductNameIsWhitespace()
    {
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ProductName = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ProductName);
    }

    [Test]
    public void ShouldNotHaveErrorWhenProductNameIsSpecified()
    {
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ProductName = "Sample Product"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ProductName);
    }

    #endregion

    #region Keywords Validation Tests

    [Test]
    public void ShouldHaveErrorWhenKeywordsIsNull()
    {
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            Keywords = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Keywords);
    }

    [Test]
    public void ShouldHaveErrorWhenKeywordsIsEmpty()
    {
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            Keywords = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Keywords);
    }

    [Test]
    public void ShouldHaveErrorWhenKeywordsIsWhitespace()
    {
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            Keywords = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Keywords);
    }

    [Test]
    public void ShouldNotHaveErrorWhenKeywordsIsSpecified()
    {
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            Keywords = "electronics, smartphone, mobile"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Keywords);
    }

    #endregion

    #region CustomToneOfVoice Validation Tests

    [Test]
    public void ShouldHaveErrorWhenCustomToneOfVoiceIsNullAndToneOfVoiceIsCustom()
    {
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ToneOfVoiceId = (int)ToneOfVoiceType.Custom,
            CustomToneOfVoice = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.CustomToneOfVoice);
    }

    [Test]
    public void ShouldHaveErrorWhenCustomToneOfVoiceIsEmptyAndToneOfVoiceIsCustom()
    {
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ToneOfVoiceId = (int)ToneOfVoiceType.Custom,
            CustomToneOfVoice = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.CustomToneOfVoice);
    }

    [Test]
    public void ShouldHaveErrorWhenCustomToneOfVoiceIsWhitespaceAndToneOfVoiceIsCustom()
    {
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ToneOfVoiceId = (int)ToneOfVoiceType.Custom,
            CustomToneOfVoice = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.CustomToneOfVoice);
    }

    [Test]
    public void ShouldNotHaveErrorWhenCustomToneOfVoiceIsSpecifiedAndToneOfVoiceIsCustom()
    {
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ToneOfVoiceId = (int)ToneOfVoiceType.Custom,
            CustomToneOfVoice = "Professional and informative"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.CustomToneOfVoice);
    }

    [Test]
    public void ShouldNotHaveErrorWhenCustomToneOfVoiceIsNullAndToneOfVoiceIsNotCustom()
    {
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ToneOfVoiceId = (int)ToneOfVoiceType.Expert,
            CustomToneOfVoice = null
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.CustomToneOfVoice);
    }

    [Test]
    public void ShouldNotHaveErrorWhenCustomToneOfVoiceIsEmptyAndToneOfVoiceIsNotCustom()
    {
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ToneOfVoiceId = (int)ToneOfVoiceType.Supportive,
            CustomToneOfVoice = string.Empty
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.CustomToneOfVoice);
    }

    #endregion

    #region Combined Validation Tests

    [Test]
    public void ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalid()
    {
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ProductName = null,
            Keywords = string.Empty,
            ToneOfVoiceId = (int)ToneOfVoiceType.Custom,
            CustomToneOfVoice = "   "
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ProductName);
        result.ShouldHaveValidationErrorFor(x => x.Keywords);
        result.ShouldHaveValidationErrorFor(x => x.CustomToneOfVoice);
    }

    [Test]
    public void ShouldNotHaveErrorsWhenAllFieldsAreValidWithCustomTone()
    {
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ProductName = "Smartphone XYZ",
            Keywords = "smartphone, mobile, electronics, communication",
            ToneOfVoiceId = (int)ToneOfVoiceType.Custom,
            CustomToneOfVoice = "Professional and engaging"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.ProductName);
        result.ShouldNotHaveValidationErrorFor(x => x.Keywords);
        result.ShouldNotHaveValidationErrorFor(x => x.CustomToneOfVoice);
    }

    [Test]
    public void ShouldNotHaveErrorsWhenAllFieldsAreValidWithNonCustomTone()
    {
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ProductName = "Laptop ABC",
            Keywords = "laptop, computer, technology, productivity",
            ToneOfVoiceId = (int)ToneOfVoiceType.Expert,
            CustomToneOfVoice = null
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.ProductName);
        result.ShouldNotHaveValidationErrorFor(x => x.Keywords);
        result.ShouldNotHaveValidationErrorFor(x => x.CustomToneOfVoice);
    }

    #endregion
}
