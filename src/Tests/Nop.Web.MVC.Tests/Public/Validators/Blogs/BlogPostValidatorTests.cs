using FluentValidation.TestHelper;
using Nop.Tests;
using Nop.Web.Models.Blogs;
using Nop.Web.Validators.Blogs;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Public.Validators.Blogs
{
    [TestFixture]
    public class BlogPostValidatorTests : BaseNopTest
    {
        private BlogPostValidator _validator;
        
        [SetUp]
        public void Setup()
        {
            _validator = GetService<BlogPostValidator>();
        }

        [Test]
        public void ShouldHaveErrorWhenCommentIsNullOrEmpty()
        {
            var model = new BlogPostModel { AddNewComment = { CommentText = null } };
            _validator.ShouldHaveValidationErrorFor(x => x.AddNewComment.CommentText, model);
            model.AddNewComment.CommentText = string.Empty;
            _validator.ShouldHaveValidationErrorFor(x => x.AddNewComment.CommentText, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenCommentIsSpecified()
        {
            var model = new BlogPostModel { AddNewComment = { CommentText = "some comment" } };
            _validator.ShouldNotHaveValidationErrorFor(x => x.AddNewComment.CommentText, model);
        }
    }
}
