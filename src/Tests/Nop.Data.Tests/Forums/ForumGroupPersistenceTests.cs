using System;
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
                Description = "Forum Group 1 Description",
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            };

            var fromDb = SaveAndLoadEntity(forumGroup);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Forum Group 1");
            fromDb.Description.ShouldEqual("Forum Group 1 Description");
            fromDb.DisplayOrder.ShouldEqual(1);
        }
    }
}
