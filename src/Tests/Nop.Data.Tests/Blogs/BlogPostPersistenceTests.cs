using System.Linq;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Blogs
{
    [TestFixture]
    public class BlogPostPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_blogPost()
        {
            var blogPost = this.GetTestBlogPost();

            var fromDb = SaveAndLoadEntity(this.GetTestBlogPost());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(blogPost);

            fromDb.Language.ShouldNotBeNull();
            fromDb.Language.PropertiesShouldEqual(blogPost.Language);
        }

        [Test]
        public void Can_save_and_load_blogPost_with_blogComments()
        {
            var blogPost = this.GetTestBlogPost();

            blogPost.BlogComments.Add(this.GetTestBlogComment());
            var fromDb = SaveAndLoadEntity(blogPost);
            fromDb.ShouldNotBeNull();

            fromDb.BlogComments.ShouldNotBeNull();
            (fromDb.BlogComments.Count == 1).ShouldBeTrue();
            fromDb.BlogComments.First().IsApproved.ShouldEqual(true);
            fromDb.BlogComments.First().Store.ShouldNotBeNull();
        }        
    }
}
