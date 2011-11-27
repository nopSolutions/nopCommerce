using FluentValidation.TestHelper;
using Nop.Web.Models.Newsletter;
using Nop.Web.Validators.Newsletter;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Public.Validators.Newsletter
{
    [TestFixture]
    public class NewsletterBoxValidatorTests : BaseValidatorTests
    {
        private NewsletterBoxValidator _validator;
        
        [SetUp]
        public void Setup()
        {
            _validator = new NewsletterBoxValidator(_localizationService);
        }
        
        [Test]
        public void Should_have_error_when_email_is_null_or_empty()
        {
            var model = new NewsletterBoxModel();
            model.Email = null;
            _validator.ShouldHaveValidationErrorFor(x => x.Email, model);
            model.Email = "";
            _validator.ShouldHaveValidationErrorFor(x => x.Email, model);
        }

        [Test]
        public void Should_have_error_when_email_is_wrong_format()
        {
            var model = new NewsletterBoxModel();
            model.Email = "adminexample.com";
            _validator.ShouldHaveValidationErrorFor(x => x.Email, model);
        }

        [Test]
        public void Should_not_have_error_when_email_is_correct_format()
        {
            var model = new NewsletterBoxModel();
            model.Email = "admin@example.com";
            _validator.ShouldNotHaveValidationErrorFor(x => x.Email, model);
        }
    }
}
