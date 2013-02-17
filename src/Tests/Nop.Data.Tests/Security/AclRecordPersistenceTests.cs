using Nop.Core.Domain.Security;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Security
{
    [TestFixture]
    public class AclRecordPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_urlRecord()
        {
            var aclRecord = new AclRecord
            {
                EntityId = 1,
                EntityName = "EntityName 1",
                CustomerRoleId = 2,
            };

            var fromDb = SaveAndLoadEntity(aclRecord);
            fromDb.ShouldNotBeNull();
            fromDb.EntityId.ShouldEqual(1);
            fromDb.EntityName.ShouldEqual("EntityName 1");
            fromDb.CustomerRoleId.ShouldEqual(2);
        }
    }
}
