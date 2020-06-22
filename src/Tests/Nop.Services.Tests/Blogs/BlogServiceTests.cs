﻿using System;
using FluentAssertions;
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
            _blogService = new BlogService(new CatalogSettings(), new FakeCacheKeyService(),  null, null, null, null, new TestCacheManager());
        }

        [Test]
        public void Can_parse_tags()
        {
            var blogPost = new BlogPost
            {
                Tags = "tag1, tag2, tag 3 4,  "
            };

            var tags = _blogService.ParseTags(blogPost);
            tags.Count.Should().Be(3);
            tags[0].Should().Be("tag1");
            tags[1].Should().Be("tag2");
            tags[2].Should().Be("tag 3 4");
        }

        [Test]
        public void Should_be_available_when_startdate_is_not_set()
        {
            var blogPost = new BlogPost
            {
                StartDateUtc = null
            };

            _blogService.BlogPostIsAvailable(blogPost, new DateTime(2010, 01, 03)).Should().BeTrue();
        }

        [Test]
        public void Should_be_available_when_startdate_is_less_than_somedate()
        {
            var blogPost = new BlogPost
            {
                StartDateUtc = new DateTime(2010, 01, 02)
            };

            _blogService.BlogPostIsAvailable(blogPost, new DateTime(2010, 01, 03)).Should().BeTrue();
        }

        [Test]
        public void Should_not_be_available_when_startdate_is_greater_than_somedate()
        {
            var blogPost = new BlogPost
            {
                StartDateUtc = new DateTime(2010, 01, 02)
            };

            _blogService.BlogPostIsAvailable(blogPost, new DateTime(2010, 01, 01)).Should().BeFalse();
        }

        [Test]
        public void Should_be_available_when_enddate_is_not_set()
        {
            var blogPost = new BlogPost
            {
                EndDateUtc = null
            };

            _blogService.BlogPostIsAvailable(blogPost, new DateTime(2010, 01, 03)).Should().BeTrue();
        }

        [Test]
        public void Should_be_available_when_enddate_is_greater_than_somedate()
        {
            var blogPost = new BlogPost
            {
                EndDateUtc = new DateTime(2010, 01, 02)
            };

            _blogService.BlogPostIsAvailable(blogPost, new DateTime(2010, 01, 01)).Should().BeTrue();
        }

        [Test]
        public void Should_not_be_available_when_enddate_is_less_than_somedate()
        {
            var blogPost = new BlogPost
            {
                EndDateUtc = new DateTime(2010, 01, 02)
            };

            _blogService.BlogPostIsAvailable(blogPost, new DateTime(2010, 01, 03)).Should().BeFalse();
        }
    }
}
