using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Affiliates
{
    [TestFixture]
    public class AffiliatePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_affiliate()
        {
            var affiliate = this.GetTestAffiliate();

            var fromDb = SaveAndLoadEntity(this.GetTestAffiliate());
            fromDb.ShouldNotBeNull();
            fromDb.Address.ShouldNotBeNull();

            fromDb.PropertiesShouldEqual(affiliate);
            fromDb.Address.PropertiesShouldEqual(affiliate.Address);
        }        
    }
}
