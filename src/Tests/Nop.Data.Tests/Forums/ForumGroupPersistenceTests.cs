using System;
using System.Linq;
using Nop.Core.Domain.Forums;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Forums
{
    [TestFixture]
    public class ForumGroupPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_forumgroup()
        {
            var forumGroup = new ForumGroup
            {
                Name = "Forum Group 1",
                DisplayOrder = 1,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };

            var fromDb = SaveAndLoadEntity(forumGroup);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Forum Group 1");
            fromDb.DisplayOrder.ShouldEqual(1);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.UpdatedOnUtc.ShouldEqual(new DateTime(2010, 01, 02));
        }

        [Test]
        public void Can_save_and_load_forumgroup_with_forums()
        {
            var forumGroup = new ForumGroup
            {
                Name = "Forum Group 1",
                DisplayOrder = 1,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };

            forumGroup.Forums.Add(new Forum
            {
                Name = "Forum 1",
                Description = "Forum 1 Description",
                DisplayOrder = 10,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                NumPosts = 25,
                NumTopics = 15,
            });

            var fromDb = SaveAndLoadEntity(forumGroup);
            fromDb.ShouldNotBeNull();

            fromDb.Forums.ShouldNotBeNull();
            (fromDb.Forums.Count == 1).ShouldBeTrue();
            fromDb.Forums.First().Name.ShouldEqual("Forum 1");

        }
    }
}
