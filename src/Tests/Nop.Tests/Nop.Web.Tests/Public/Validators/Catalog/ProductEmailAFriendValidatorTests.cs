using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Models.Catalog;
using Nop.Web.Validators.Catalog;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Catalog;

[TestFixture]
public class ProductEmailAFriendValidatorTests : BaseNopTest
{
    private ProductEmailAFriendValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
        _validator = new ProductEmailAFriendValidator(GetService<ILocalizationService>());
    }

    [Test]
    public void ShouldHaveErrorWhenFriendEmailIsNullOrEmpty()
    {
        var model = new ProductEmailAFriendModel
        {
            FriendEmail = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FriendEmail);
        model.FriendEmail = string.Empty;
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FriendEmail);
    }

    [Test]
    public void ShouldHaveErrorWhenFriendEmailIsWrongFormat()
    {
        var model = new ProductEmailAFriendModel
        {
            FriendEmail = "adminexample.com"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FriendEmail);
    }

    [Test]
    public void PublicVoidShouldNotHaveErrorWhenFriendEmailIsCorrectFormat()
    {
        var model = new ProductEmailAFriendModel
        {
            FriendEmail = "admin@example.com"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.FriendEmail);
    }

    [Test]
    public void ShouldHaveErrorWhenYourEmailAddressIsNullOrEmpty()
    {
        var model = new ProductEmailAFriendModel
        {
            YourEmailAddress = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.YourEmailAddress);
        model.YourEmailAddress = string.Empty;
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.YourEmailAddress);
    }

    [Test]
    public void ShouldHaveErrorWhenYourEmailAddressIsWrongFormat()
    {
        var model = new ProductEmailAFriendModel
        {
            YourEmailAddress = "adminexample.com"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.YourEmailAddress);
    }

    [Test]
    public void ShouldNotHaveErrorWhenYourEmailAddressIsCorrectFormat()
    {
        var model = new ProductEmailAFriendModel
        {
            YourEmailAddress = "admin@example.com"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.YourEmailAddress);
    }
}