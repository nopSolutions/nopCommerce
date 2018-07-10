using System;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Services.Blogs;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Blogs
{
    [TestFixture]
    public class BlogServiceTests
    {
        private IBlogService _blogService;

        [SetUp]
        public void SetUp()
        {
            _blogService = new BlogService(new CatalogSettings(), null, null, null, null);
        }

        [Test]
        public void Can_parse_tags()
        {
            var blogPost = new BlogPost
            {
                Tags = "tag1, tag2, tag 3 4,  "
            };

            var tags = _blogService.ParseTags(blogPost);
            tags.Count.ShouldEqual(3);
            tags[0].ShouldEqual("tag1");
            tags[1].ShouldEqual("tag2");
            tags[2].ShouldEqual("tag 3 4");
        }

        [Test]
        public void Should_be_available_when_startdate_is_not_set()
        {
            var blogPost = new BlogPost
            {
                StartDateUtc = null
            };

            _blogService.BlogPostIsAvailable(blogPost, new DateTime(2010, 01, 03)).ShouldEqual(true);
        }

        [Test]
        public void Should_be_available_when_startdate_is_less_than_somedate()
        {
            var blogPost = new BlogPost
            {
                StartDateUtc = new DateTime(2010, 01, 02)
            };

            _blogService.BlogPostIsAvailable(blogPost, new DateTime(2010, 01, 03)).ShouldEqual(true);
        }

        [Test]
        public void Should_not_be_available_when_startdate_is_greater_than_somedate()
        {
            var blogPost = new BlogPost
            {
                StartDateUtc = new DateTime(2010, 01, 02)
            };

            _blogService.BlogPostIsAvailable(blogPost, new DateTime(2010, 01, 01)).ShouldEqual(false);
        }

        [Test]
        public void Should_be_available_when_enddate_is_not_set()
        {
            var blogPost = new BlogPost
            {
                EndDateUtc = null
            };

            _blogService.BlogPostIsAvailable(blogPost, new DateTime(2010, 01, 03)).ShouldEqual(true);
        }

        [Test]
        public void Should_be_available_when_enddate_is_greater_than_somedate()
        {
            var blogPost = new BlogPost
            {
                EndDateUtc = new DateTime(2010, 01, 02)
            };

            _blogService.BlogPostIsAvailable(blogPost, new DateTime(2010, 01, 01)).ShouldEqual(true);
        }

        [Test]
        public void Should_not_be_available_when_enddate_is_less_than_somedate()
        {
            var blogPost = new BlogPost
            {
                EndDateUtc = new DateTime(2010, 01, 02)
            };

            _blogService.BlogPostIsAvailable(blogPost, new DateTime(2010, 01, 03)).ShouldEqual(false);
        }
    }
}
