using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Vendors
{
    [TestFixture]
    public class VendorNotePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_vendorNote()
        {
            var on = this.GetTestVendorNote();
            on.Vendor = this.GetTestVendor();

            var fromDb = SaveAndLoadEntity(on);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestVendorNote());

            fromDb.Vendor.ShouldNotBeNull();
            fromDb.Vendor.PropertiesShouldEqual(this.GetTestVendor());
        }
    }
}