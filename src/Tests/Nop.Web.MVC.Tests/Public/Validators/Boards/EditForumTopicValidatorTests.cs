using FluentValidation.TestHelper;
using Nop.Tests;
using Nop.Web.Models.Boards;
using Nop.Web.Validators.Boards;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Public.Validators.Boards
{
    [TestFixture]
    public class EditForumTopicValidatorTests : BaseNopTest
    {
        private EditForumTopicValidator _validator;
        
        [SetUp]
        public void Setup()
        {
            _validator = GetService<EditForumTopicValidator>();
        }

        [Test]
        public void ShouldHaveErrorWhenSubjectIsNullOrEmpty()
        {
            var model = new EditForumTopicModel
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
            var model = new EditForumTopicModel
            {
                Subject = "some comment"
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.Subject, model);
        }

        [Test]
        public void ShouldHaveErrorWhenTextIsNullOrEmpty()
        {
            var model = new EditForumTopicModel
            {
                Text = null
            };
            _validator.ShouldHaveValidationErrorFor(x => x.Text, model);
            model.Text = string.Empty;
            _validator.ShouldHaveValidationErrorFor(x => x.Text, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenTextIsSpecified()
        {
            var model = new EditForumTopicModel
            {
                Text = "some comment"
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.Text, model);
        }
    }
}
