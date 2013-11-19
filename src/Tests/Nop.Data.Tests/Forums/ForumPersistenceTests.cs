using System;
using Nop.Core.Domain.Forums;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Forums
{
    [TestFixture]
    public class ForumPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_forum()
        {
            var forum = new Forum
            {
                ForumGroup = new ForumGroup
                {
                    Name = "Forum Group 1",
                    DisplayOrder = 1,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                },
                Name = "Forum 1",
                Description = "Forum 1 Description",
                DisplayOrder = 10,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
                NumPosts = 25,
                NumTopics = 15,
            };

            var fromDb = SaveAndLoadEntity(forum);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Forum 1");
            fromDb.Description.ShouldEqual("Forum 1 Description");
            fromDb.DisplayOrder.ShouldEqual(10);
            fromDb.NumTopics.ShouldEqual(15);
            fromDb.NumPosts.ShouldEqual(25);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.UpdatedOnUtc.ShouldEqual(new DateTime(2010, 01, 02));
        }
    }
}