using System.Linq;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Orders
{
    [TestFixture]
    public class RecurringPaymentPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_recurringPayment()
        {
            var rp = this.GetTestRecurringPayment();
            rp.InitialOrder.Customer = this.GetTestCustomer();
            var fromDb = SaveAndLoadEntity(rp);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestRecurringPayment());

            fromDb.InitialOrder.ShouldNotBeNull();
            fromDb.InitialOrder.PropertiesShouldEqual(this.GetTestOrder());
        }

        [Test]
        public void Can_save_and_load_recurringPayment_with_history()
        {
            var rp = this.GetTestRecurringPayment();
            rp.InitialOrder.Customer = this.GetTestCustomer();
            rp.RecurringPaymentHistory.Add(this.GetTestRecurringPaymentHistory());
            var fromDb = SaveAndLoadEntity(rp);
            fromDb.ShouldNotBeNull();

            fromDb.RecurringPaymentHistory.ShouldNotBeNull();
            (fromDb.RecurringPaymentHistory.Count == 1).ShouldBeTrue();
            fromDb.RecurringPaymentHistory.First().PropertiesShouldEqual(this.GetTestRecurringPaymentHistory());
        }
    }
}