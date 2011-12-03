using FluentValidation.TestHelper;
using Nop.Web.Models.PrivateMessages;
using Nop.Web.Validators.PrivateMessages;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Public.Validators.PrivateMessages
{
    [TestFixture]
    public class PrivateMessageValidatorTests : BaseValidatorTests
    {
        private PrivateMessageValidator _validator;
        
        [SetUp]
        public void Setup()
        {
            _validator = new PrivateMessageValidator(_localizationService);
        }

        [Test]
        public void Should_have_error_when_subject_is_null_or_empty()
        {
            var model = new PrivateMessageModel();
            model.Subject = null;
            _validator.ShouldHaveValidationErrorFor(x => x.Subject, model);
            model.Subject = "";
            _validator.ShouldHaveValidationErrorFor(x => x.Subject, model);
        }

        [Test]
        public void Should_not_have_error_when_subject_is_specified()
        {
            var model = new PrivateMessageModel();
            model.Subject = "some comment";
            _validator.ShouldNotHaveValidationErrorFor(x => x.Subject, model);
        }

        [Test]
        public void Should_have_error_when_message_is_null_or_empty()
        {
            var model = new PrivateMessageModel();
            model.Message = null;
            _validator.ShouldHaveValidationErrorFor(x => x.Message, model);
            model.Message = "";
            _validator.ShouldHaveValidationErrorFor(x => x.Message, model);
        }

        [Test]
        public void Should_not_have_error_when_message_is_specified()
        {
            var model = new PrivateMessageModel();
            model.Message = "some comment";
            _validator.ShouldNotHaveValidationErrorFor(x => x.Message, model);
        }
    }
}
