using System.Linq;
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
            var forumGroup = this.GetTestForumGroup();

            var fromDb = SaveAndLoadEntity(this.GetTestForumGroup());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(forumGroup);
        }

        [Test]
        public void Can_save_and_load_forumgroup_with_forums()
        {
            var forumGroup = this.GetTestForumGroup();

            forumGroup.Forums.Add(this.GetTestForum());

            var fromDb = SaveAndLoadEntity(forumGroup);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestForumGroup());

            fromDb.Forums.ShouldNotBeNull();
            (fromDb.Forums.Count == 1).ShouldBeTrue();
            fromDb.Forums.First().PropertiesShouldEqual(this.GetTestForum());
        }
    }
}
