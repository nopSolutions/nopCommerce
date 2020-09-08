using FluentValidation.TestHelper;
using Nop.Tests;
using Nop.Web.Models.Catalog;
using Nop.Web.Validators.Catalog;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Public.Validators.Catalog
{
    [TestFixture]
    public class ProductEmailAFriendValidatorTests : BaseNopTest
    {
        private ProductEmailAFriendValidator _validator;
        
        [SetUp]
        public void Setup()
        {
            _validator = GetService<ProductEmailAFriendValidator>();
        }
        
        [Test]
        public void ShouldHaveErrorWhenFriendEmailIsNullOrEmpty()
        {
            var model = new ProductEmailAFriendModel
            {
                FriendEmail = null
            };
            _validator.ShouldHaveValidationErrorFor(x => x.FriendEmail, model);
            model.FriendEmail = string.Empty;
            _validator.ShouldHaveValidationErrorFor(x => x.FriendEmail, model);
        }

        [Test]
        public void ShouldHaveErrorWhenFriendEmailIsWrongFormat()
        {
            var model = new ProductEmailAFriendModel
            {
                FriendEmail = "adminexample.com"
            };
            _validator.ShouldHaveValidationErrorFor(x => x.FriendEmail, model);
        }

        [Test]
        public void PublicVoidShouldNotHaveErrorWhenFriendEmailIsCorrectFormat()
        {
            var model = new ProductEmailAFriendModel
            {
                FriendEmail = "admin@example.com"
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.FriendEmail, model);
        }

        [Test]
        public void ShouldHaveErrorWhenYourEmailAddressIsNullOrEmpty()
        {
            var model = new ProductEmailAFriendModel
            {
                YourEmailAddress = null
            };
            _validator.ShouldHaveValidationErrorFor(x => x.YourEmailAddress, model);
            model.YourEmailAddress = string.Empty;
            _validator.ShouldHaveValidationErrorFor(x => x.YourEmailAddress, model);
        }

        [Test]
        public void ShouldHaveErrorWhenYourEmailAddressIsWrongFormat()
        {
            var model = new ProductEmailAFriendModel
            {
                YourEmailAddress = "adminexample.com"
            };
            _validator.ShouldHaveValidationErrorFor(x => x.YourEmailAddress, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenYourEmailAddressIsCorrectFormat()
        {
            var model = new ProductEmailAFriendModel
            {
                YourEmailAddress = "admin@example.com"
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.YourEmailAddress, model);
        }
    }
}
