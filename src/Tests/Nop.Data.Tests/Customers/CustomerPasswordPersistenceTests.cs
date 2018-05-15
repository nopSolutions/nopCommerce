using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Customers
{
    [TestFixture]
    public class CustomerPasswordPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_customerPassword()
        {
            var customerPassword = this.GetTestCustomerPassword();
            customerPassword.Customer = this.GetTestCustomer();

            var fromDb = SaveAndLoadEntity(customerPassword);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestCustomerPassword());
        }
    }
}