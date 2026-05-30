using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Models.Blogs;
using Nop.Web.Areas.Admin.Validators.Blogs;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Admin.Validators.Blogs;

[TestFixture]
public class BlogPostValidatorTests : BaseNopTest
{
    private BlogPostValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
        _validator = new BlogPostValidator(GetService<ILocalizationService>());
    }

    #region Title Validation Tests

    [Test]
    public void ShouldHaveErrorWhenTitleIsNull()
    {
        var model = new BlogPostModel
        {
            Title = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Test]
    public void ShouldHaveErrorWhenTitleIsEmpty()
    {
        var model = new BlogPostModel
        {
            Title = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Test]
    public void ShouldHaveErrorWhenTitleIsWhitespace()
    {
        var model = new BlogPostModel
        {
            Title = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Test]
    public void ShouldNotHaveErrorWhenTitleIsSpecified()
    {
        var model = new BlogPostModel
        {
            Title = "Sample Blog Post Title"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    #endregion

    #region Body Validation Tests

    [Test]
    public void ShouldHaveErrorWhenBodyIsNull()
    {
        var model = new BlogPostModel
        {
            Body = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Body);
    }

    [Test]
    public void ShouldHaveErrorWhenBodyIsEmpty()
    {
        var model = new BlogPostModel
        {
            Body = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Body);
    }

    [Test]
    public void ShouldHaveErrorWhenBodyIsWhitespace()
    {
        var model = new BlogPostModel
        {
            Body = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Body);
    }

    [Test]
    public void ShouldNotHaveErrorWhenBodyIsSpecified()
    {
        var model = new BlogPostModel
        {
            Body = "This is the blog post content."
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Body);
    }

    #endregion

    #region Tags Validation Tests

    [Test]
    public void ShouldHaveErrorWhenTagsContainDots()
    {
        var model = new BlogPostModel
        {
            Tags = "tag1,tag.with.dots,tag3"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Tags);
    }

    [Test]
    public void ShouldHaveErrorWhenTagsContainSingleDot()
    {
        var model = new BlogPostModel
        {
            Tags = "tag1,tag2,tag.3"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Tags);
    }

    [Test]
    public void ShouldNotHaveErrorWhenTagsDoNotContainDots()
    {
        var model = new BlogPostModel
        {
            Tags = "tag1,tag2,tag3"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Tags);
    }

    [Test]
    public void ShouldNotHaveErrorWhenTagsIsNull()
    {
        var model = new BlogPostModel
        {
            Tags = null
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Tags);
    }

    [Test]
    public void ShouldNotHaveErrorWhenTagsIsEmpty()
    {
        var model = new BlogPostModel
        {
            Tags = string.Empty
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Tags);
    }

    [Test]
    public void ShouldNotHaveErrorWhenTagsIsWhitespace()
    {
        var model = new BlogPostModel
        {
            Tags = "   "
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Tags);
    }

    #endregion

    #region SeName Validation Tests

    [Test]
    public void ShouldNotHaveErrorWhenSeNameIsNull()
    {
        var model = new BlogPostModel
        {
            SeName = null
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.SeName);
    }

    [Test]
    public void ShouldNotHaveErrorWhenSeNameIsEmpty()
    {
        var model = new BlogPostModel
        {
            SeName = string.Empty
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.SeName);
    }

    [Test]
    public void ShouldNotHaveErrorWhenSeNameIsWithinMaxLength()
    {
        var model = new BlogPostModel
        {
            SeName = new string('a', NopSeoDefaults.SearchEngineNameLength) // 200 characters
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.SeName);
    }

    [Test]
    public void ShouldHaveErrorWhenSeNameExceedsMaxLength()
    {
        var model = new BlogPostModel
        {
            SeName = new string('a', NopSeoDefaults.SearchEngineNameLength + 1) // 201 characters
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.SeName);
    }

    [Test]
    public void ShouldNotHaveErrorWhenSeNameIsValidLength()
    {
        var model = new BlogPostModel
        {
            SeName = "sample-blog-post-sename"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.SeName);
    }

    #endregion

    #region Combined Validation Tests

    [Test]
    public void ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalid()
    {
        var model = new BlogPostModel
        {
            Title = null,
            Body = string.Empty,
            Tags = "tag.with.dots",
            SeName = new string('a', NopSeoDefaults.SearchEngineNameLength + 1)
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
        result.ShouldHaveValidationErrorFor(x => x.Body);
        result.ShouldHaveValidationErrorFor(x => x.Tags);
        result.ShouldHaveValidationErrorFor(x => x.SeName);
    }

    [Test]
    public void ShouldNotHaveErrorsWhenAllFieldsAreValid()
    {
        var model = new BlogPostModel
        {
            Title = "Valid Blog Post Title",
            Body = "This is a valid blog post body content.",
            Tags = "tag1,tag2,tag3",
            SeName = "valid-blog-post-sename"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
        result.ShouldNotHaveValidationErrorFor(x => x.Body);
        result.ShouldNotHaveValidationErrorFor(x => x.Tags);
        result.ShouldNotHaveValidationErrorFor(x => x.SeName);
    }

    #endregion
}
