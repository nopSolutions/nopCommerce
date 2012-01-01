using FluentValidation.TestHelper;
using Nop.Web.Models.Boards;
using Nop.Web.Validators.Boards;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Public.Validators.Boards
{
    [TestFixture]
    public class EditForumPostValidatorTests : BaseValidatorTests
    {
        private EditForumPostValidator _validator;
        
        [SetUp]
        public new void Setup()
        {
            _validator = new EditForumPostValidator(_localizationService);
        }
        
        [Test]
        public void Should_have_error_when_text_is_null_or_empty()
        {
            var model = new EditForumPostModel();
            model.Text = null;
            _validator.ShouldHaveValidationErrorFor(x => x.Text, model);
            model.Text = "";
            _validator.ShouldHaveValidationErrorFor(x => x.Text, model);
        }

        [Test]
        public void Should_not_have_error_when_text_is_specified()
        {
            var model = new EditForumPostModel();
            model.Text = "some comment";
            _validator.ShouldNotHaveValidationErrorFor(x => x.Text, model);
        }
    }
}
