using System;
using Nop.Core.Domain.Blogs;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Core.Tests.Domain.Blogs
{
    [TestFixture]
    public class BlogPostTests
    {
        [Test]
        public void Can_parse_tags()
        {
            var blogPost = new BlogPost
            {
                Tags = "tag1, tag2, tag 3 4,  "
            };

            var tags = blogPost.ParseTags();
            tags.Length.ShouldEqual(3);
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

            blogPost.IsAvailable(new DateTime(2010, 01, 03)).ShouldEqual(true);
        }

        [Test]
        public void Should_be_available_when_startdate_is_less_than_somedate()
        {
            var blogPost = new BlogPost
            {
                StartDateUtc = new DateTime(2010, 01, 02)
            };

            blogPost.IsAvailable(new DateTime(2010, 01, 03)).ShouldEqual(true);
        }

        [Test]
        public void Should_not_be_available_when_startdate_is_greater_than_somedate()
        {
            var blogPost = new BlogPost
            {
                StartDateUtc = new DateTime(2010, 01, 02)
            };

            blogPost.IsAvailable(new DateTime(2010, 01, 01)).ShouldEqual(false);
        }

        [Test]
        public void Should_be_available_when_enddate_is_not_set()
        {
            var blogPost = new BlogPost
            {
                EndDateUtc = null
            };

            blogPost.IsAvailable(new DateTime(2010, 01, 03)).ShouldEqual(true);
        }

        [Test]
        public void Should_be_available_when_enddate_is_greater_than_somedate()
        {
            var blogPost = new BlogPost
            {
                EndDateUtc = new DateTime(2010, 01, 02)
            };

            blogPost.IsAvailable(new DateTime(2010, 01, 01)).ShouldEqual(true);
        }

        [Test]
        public void Should_not_be_available_when_enddate_is_less_than_somedate()
        {
            var blogPost = new BlogPost
            {
                EndDateUtc = new DateTime(2010, 01, 02)
            };

            blogPost.IsAvailable(new DateTime(2010, 01, 03)).ShouldEqual(false);
        }
    }
}
