using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Orders
{
    [TestFixture]
    public class ReturnRequestActionPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_returnRequestAction()
        {
            var returnRequestAction = this.GetTestReturnRequestAction();

            var fromDb = SaveAndLoadEntity(this.GetTestReturnRequestAction());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(returnRequestAction);
        }
    }
}
