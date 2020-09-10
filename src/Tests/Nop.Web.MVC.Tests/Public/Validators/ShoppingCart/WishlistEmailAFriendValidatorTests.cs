using FluentValidation.TestHelper;
using Nop.Tests;
using Nop.Web.Models.ShoppingCart;
using Nop.Web.Validators.ShoppingCart;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Public.Validators.ShoppingCart
{
    [TestFixture]
    public class WishlistEmailAFriendValidatorTests : BaseNopTest
    {
        private WishlistEmailAFriendValidator _validator;
        
        [SetUp]
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
            _validator.ShouldHaveValidationErrorFor(x => x.FriendEmail, model);
            model.FriendEmail = string.Empty;
            _validator.ShouldHaveValidationErrorFor(x => x.FriendEmail, model);
        }

        [Test]
        public void ShouldHaveErrorWhenFriendEmailIsWrongFormat()
        {
            var model = new WishlistEmailAFriendModel
            {
                FriendEmail = "adminexample.com"
            };
            _validator.ShouldHaveValidationErrorFor(x => x.FriendEmail, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenFriendEmailIsCorrectFormat()
        {
            var model = new WishlistEmailAFriendModel
            {
                FriendEmail = "admin@example.com"
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.FriendEmail, model);
        }

        [Test]
        public void ShouldHaveErrorWhenYourEmailAddressIsNullOrEmpty()
        {
            var model = new WishlistEmailAFriendModel
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
            var model = new WishlistEmailAFriendModel
            {
                YourEmailAddress = "adminexample.com"
            };
            _validator.ShouldHaveValidationErrorFor(x => x.YourEmailAddress, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenYourEmailAddressIsCorrectFormat()
        {
            var model = new WishlistEmailAFriendModel
            {
                YourEmailAddress = "admin@example.com"
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.YourEmailAddress, model);
        }
    }
}
