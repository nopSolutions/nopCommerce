using FluentAssertions;
using Nop.Core.Domain.Blogs;
using Nop.Services.Blogs;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Blogs
{
    [TestFixture]
    public class BlogServiceTests : ServiceTest
    {
        private IBlogService _blogService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _blogService = GetService<IBlogService>();
        }

        [Test]
        public async Task CanParseTags()
        {
            var blogPost = new BlogPost
            {
                Tags = "tag1, tag2, tag 3 4,  "
            };

            var tags = await _blogService.ParseTagsAsync(blogPost);
            tags.Count.Should().Be(3);
            tags[0].Should().Be("tag1");
            tags[1].Should().Be("tag2");
            tags[2].Should().Be("tag 3 4");
        }

        [Test]
        public void ShouldBeAvailableWhenStartDateIsNotSet()
        {
            var blogPost = new BlogPost
            {
                StartDateUtc = null
            };

            _blogService.BlogPostIsAvailable(blogPost, new DateTime(2010, 01, 03)).Should().BeTrue();
        }

        [Test]
        public void ShouldBeAvailableWhenStartDateIsLessThanSomeDate()
        {
            var blogPost = new BlogPost
            {
                StartDateUtc = new DateTime(2010, 01, 02)
            };

            _blogService.BlogPostIsAvailable(blogPost, new DateTime(2010, 01, 03)).Should().BeTrue();
        }

        [Test]
        public void ShouldNotBeAvailableWhenStartDateIsGreaterThanSomeDate()
        {
            var blogPost = new BlogPost
            {
                StartDateUtc = new DateTime(2010, 01, 02)
            };

            _blogService.BlogPostIsAvailable(blogPost, new DateTime(2010, 01, 01)).Should().BeFalse();
        }

        [Test]
        public void ShouldBeAvailableWhenEndDateIsNotSet()
        {
            var blogPost = new BlogPost
            {
                EndDateUtc = null
            };

            _blogService.BlogPostIsAvailable(blogPost, new DateTime(2010, 01, 03)).Should().BeTrue();
        }

        [Test]
        public void ShouldBeAvailableWhenEndDateIsGreaterThanSomeDate()
        {
            var blogPost = new BlogPost
            {
                EndDateUtc = new DateTime(2010, 01, 02)
            };

            _blogService.BlogPostIsAvailable(blogPost, new DateTime(2010, 01, 01)).Should().BeTrue();
        }

        [Test]
        public void ShouldNotBeAvailableWhenEndDateIsLessThanSomeDate()
        {
            var blogPost = new BlogPost
            {
                EndDateUtc = new DateTime(2010, 01, 02)
            };

            _blogService.BlogPostIsAvailable(blogPost, new DateTime(2010, 01, 03)).Should().BeFalse();
        }

        [Test]
        public async Task CanGetAllBlogPosts()
        {
            var blogPosts = await _blogService.GetAllBlogPostsAsync();
            blogPosts.TotalCount.Should().Be(2);

            blogPosts = await _blogService.GetAllBlogPostsAsync(showHidden: true);
            blogPosts.TotalCount.Should().Be(2);
        }

        [Test]
        public async Task CanGetAllBlogPostsByTag()
        {
            var blogPosts = await _blogService.GetAllBlogPostsByTagAsync(showHidden: true);
            blogPosts.TotalCount.Should().Be(0);

            blogPosts = await _blogService.GetAllBlogPostsByTagAsync();
            blogPosts.TotalCount.Should().Be(0);

            blogPosts = await _blogService.GetAllBlogPostsByTagAsync(tag: "e-commerce");
            blogPosts.TotalCount.Should().Be(2);

            blogPosts = await _blogService.GetAllBlogPostsByTagAsync(tag: "nopCommerce");
            blogPosts.TotalCount.Should().Be(1);

            blogPosts = await _blogService.GetAllBlogPostsByTagAsync(tag: "blog");
            blogPosts.TotalCount.Should().Be(1);

            blogPosts = await _blogService.GetAllBlogPostsByTagAsync(tag: "not exists");
            blogPosts.TotalCount.Should().Be(0);
        }

        [Test]
        public async Task CanGetAllBlogPostTags()
        {
            var blogPostTags = await _blogService.GetAllBlogPostTagsAsync(1, 1);
            blogPostTags.Count.Should().Be(6);

            blogPostTags = await _blogService.GetAllBlogPostTagsAsync(2, 1);
            blogPostTags.Count.Should().Be(6);

            blogPostTags = await _blogService.GetAllBlogPostTagsAsync(1, 2);
            blogPostTags.Count.Should().Be(0);
        }

        [Test]
        public async Task CanGetPostsByDate()
        {
            var posts = (await _blogService.GetAllBlogPostsByTagAsync(tag: "e-commerce")).OrderBy(b => b.Id).ToList();
            posts[1].CreatedOnUtc = posts[1].CreatedOnUtc.AddDays(1);

            var filteredPosts = await _blogService.GetPostsByDateAsync(posts, posts[0].CreatedOnUtc, posts[0].CreatedOnUtc);
            filteredPosts.Count.Should().Be(1);

            filteredPosts = await _blogService.GetPostsByDateAsync(posts, posts[0].CreatedOnUtc, posts[1].CreatedOnUtc);
            filteredPosts.Count.Should().Be(2);
        }

        [Test]
        public async Task CanGetAllComments()
        {
            var comments = await _blogService.GetAllCommentsAsync();
            comments.Count.Should().Be(2);
            comments = await _blogService.GetAllCommentsAsync(blogPostId: 1);
            comments.Count.Should().Be(1);
            comments = await _blogService.GetAllCommentsAsync(blogPostId: 3);
            comments.Count.Should().Be(0);
        }

        [Test]
        public async Task CanGetBlogCommentsCount()
        {
            var post = await _blogService.GetBlogPostByIdAsync(1);
            var count = await _blogService.GetBlogCommentsCountAsync(post);
            count.Should().Be(1);
        }

        [Test]
        public async Task TestCRUD()
        {
            var post = new BlogPost
            {
                Title = string.Empty,
                Body = string.Empty,
                LanguageId = 1
            };

            await _blogService.InsertBlogPostAsync(post);

            var postId = post.Id;

            postId.Should().BeGreaterThan(0);

            post.Body.Should().BeNullOrEmpty();
            post.Body = "test body";
            await _blogService.UpdateBlogPostAsync(post);
            var testPost = await _blogService.GetBlogPostByIdAsync(postId);

            testPost.Body.Should().NotBeEmpty();
            testPost.Body.Should().BeEquivalentTo(post.Body);

            var comment = new BlogComment { BlogPostId = postId, StoreId = 1, CustomerId = 1 };

            await _blogService.InsertBlogCommentAsync(comment);
            var commentId = comment.Id;

            commentId.Should().BeGreaterThan(0);
            comment.CommentText.Should().BeNullOrEmpty();
            comment.CommentText = "test comment";
            await _blogService.UpdateBlogCommentAsync(comment);
            var testComment = await _blogService.GetBlogCommentByIdAsync(commentId);

            testComment.CommentText.Should().NotBeEmpty();
            testComment.CommentText.Should().BeEquivalentTo(comment.CommentText);

            await _blogService.DeleteBlogCommentAsync(comment);
            testComment = await _blogService.GetBlogCommentByIdAsync(commentId);
            testComment.Should().BeNull();

            var comments = new List<BlogComment>
            {
                new() {BlogPostId = postId, StoreId = 1, CustomerId = 1},
                new() {BlogPostId = postId, StoreId = 1, CustomerId = 1},
                new() {BlogPostId = postId, StoreId = 1, CustomerId = 1}
            } as IList<BlogComment>;

            foreach (var blogComment in comments)
                await _blogService.InsertBlogCommentAsync(blogComment);

            comments = await _blogService.GetBlogCommentsByIdsAsync(comments.Select(p => p.Id).ToArray());
            comments.Count.Should().Be(3);

            await _blogService.DeleteBlogCommentsAsync(comments);

            comments = await _blogService.GetBlogCommentsByIdsAsync(comments.Select(p => p.Id).ToArray());
            comments.Count.Should().Be(0);

            await _blogService.DeleteBlogPostAsync(post);

            testPost = await _blogService.GetBlogPostByIdAsync(postId);
            testPost.Should().BeNull();
        }
    }
}
