using FluentValidation.TestHelper;
using Nop.Tests;
using Nop.Web.Models.Customer;
using Nop.Web.Validators.Customer;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Public.Validators.Customer
{
    [TestFixture]
    public class PasswordRecoveryValidatorTests : BaseNopTest
    {
        private PasswordRecoveryValidator _validator;
        
        [SetUp]
        public void Setup()
        {
            _validator = GetService<PasswordRecoveryValidator>();
        }
        
        [Test]
        public void ShouldHaveErrorWhenEmailIsNullOrEmpty()
        {
            var model = new PasswordRecoveryModel
            {
                Email = null
            };
            _validator.ShouldHaveValidationErrorFor(x => x.Email, model);
            model.Email = string.Empty;
            _validator.ShouldHaveValidationErrorFor(x => x.Email, model);
        }

        [Test]
        public void ShouldHaveErrorWhenEmailIsWrongFormat()
        {
            var model = new PasswordRecoveryModel
            {
                Email = "adminexample.com"
            };
            _validator.ShouldHaveValidationErrorFor(x => x.Email, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenEmailIsCorrectFormat()
        {
            var model = new PasswordRecoveryModel
            {
                Email = "admin@example.com"
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.Email, model);
        }
    }
}
