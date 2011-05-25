using System;
using System.Collections.Generic;
using Nop.Core.Domain.Forums;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class ForumPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_forum()
        {
            var forumGroup = new ForumGroup
            {
                Name = "Forum Group 1",
                Description = "Forum Group 1 Description",
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };

            var fromDb = SaveAndLoadEntity(forumGroup);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Forum Group 1");
            fromDb.Description.ShouldEqual("Forum Group 1 Description");
            fromDb.DisplayOrder.ShouldEqual(1);

            var forum = new Forum
            {
                ForumGroup = fromDb,
                Name = "Forum 1",
                Description = "Forum 1 Description",
                ForumGroupId = fromDb.Id,
                DisplayOrder = 10,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                NumPosts = 25,
                NumTopics = 15,
            };

            forumGroup.Forums.Add(forum);
            var fromDb2 = SaveAndLoadEntity(forum);
            fromDb2.ShouldNotBeNull();
            fromDb2.Name.ShouldEqual("Forum 1");
            fromDb2.Description.ShouldEqual("Forum 1 Description");
            fromDb2.DisplayOrder.ShouldEqual(10);
            fromDb2.NumTopics.ShouldEqual(15);
            fromDb2.NumPosts.ShouldEqual(25);
            fromDb2.ForumGroupId.ShouldEqual(fromDb.Id);
        }
    }
}