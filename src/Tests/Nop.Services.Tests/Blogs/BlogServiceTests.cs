﻿using System;
using FluentAssertions;
using Nop.Core.Domain.Blogs;
using Nop.Services.Blogs;
using NUnit.Framework;

namespace Nop.Services.Tests.Blogs
{
    [TestFixture]
    public class BlogServiceTests: ServiceTest
    {
        private IBlogService _blogService;

        [SetUp]
        public void SetUp()
        {
            _blogService = GetService<IBlogService>();
        }

        [Test]
        public void CanParseTags()
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
    }
}
