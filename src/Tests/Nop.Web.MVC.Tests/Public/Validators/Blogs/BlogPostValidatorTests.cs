using Nop.Web.Validators.Blogs;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Public.Validators.Blogs
{
    [TestFixture]
    public class BlogPostValidatorTests : BaseValidatorTests
    {
        private BlogPostValidator _validator;
        
        [SetUp]
        public new void Setup()
        {
            _validator = new BlogPostValidator(_localizationService);
        }
        //TODO uncomment tests when the following FluentVlidation issue is fixed http://fluentvalidation.codeplex.com/workitem/7095
        //[Test]
        //public void Should_have_error_when_comment_is_null_or_empty()
        //{
        //    var model = new BlogPostModel();
        //    model.AddNewComment.CommentText = null;
        //    _validator.ShouldHaveValidationErrorFor(x => x.AddNewComment.CommentText, model);
        //    model.AddNewComment.CommentText = "";
        //    _validator.ShouldHaveValidationErrorFor(x => x.AddNewComment.CommentText, model);
        //}

        //[Test]
        //public void Should_not_have_error_when_comment_is_specified()
        //{
        //    var model = new BlogPostModel();
        //    model.AddNewComment.CommentText = "some comment";
        //    _validator.ShouldNotHaveValidationErrorFor(x => x.AddNewComment.CommentText, model);
        //}
    }
}
