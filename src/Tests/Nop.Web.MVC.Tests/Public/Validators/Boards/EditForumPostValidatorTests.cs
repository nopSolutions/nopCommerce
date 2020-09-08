using FluentValidation.TestHelper;
using Nop.Tests;
using Nop.Web.Models.Boards;
using Nop.Web.Validators.Boards;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Public.Validators.Boards
{
    [TestFixture]
    public class EditForumPostValidatorTests : BaseNopTest
    {
        private EditForumPostValidator _validator;
        
        [SetUp]
        public void Setup()
        {
            _validator = GetService<EditForumPostValidator>();
        }
        
        [Test]
        public void ShouldHaveErrorWhenTextIsNullOrEmpty()
        {
            var model = new EditForumPostModel
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
            var model = new EditForumPostModel
            {
                Text = "some comment"
            };
            _validator.ShouldNotHaveValidationErrorFor(x => x.Text, model);
        }
    }
}
