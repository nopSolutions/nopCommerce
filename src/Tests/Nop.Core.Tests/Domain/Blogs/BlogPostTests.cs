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
    }
}
