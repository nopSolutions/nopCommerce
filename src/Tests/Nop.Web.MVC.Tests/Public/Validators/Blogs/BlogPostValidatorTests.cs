using FluentValidation;
using FluentValidation.TestHelper;
using Nop.Tests;
using Nop.Web.Models.Blogs;
using Nop.Web.Validators.Blogs;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Public.Validators.Blogs
{
    [TestFixture]
    public class BlogPostValidatorTests : BaseValidatorTests
    {
        private BlogPostValidator _validator;
        
        [SetUp]
        public void Setup()
        {
            _validator = new BlogPostValidator(_localizationService);
        }

        [Test]
        public void Should_have_error_when_comment_is_null()
        {
            var model = new BlogPostModel();
            //null
            model.AddNewComment.CommentText = null;
            _validator.Validate(model).IsValid.ShouldBeFalse();
            //empty
            model.AddNewComment.CommentText = "";
            _validator.Validate(model).IsValid.ShouldBeFalse();
            //we cannot use better approach for nested properties (commented below)
            //model.AddNewComment.CommentText = null;
            //_validator.ShouldHaveValidationErrorFor(x => x.AddNewComment.CommentText, model);
            //model.AddNewComment.CommentText = "";
            //_validator.ShouldHaveValidationErrorFor(x => x.AddNewComment.CommentText, model);
        }

        [Test]
        public void Should_not_have_error_when_comment_is_specified()
        {
            var model = new BlogPostModel();
            //specified
            model.AddNewComment.CommentText = "some comment";
            _validator.Validate(model).IsValid.ShouldBeTrue();


            //we cannot use better approach for nested properties (commented below)
            //model.AddNewComment.CommentText = "some comment";
            //_validator.ShouldNotHaveValidationErrorFor(x => x.AddNewComment.CommentText, model);

        }
    }
}
