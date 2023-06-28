using FluentValidation.TestHelper;
using Nop.Web.Models.Blogs;
using Nop.Web.Validators.Blogs;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Blogs
{
    [TestFixture]
    public class BlogPostValidatorTests : BaseNopTest
    {
        private BlogPostValidator _validator;

        [OneTimeSetUp]
        public void Setup()
        {
            _validator = GetService<BlogPostValidator>();
        }

        [Test]
        public void ShouldHaveErrorWhenCommentIsNullOrEmpty()
        {
            var model = new BlogPostModel { AddNewComment = { CommentText = null } };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AddNewComment.CommentText);
            model.AddNewComment.CommentText = string.Empty;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AddNewComment.CommentText);
        }

        [Test]
        public void ShouldNotHaveErrorWhenCommentIsSpecified()
        {
            var model = new BlogPostModel { AddNewComment = { CommentText = "some comment" } };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AddNewComment.CommentText);
        }
    }
}
