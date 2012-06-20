using FluentValidation.TestHelper;
using Nop.Web.Infrastructure.Installation;
using Nop.Web.Models.Install;
using Nop.Web.Validators.Install;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Web.MVC.Tests.Public.Validators.Install
{
    [TestFixture]
    public class InstallValidatorTests : BaseValidatorTests
    {
        protected IInstallationLocalizationService _ilService;
        private InstallValidator _validator;

        [SetUp]
        public new void Setup()
        {
            //set up localziation service used by almost all validators
            _ilService = MockRepository.GenerateMock<IInstallationLocalizationService>();
            _ilService.Expect(l => l.GetResource("")).Return("Invalid").IgnoreArguments();

            _validator = new InstallValidator(_ilService);
        }
        
        [Test]
        public void Should_have_error_when_adminEmail_is_null_or_empty()
        {
            var model = new InstallModel();
            model.AdminEmail = null;
            _validator.ShouldHaveValidationErrorFor(x => x.AdminEmail, model);
            model.AdminEmail = "";
            _validator.ShouldHaveValidationErrorFor(x => x.AdminEmail, model);
        }

        [Test]
        public void Should_have_error_when_adminEmail_is_wrong_format()
        {
            var model = new InstallModel();
            model.AdminEmail = "adminexample.com";
            _validator.ShouldHaveValidationErrorFor(x => x.AdminEmail, model);
        }

        [Test]
        public void Should_not_have_error_when_adminEmail_is_correct_format()
        {
            var model = new InstallModel();
            model.AdminEmail = "admin@example.com";
            _validator.ShouldNotHaveValidationErrorFor(x => x.AdminEmail, model);
        }

        [Test]
        public void Should_have_error_when_password_is_null_or_empty()
        {
            var model = new InstallModel();
            model.AdminPassword = null;
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
            var model = new InstallModel();
            model.AdminPassword = "password";
            //we know that password should equal confirmation password
            model.ConfirmPassword = model.AdminPassword;
            _validator.ShouldNotHaveValidationErrorFor(x => x.AdminPassword, model);
        }

        [Test]
        public void Should_have_error_when_confirmPassword_is_null_or_empty()
        {
            var model = new InstallModel();
            model.ConfirmPassword = null;
            _validator.ShouldHaveValidationErrorFor(x => x.ConfirmPassword, model);
            model.ConfirmPassword = "";
            _validator.ShouldHaveValidationErrorFor(x => x.ConfirmPassword, model);
        }

        [Test]
        public void Should_not_have_error_when_confirmPassword_is_specified()
        {
            var model = new InstallModel();
            model.ConfirmPassword = "some password";
            _validator.ShouldNotHaveValidationErrorFor(x => x.ConfirmPassword, model);
        }

        [Test]
        public void Should_have_error_when_password_doesnot_equal_confirmationPassword()
        {
            var model = new InstallModel();
            model.AdminPassword = "some password";
            model.ConfirmPassword = "another password";
            _validator.ShouldHaveValidationErrorFor(x => x.AdminPassword, model);
        }

        [Test]
        public void Should_not_have_error_when_password_equals_confirmationPassword()
        {
            var model = new InstallModel();
            model.AdminPassword = "some password";
            model.ConfirmPassword = "some password";
            _validator.ShouldNotHaveValidationErrorFor(x => x.AdminPassword, model);
        }
    }
}
