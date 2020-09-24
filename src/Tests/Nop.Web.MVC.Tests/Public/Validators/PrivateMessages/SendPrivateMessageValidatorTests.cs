using FluentValidation.TestHelper;
using Nop.Tests;
using Nop.Web.Models.PrivateMessages;
using Nop.Web.Validators.PrivateMessages;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Public.Validators.PrivateMessages
{
    [TestFixture]
    public class SendPrivateMessageValidatorTests : BaseNopTest
    {
        private SendPrivateMessageValidator _validator;
        
        [SetUp]
        public void Setup()
        {
            _validator = GetService<SendPrivateMessageValidator>();
        }

        [Test]
        public void ShouldHaveErrorWhenSubjectIsNullOrEmpty()
        {
            var model = new SendPrivateMessageModel
            {
                Subject = null
            };
            _validator.ShouldHaveValidationErrorFor(x => x.Subject, model);
            model.Subject = string.Empty;
            _validator.ShouldHaveValidationErrorFor(x => x.Subject, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenSubjectIsSpecified()
        {
            var model = new SendPrivateMessageModel
            {
                Subject = "some comment"
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.Subject, model);
        }

        [Test]
        public void ShouldHaveErrorWhenMessageIsNullOrEmpty()
        {
            var model = new SendPrivateMessageModel
            {
                Message = null
            };
            _validator.ShouldHaveValidationErrorFor(x => x.Message, model);
            model.Message = string.Empty;
            _validator.ShouldHaveValidationErrorFor(x => x.Message, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenMessageIsSpecified()
        {
            var model = new SendPrivateMessageModel
            {
                Message = "some comment"
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.Message, model);
        }
    }
}
