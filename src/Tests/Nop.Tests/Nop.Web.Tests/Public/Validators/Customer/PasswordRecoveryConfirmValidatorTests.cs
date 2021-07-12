using FluentValidation.TestHelper;
using Nop.Web.Models.Customer;
using Nop.Web.Validators.Customer;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Customer
{
    [TestFixture]
    public class PasswordRecoveryConfirmValidatorTests : BaseNopTest
    {
        private PasswordRecoveryConfirmValidator _validator;
        
        [OneTimeSetUp]
        public void Setup()
        {
            _validator = GetService<PasswordRecoveryConfirmValidator>();
        }

        [Test]
        public void ShouldHaveErrorWhenNewPasswordIsNullOrEmpty()
        {
            var model = new PasswordRecoveryConfirmModel
            {
                NewPassword = null
            };
            //we know that new password should equal confirmation password
            model.ConfirmNewPassword = model.NewPassword;
            _validator.ShouldHaveValidationErrorFor(x => x.NewPassword, model);
            model.NewPassword = string.Empty;
            //we know that new password should equal confirmation password
            model.ConfirmNewPassword = model.NewPassword;
            _validator.ShouldHaveValidationErrorFor(x => x.NewPassword, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenNewPasswordIsSpecified()
        {
            var model = new PasswordRecoveryConfirmModel
            {
                NewPassword = "new password"
            };
            //we know that new password should equal confirmation password
            model.ConfirmNewPassword = model.NewPassword;
            _validator.ShouldNotHaveValidationErrorFor(x => x.NewPassword, model);
        }

        [Test]
        public void ShouldHaveErrorWhenConfirmNewPasswordIsNullOrEmpty()
        {
            var model = new PasswordRecoveryConfirmModel
            {
                ConfirmNewPassword = null
            };
            _validator.ShouldHaveValidationErrorFor(x => x.ConfirmNewPassword, model);
            model.ConfirmNewPassword = string.Empty;
            _validator.ShouldHaveValidationErrorFor(x => x.ConfirmNewPassword, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenConfirmNewPasswordIsSpecified()
        {
            var model = new PasswordRecoveryConfirmModel
            {
                ConfirmNewPassword = "some password"
            };
            //we know that new password should equal confirmation password
            model.NewPassword = model.ConfirmNewPassword;
            _validator.ShouldNotHaveValidationErrorFor(x => x.ConfirmNewPassword, model);
        }

        [Test]
        public void ShouldHaveErrorWhenNewPasswordDoesNotEqualConfirmationPassword()
        {
            var model = new PasswordRecoveryConfirmModel
            {
                NewPassword = "some password",
                ConfirmNewPassword = "another password"
            };
            _validator.ShouldHaveValidationErrorFor(x => x.ConfirmNewPassword, model);
        }

        [Test]
        public void Should_not_have_error_when_newPassword_equals_confirmationPassword()
        {
            var model = new PasswordRecoveryConfirmModel
            {
                NewPassword = "some password",
                ConfirmNewPassword = "some password"
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.NewPassword, model);
        }                
    }
}
