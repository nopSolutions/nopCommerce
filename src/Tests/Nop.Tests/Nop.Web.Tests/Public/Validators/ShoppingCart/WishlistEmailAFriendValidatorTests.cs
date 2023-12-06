using FluentValidation.TestHelper;
using Nop.Web.Models.ShoppingCart;
using Nop.Web.Validators.ShoppingCart;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.ShoppingCart
{
    [TestFixture]
    public class WishlistEmailAFriendValidatorTests : BaseNopTest
    {
        private WishlistEmailAFriendValidator _validator;

        [OneTimeSetUp]
        public void Setup()
        {
            _validator = GetService<WishlistEmailAFriendValidator>();
        }

        [Test]
        public void ShouldHaveErrorWhenFriendEmailIsNullOrEmpty()
        {
            var model = new WishlistEmailAFriendModel
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
            var model = new WishlistEmailAFriendModel
            {
                FriendEmail = "adminexample.com"
            };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FriendEmail);
        }

        [Test]
        public void ShouldNotHaveErrorWhenFriendEmailIsCorrectFormat()
        {
            var model = new WishlistEmailAFriendModel
            {
                FriendEmail = "admin@example.com"
            };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.FriendEmail);
        }

        [Test]
        public void ShouldHaveErrorWhenYourEmailAddressIsNullOrEmpty()
        {
            var model = new WishlistEmailAFriendModel
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
            var model = new WishlistEmailAFriendModel
            {
                YourEmailAddress = "adminexample.com"
            };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.YourEmailAddress);
        }

        [Test]
        public void ShouldNotHaveErrorWhenYourEmailAddressIsCorrectFormat()
        {
            var model = new WishlistEmailAFriendModel
            {
                YourEmailAddress = "admin@example.com"
            };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.YourEmailAddress);
        }
    }
}
