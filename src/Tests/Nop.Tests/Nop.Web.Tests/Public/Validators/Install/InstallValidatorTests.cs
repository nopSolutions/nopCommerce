using FluentValidation.TestHelper;
using Nop.Web.Models.Install;
using Nop.Web.Validators.Install;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Install
{
    [TestFixture]
    public class InstallValidatorTests : BaseNopTest
    {
        private InstallValidator _validator;

        [OneTimeSetUp]
        public void Setup()
        {
            _validator = GetService<InstallValidator>();
        }
        
        [Test]
        public void ShouldHaveErrorWhenAdminEmailIsNullOrEmpty()
        {
            var model = new InstallModel
            {
                AdminEmail = null
            };
            _validator.ShouldHaveValidationErrorFor(x => x.AdminEmail, model);
            model.AdminEmail = string.Empty;
            _validator.ShouldHaveValidationErrorFor(x => x.AdminEmail, model);
        }

        [Test]
        public void ShouldHaveErrorWhenAdminEmailIsWrongFormat()
        {
            var model = new InstallModel
            {
                AdminEmail = "adminexample.com"
            };
            _validator.ShouldHaveValidationErrorFor(x => x.AdminEmail, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenAdminEmailIsCorrectFormat()
        {
            var model = new InstallModel
            {
                AdminEmail = "admin@example.com"
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.AdminEmail, model);
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
            _validator.ShouldHaveValidationErrorFor(x => x.AdminPassword, model);
            model.AdminPassword = string.Empty;
            //we know that password should equal confirmation password
            model.ConfirmPassword = model.AdminPassword;
            _validator.ShouldHaveValidationErrorFor(x => x.AdminPassword, model);
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
            _validator.ShouldNotHaveValidationErrorFor(x => x.AdminPassword, model);
        }

        [Test]
        public void ShouldHaveErrorWhenConfirmPasswordIsNullOrEmpty()
        {
            var model = new InstallModel
            {
                ConfirmPassword = null
            };
            _validator.ShouldHaveValidationErrorFor(x => x.ConfirmPassword, model);
            model.ConfirmPassword = string.Empty;
            _validator.ShouldHaveValidationErrorFor(x => x.ConfirmPassword, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenConfirmPasswordIsSpecified()
        {
            var model = new InstallModel
            {
                ConfirmPassword = "some password"
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.ConfirmPassword, model);
        }

        [Test]
        public void ShouldHaveErrorWhenPasswordDoesNotEqualConfirmationPassword()
        {
            var model = new InstallModel
            {
                AdminPassword = "some password",
                ConfirmPassword = "another password"
            };
            _validator.ShouldHaveValidationErrorFor(x => x.AdminPassword, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenPasswordEqualsConfirmationPassword()
        {
            var model = new InstallModel
            {
                AdminPassword = "some password",
                ConfirmPassword = "some password"
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.AdminPassword, model);
        }
    }
}
