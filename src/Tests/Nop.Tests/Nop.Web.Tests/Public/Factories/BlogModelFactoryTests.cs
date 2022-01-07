using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Services.Blogs;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Web.Factories;
using Nop.Web.Models.Blogs;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Factories
{
    [TestFixture]
    public class BlogModelFactoryTests : BaseNopTest
    {
        private IBlogModelFactory _blogModelFactory;
        private IBlogService _blogService;
        private ICustomerService _customerService;
        private IDateTimeHelper _dateTimeHelper;
        private CustomerSettings _customerSettings;
        private CaptchaSettings _captchaSettings;
        private BlogSettings _blogSettings;
        private ISettingService _settingsService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _blogModelFactory = GetService<IBlogModelFactory>();
            _blogService = GetService<IBlogService>();
            _customerService = GetService<ICustomerService>();
            _dateTimeHelper = GetService<IDateTimeHelper>();
            _customerSettings = GetService<CustomerSettings>();
            _captchaSettings = GetService<CaptchaSettings>();
            _blogSettings = GetService<BlogSettings>();
            _settingsService = GetService<ISettingService>();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            _customerSettings.AllowCustomersToUploadAvatars = false;
            _customerSettings.AllowViewingProfiles = false;
            await _settingsService.SaveSettingAsync(_customerSettings);
            _captchaSettings.Enabled = false;
            _captchaSettings.ShowOnBlogCommentPage = false;
            await _settingsService.SaveSettingAsync(_captchaSettings);
            _blogSettings.NumberOfTags = 15;
            await _settingsService.SaveSettingAsync(_blogSettings);
        }

        [Test]
        public void PrepareBlogPostModelShouldRaiseExceptionIfBlogPostModelOrBlogPostIsNull()
        {
            Assert.Throws<AggregateException>(() =>
                _blogModelFactory.PrepareBlogPostModelAsync(null, new BlogPost(), false).Wait());

            Assert.Throws<AggregateException>(() =>
                _blogModelFactory.PrepareBlogPostModelAsync(null, new BlogPost(), true).Wait());

            Assert.Throws<AggregateException>(() =>
                _blogModelFactory.PrepareBlogPostModelAsync(new BlogPostModel(), null, false).Wait());

            Assert.Throws<AggregateException>(() =>
                _blogModelFactory.PrepareBlogPostModelAsync(new BlogPostModel(), null, true).Wait());
        }

        [Test]
        public async Task CanPrepareBlogPostModelWithComets()
        {
            var model = new BlogPostModel();
            var blogPost = await _blogService.GetBlogPostByIdAsync(1);

            await _blogModelFactory.PrepareBlogPostModelAsync(model, blogPost, true);

            model.Id.Should().Be(blogPost.Id);
            model.MetaTitle.Should().Be(blogPost.MetaTitle);
            model.MetaDescription.Should().Be(blogPost.MetaDescription);
            model.MetaKeywords.Should().Be(blogPost.MetaKeywords);
            model.SeName = model.Title.Replace(" ", "-").ToLowerInvariant();
            model.Title.Should().Be(blogPost.Title);
            model.Body.Should().Be(blogPost.Body);
            model.BodyOverview.Should().Be(blogPost.BodyOverview);
            model.AllowComments.Should().Be(blogPost.AllowComments);
            model.CreatedOn.ToUniversalTime().Should().Be(blogPost.CreatedOnUtc);
            model.Tags.Should().BeEquivalentTo(await _blogService.ParseTagsAsync(blogPost));
            model.NumberOfComments.Should().Be(await _blogService.GetBlogCommentsCountAsync(blogPost, 0, true));
            model.Comments.Count.Should().Be(model.NumberOfComments);
        }

        [Test]
        public async Task CanPrepareBlogPostModelWithOutComets()
        {
            var model = new BlogPostModel();
            var blogPost = await _blogService.GetBlogPostByIdAsync(1);

            await _blogModelFactory.PrepareBlogPostModelAsync(model, blogPost, false);

            model.Id.Should().Be(blogPost.Id);
            model.MetaTitle.Should().Be(blogPost.MetaTitle);
            model.MetaDescription.Should().Be(blogPost.MetaDescription);
            model.MetaKeywords.Should().Be(blogPost.MetaKeywords);
            model.SeName = model.Title.Replace(" ", "-").ToLowerInvariant();
            model.Title.Should().Be(blogPost.Title);
            model.Body.Should().Be(blogPost.Body);
            model.BodyOverview.Should().Be(blogPost.BodyOverview);
            model.AllowComments.Should().Be(blogPost.AllowComments);
            model.CreatedOn.ToUniversalTime().Should().Be(blogPost.CreatedOnUtc);
            model.Tags.Should().BeEquivalentTo(await _blogService.ParseTagsAsync(blogPost));
            model.NumberOfComments.Should().Be(await _blogService.GetBlogCommentsCountAsync(blogPost, 0, true));
            model.Comments.Count.Should().Be(0);
        }

        [Test]
        public async Task PrepareBlogPostModelDisplayCaptchaShouldBeDependOnSettings()
        {
            var model = new BlogPostModel();
            var blogPost = await _blogService.GetBlogPostByIdAsync(1);
            await _blogModelFactory.PrepareBlogPostModelAsync(model, blogPost, false);

            model.AddNewComment.DisplayCaptcha.Should().BeFalse();

            _captchaSettings.Enabled = _captchaSettings.ShowOnBlogCommentPage = true;
            await _settingsService.SaveSettingAsync(_captchaSettings);

            await GetService<IBlogModelFactory>().PrepareBlogPostModelAsync(model, blogPost, false);
            _captchaSettings.Enabled = _captchaSettings.ShowOnBlogCommentPage = false;
            await _settingsService.SaveSettingAsync(_captchaSettings);

            model.AddNewComment.DisplayCaptcha.Should().BeTrue();
        }

        [Test]
        public async Task CanPrepareBlogPostListModel()
        {
            var model = await _blogModelFactory.PrepareBlogPostListModelAsync(new BlogPagingFilteringModel());
            
            model.PagingFilteringContext.FirstItem.Should().Be(1);
            model.PagingFilteringContext.HasNextPage.Should().BeFalse();
            model.PagingFilteringContext.HasPreviousPage.Should().BeFalse();
            model.PagingFilteringContext.LastItem.Should().Be(2);
            model.PagingFilteringContext.PageNumber.Should().Be(1);
            model.PagingFilteringContext.PageSize.Should().Be(10);
            model.PagingFilteringContext.TotalItems.Should().Be(2);
            model.PagingFilteringContext.TotalPages.Should().Be(1);
            model.PagingFilteringContext.Tag.Should().BeNull();
            model.PagingFilteringContext.Month.Should().BeNull();

            model.BlogPosts.Should().NotBeNull();
            model.BlogPosts.Count.Should().Be(2);

            var date = DateTime.Now.ToUniversalTime();

            model = await _blogModelFactory.PrepareBlogPostListModelAsync(new BlogPagingFilteringModel
            {
                Month = $"{date.Year}-{date.Month}"
            });

            model.PagingFilteringContext.Month.Should().NotBeNullOrEmpty();
            model.PagingFilteringContext.Tag.Should().BeNull();
            model.BlogPosts.Should().NotBeNull();
            model.BlogPosts.Count.Should().Be(2);

            model = await _blogModelFactory.PrepareBlogPostListModelAsync(new BlogPagingFilteringModel
            {
                Month = $"{date.Year}-{date.Month}",
                Tag = "nopCommerce"
            });

            model.PagingFilteringContext.Month.Should().NotBeNullOrEmpty();
            model.PagingFilteringContext.Tag.Should().NotBeNullOrEmpty();
            model.BlogPosts.Should().NotBeNull();
            model.BlogPosts.Count.Should().Be(1);
        }

        [Test]
        public void PrepareBlogPostListModelShouldRaiseExceptionIfCommandIsNull()
        {
            Assert.Throws<AggregateException>(() =>
                _blogModelFactory.PrepareBlogPostListModelAsync(null).Wait());
        }

        [Test]
        public async Task CanPrepareBlogPostTagListModel()
        {
            var model = await _blogModelFactory.PrepareBlogPostTagListModelAsync();
            model.Tags.Count.Should().Be(6);
        }

        [Test]
        public async Task PrepareBlogPostTagListModelCountShouldDependOnSettings()
        {
            var model = await _blogModelFactory.PrepareBlogPostTagListModelAsync();
            model.Tags.Count.Should().Be(6);

            _blogSettings.NumberOfTags = 1;
            await _settingsService.SaveSettingAsync(_blogSettings);

            model = await GetService<IBlogModelFactory>().PrepareBlogPostTagListModelAsync();

            _blogSettings.NumberOfTags = 15;
            await _settingsService.SaveSettingAsync(_blogSettings);

            model.Tags.Count.Should().Be(1);
        }

        [Test]
        public async Task CanPrepareBlogPostYearModel()
        {
            var model = await _blogModelFactory.PrepareBlogPostYearModelAsync();
            var date = DateTime.Now.ToUniversalTime();

            model.Count.Should().Be(1);
            model[0].Year.Should().Be(date.Year);
            var months = model[0].Months;

            months.Count.Should().Be(1);

            var month = months.First();

            month.BlogPostCount.Should().Be(2);
            month.Month.Should().Be(date.Month);
        }

        [Test]
        public void PrepareBlogPostCommentModelShouldRaiseExceptionIfBlogCommentIsNull()
        {
            Assert.Throws<AggregateException>(() =>
                _blogModelFactory.PrepareBlogPostCommentModelAsync(null).Wait());
        }

        [Test]
        public async Task PrepareBlogPostCommentCustomerAvatarUrlShouldNotBeNullIfAllowCustomersToUploadAvatarsIsTrue()
        {
            var blogComment = await _blogService.GetBlogCommentByIdAsync(1);
            _customerSettings.AllowCustomersToUploadAvatars = true;
            await _settingsService.SaveSettingAsync(_customerSettings);
            var blogModelFactory = GetService<IBlogModelFactory>();
            var model = await blogModelFactory.PrepareBlogPostCommentModelAsync(blogComment);
            _customerSettings.AllowCustomersToUploadAvatars = false;
            await _settingsService.SaveSettingAsync(_customerSettings);

            model.CustomerAvatarUrl.Should().NotBeNullOrEmpty();
            model.CustomerAvatarUrl.Should()
                .Be($"http://{NopTestsDefaults.HostIpAddress}/images/thumbs/default-avatar_{GetService<MediaSettings>().AvatarPictureSize}.jpg");
        }

        [Test]
        public async Task PrepareBlogPostCommentCustomerAllowViewingProfilesShouldBeDependOnSettings()
        {
            var blogComment = await _blogService.GetBlogCommentByIdAsync(1);
            var model = await _blogModelFactory.PrepareBlogPostCommentModelAsync(blogComment);
            model.AllowViewingProfiles.Should().BeFalse();
            _customerSettings.AllowViewingProfiles = true;
            await _settingsService.SaveSettingAsync(_customerSettings);
            var blogModelFactory = GetService<IBlogModelFactory>();
            model = await blogModelFactory.PrepareBlogPostCommentModelAsync(blogComment);
            _customerSettings.AllowViewingProfiles = false;
            await _settingsService.SaveSettingAsync(_customerSettings);
            model.AllowViewingProfiles.Should().BeTrue();
        }

        [Test]
        public async Task CanPrepareBlogPostComment()
        {
            var blogComment = await _blogService.GetBlogCommentByIdAsync(1);
            var model = await _blogModelFactory.PrepareBlogPostCommentModelAsync(blogComment);

            var customer = await _customerService.GetCustomerByIdAsync(1);

            model.Id.Should().Be(blogComment.Id);
            model.CustomerId.Should().Be(blogComment.CustomerId);
            model.CustomerName.Should().Be(await _customerService.FormatUsernameAsync(customer));
            model.CommentText.Should().Be(blogComment.CommentText);
            model.CreatedOn.Should().Be(await _dateTimeHelper.ConvertToUserTimeAsync(blogComment.CreatedOnUtc, DateTimeKind.Utc));
            model.AllowViewingProfiles.Should().BeFalse();
            model.CustomerAvatarUrl.Should().BeNull();
        }
    }
}
