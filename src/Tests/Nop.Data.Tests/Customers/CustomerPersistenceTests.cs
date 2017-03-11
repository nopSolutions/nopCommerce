using System.Linq;
using Nop.Core.Domain.Customers;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Customers
{
    [TestFixture]
    public class CustomerPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_customer()
        {
            var customer = this.GetTestCustomer();

            var fromDb = SaveAndLoadEntity(this.GetTestCustomer());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(customer);
        }

        [Test]
        public void Can_save_and_load_customer_with_customerRoles()
        {
            var customer = this.GetTestCustomer();
            customer.CustomerRoles.Add(this.GetTestCustomerRole());

            var fromDb = SaveAndLoadEntity(customer);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestCustomer());

            fromDb.CustomerRoles.ShouldNotBeNull();
            (fromDb.CustomerRoles.Count == 1).ShouldBeTrue();
            fromDb.CustomerRoles.First().PropertiesShouldEqual(this.GetTestCustomerRole());
        }

        [Test]
        public void Can_save_and_load_customer_with_externalAuthenticationRecord()
        {
            var customer = this.GetTestCustomer();
            customer.ExternalAuthenticationRecords.Add(this.GetTestExternalAuthenticationRecord());

            var fromDb = SaveAndLoadEntity(customer);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestCustomer());

            fromDb.ExternalAuthenticationRecords.ShouldNotBeNull();
            (fromDb.ExternalAuthenticationRecords.Count == 1).ShouldBeTrue();
            fromDb.ExternalAuthenticationRecords.First().PropertiesShouldEqual(this.GetTestExternalAuthenticationRecord());
        }

        [Test]
        public void Can_save_and_load_customer_with_address()
        {
            var customer = this.GetTestCustomer();
            customer.Addresses.Add(this.GetTestAddress());

            var fromDb = SaveAndLoadEntity(customer);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestCustomer());

            fromDb.Addresses.Count.ShouldEqual(1);
            fromDb.Addresses.First().PropertiesShouldEqual(this.GetTestAddress());
        }

        [Test]
        public void Can_set_default_billing_and_shipping_address()
        {
            var customer = this.GetTestCustomer();

            var address = this.GetTestAddress();
            var address2 = this.GetTestAddress();
            
            customer.Addresses.Add(address);
            customer.Addresses.Add(address2);
            customer.BillingAddress = address;
            customer.ShippingAddress = address2;

            var fromDb = SaveAndLoadEntity(customer);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestCustomer());

            fromDb.Addresses.Count.ShouldEqual(2);

            fromDb.BillingAddress.PropertiesShouldEqual(this.GetTestAddress());
            fromDb.ShippingAddress.PropertiesShouldEqual(this.GetTestAddress());

            var addresses = fromDb.Addresses.ToList();

            fromDb.BillingAddress.ShouldBeTheSameAs(addresses[0]);
            fromDb.ShippingAddress.ShouldBeTheSameAs(addresses[1]);
        }

        [Test]
        public void Can_remove_a_customer_address()
        {
            var customer = this.GetTestCustomer();
            var address = this.GetTestAddress();
            customer.Addresses.Add(address);
            customer.BillingAddress = address;

            var fromDb = SaveAndLoadEntity(customer);
            fromDb.ShouldNotBeNull();
            fromDb.Addresses.Count.ShouldEqual(1);
            fromDb.BillingAddress.ShouldNotBeNull();

            fromDb.RemoveAddress(address);

            context.SaveChanges();

            fromDb.Addresses.Count.ShouldEqual(0);
            fromDb.BillingAddress.ShouldBeNull();
        }
        
        [Test]
        public void Can_save_and_load_customer_with_shopping_cart()
        {
            var customer = this.GetTestCustomer();

            var shoppingCartItems = this.GetTestShoppingCartItem();
            shoppingCartItems.Product = this.GetTestProduct();

            var testShoppingCartItems = this.GetTestShoppingCartItem();
            testShoppingCartItems.Product = this.GetTestProduct();

            customer.ShoppingCartItems.Add(shoppingCartItems);

            var fromDb = SaveAndLoadEntity(customer);
            fromDb.ShouldNotBeNull();

            fromDb.ShoppingCartItems.ShouldNotBeNull();
            (fromDb.ShoppingCartItems.Count == 1).ShouldBeTrue();
            fromDb.ShoppingCartItems.First().PropertiesShouldEqual(testShoppingCartItems);
        }
    }
}
