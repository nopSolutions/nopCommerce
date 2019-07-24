using FluentValidation.TestHelper;
using Moq;
using Nop.Web.Infrastructure.Installation;
using Nop.Web.Models.Install;
using Nop.Web.Validators.Install;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Public.Validators.Install
{
    [TestFixture]
    public class InstallValidatorTests : BaseValidatorTests
    {
        protected Mock<IInstallationLocalizationService> _ilService;
        private InstallValidator _validator;

        [SetUp]
        public new void Setup()
        {
            //set up localziation service used by almost all validators
            _ilService = new Mock<IInstallationLocalizationService>();
            _ilService.Setup(l => l.GetResource(It.IsAny<string>())).Returns("Invalid");

            _validator = new InstallValidator(_ilService.Object);
        }
        
        [Test]
        public void Should_have_error_when_adminEmail_is_null_or_empty()
        {
            var model = new InstallModel
            {
                AdminEmail = null
            };
            _validator.ShouldHaveValidationErrorFor(x => x.AdminEmail, model);
            model.AdminEmail = "";
            _validator.ShouldHaveValidationErrorFor(x => x.AdminEmail, model);
        }

        [Test]
        public void Should_have_error_when_adminEmail_is_wrong_format()
        {
            var model = new InstallModel
            {
                AdminEmail = "adminexample.com"
            };
            _validator.ShouldHaveValidationErrorFor(x => x.AdminEmail, model);
        }

        [Test]
        public void Should_not_have_error_when_adminEmail_is_correct_format()
        {
            var model = new InstallModel
            {
                AdminEmail = "admin@example.com"
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.AdminEmail, model);
        }

        [Test]
        public void Should_have_error_when_password_is_null_or_empty()
        {
            var model = new InstallModel
            {
                AdminPassword = null
            };
            //we know that password should equal confirmation password
            model.ConfirmPassword = model.AdminPassword;
            _validator.ShouldHaveValidationErrorFor(x => x.AdminPassword, model);
            model.AdminPassword = "";
            //we know that password should equal confirmation password
            model.ConfirmPassword = model.AdminPassword;
            _validator.ShouldHaveValidationErrorFor(x => x.AdminPassword, model);
        }

        [Test]
        public void Should_not_have_error_when_password_is_specified()
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
        public void Should_have_error_when_confirmPassword_is_null_or_empty()
        {
            var model = new InstallModel
            {
                ConfirmPassword = null
            };
            _validator.ShouldHaveValidationErrorFor(x => x.ConfirmPassword, model);
            model.ConfirmPassword = "";
            _validator.ShouldHaveValidationErrorFor(x => x.ConfirmPassword, model);
        }

        [Test]
        public void Should_not_have_error_when_confirmPassword_is_specified()
        {
            var model = new InstallModel
            {
                ConfirmPassword = "some password"
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.ConfirmPassword, model);
        }

        [Test]
        public void Should_have_error_when_password_doesnot_equal_confirmationPassword()
        {
            var model = new InstallModel
            {
                AdminPassword = "some password",
                ConfirmPassword = "another password"
            };
            _validator.ShouldHaveValidationErrorFor(x => x.AdminPassword, model);
        }

        [Test]
        public void Should_not_have_error_when_password_equals_confirmationPassword()
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
