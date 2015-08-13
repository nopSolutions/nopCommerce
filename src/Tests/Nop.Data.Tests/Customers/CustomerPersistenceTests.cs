using System;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
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
            var customer = GetTestCustomer();

            var fromDb = SaveAndLoadEntity(customer);
            fromDb.ShouldNotBeNull();
            fromDb.Username.ShouldEqual("a@b.com");
            fromDb.Password.ShouldEqual("password");
            fromDb.PasswordFormat.ShouldEqual(PasswordFormat.Clear);
            fromDb.PasswordSalt.ShouldEqual("");
            fromDb.Email.ShouldEqual("a@b.com");
            fromDb.AdminComment.ShouldEqual("some comment here");
            fromDb.IsTaxExempt.ShouldEqual(true);
            fromDb.AffiliateId.ShouldEqual(1);
            fromDb.VendorId.ShouldEqual(2);
            fromDb.HasShoppingCartItems.ShouldEqual(true);
            fromDb.Active.ShouldEqual(true);
            fromDb.Deleted.ShouldEqual(false);
            fromDb.IsSystemAccount.ShouldEqual(true);
            fromDb.SystemName.ShouldEqual("SystemName 1");
            fromDb.LastIpAddress.ShouldEqual("192.168.1.1");
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.LastLoginDateUtc.ShouldEqual(new DateTime(2010, 01, 02));
            fromDb.LastActivityDateUtc.ShouldEqual(new DateTime(2010, 01, 03));
        }

        [Test]
        public void Can_save_and_load_customer_with_customerRoles()
        {
            var customer = GetTestCustomer();
            customer.CustomerRoles.Add
            (
                new CustomerRole
                {
                    Name = "Administrators",
                    FreeShipping = true,
                    TaxExempt = true,
                    Active = true,
                    IsSystemRole = true,
                    SystemName = "Administrators"
                }
            );


            var fromDb = SaveAndLoadEntity(customer);
            fromDb.ShouldNotBeNull();

            fromDb.CustomerRoles.ShouldNotBeNull();
            (fromDb.CustomerRoles.Count == 1).ShouldBeTrue();
            fromDb.CustomerRoles.First().Name.ShouldEqual("Administrators");
        }

        [Test]
        public void Can_save_and_load_customer_with_externalAuthenticationRecord()
        {
            var customer = GetTestCustomer();
            customer.ExternalAuthenticationRecords.Add
            (
                new ExternalAuthenticationRecord
                {
                    ExternalIdentifier = "ExternalIdentifier 1",
                    ExternalDisplayIdentifier = "ExternalDisplayIdentifier 1",
                    OAuthToken = "OAuthToken 1",
                    OAuthAccessToken = "OAuthAccessToken 1",
                    ProviderSystemName = "ProviderSystemName 1",
                }
            );


            var fromDb = SaveAndLoadEntity(customer);
            fromDb.ShouldNotBeNull();

            fromDb.ExternalAuthenticationRecords.ShouldNotBeNull();
            (fromDb.ExternalAuthenticationRecords.Count == 1).ShouldBeTrue();
            fromDb.ExternalAuthenticationRecords.First().ExternalIdentifier.ShouldEqual("ExternalIdentifier 1");
        }


        [Test]
        public void Can_save_and_load_customer_with_address()
        {
            var customer = GetTestCustomer();

            var address = new Address 
            { 
                FirstName = "Test",
                Country = GetTestCountry(),
                CreatedOnUtc = new DateTime(2010, 01, 01),
            };

            customer.Addresses.Add(address);

            var fromDb = SaveAndLoadEntity(customer);
            fromDb.ShouldNotBeNull();
            fromDb.Addresses.Count.ShouldEqual(1);
            fromDb.Addresses.First().FirstName.ShouldEqual("Test");
        }

        [Test]
        public void Can_set_default_billing_and_shipping_address()
        {
            var customer = GetTestCustomer();

            var address = new Address { FirstName = "Billing", Country = GetTestCountry(), CreatedOnUtc = new DateTime(2010, 01, 01) };
            var address2 = new Address { FirstName = "Shipping", Country = GetTestCountry(), CreatedOnUtc = new DateTime(2010, 01, 01) };

            customer.Addresses.Add(address);
            customer.Addresses.Add(address2);

            customer.BillingAddress = address;
            customer.ShippingAddress = address2;

            var fromDb = SaveAndLoadEntity(customer);
            fromDb.ShouldNotBeNull();
            fromDb.Addresses.Count.ShouldEqual(2);

            fromDb.BillingAddress.FirstName.ShouldEqual("Billing");
            fromDb.ShippingAddress.FirstName.ShouldEqual("Shipping");

            var addresses = fromDb.Addresses.ToList();

            fromDb.BillingAddress.ShouldBeTheSameAs(addresses[0]);
            fromDb.ShippingAddress.ShouldBeTheSameAs(addresses[1]);
        }

        [Test]
        public void Can_remove_a_customer_address()
        {
            var customer = GetTestCustomer();
            var address = new Address { FirstName = "Test", Country = GetTestCountry(), CreatedOnUtc = new DateTime(2010, 01, 01) };
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
            var customer = GetTestCustomer();
            var product = GetTestProduct();

            customer.ShoppingCartItems.Add
            (
                new ShoppingCartItem
                {
                    ShoppingCartType = ShoppingCartType.ShoppingCart,
                    AttributesXml = "AttributesXml 1",
                    CustomerEnteredPrice = 1,
                    Quantity = 2,
                    CreatedOnUtc = new DateTime(2010, 01, 01),
                    UpdatedOnUtc = new DateTime(2010, 01, 02),
                    Product = product,
                }
            );


            var fromDb = SaveAndLoadEntity(customer);
            fromDb.ShouldNotBeNull();

            fromDb.ShoppingCartItems.ShouldNotBeNull();
            (fromDb.ShoppingCartItems.Count == 1).ShouldBeTrue();
            fromDb.ShoppingCartItems.First().AttributesXml.ShouldEqual("AttributesXml 1");
        }
        protected Address GetTestAddress()
        {
            return new Address
            {
                FirstName = "FirstName 1",
                LastName = "LastName 1",
                Email = "Email 1",
                Company = "Company 1",
                City = "City 1",
                Address1 = "Address1a",
                Address2 = "Address1a",
                ZipPostalCode = "ZipPostalCode 1",
                PhoneNumber = "PhoneNumber 1",
                FaxNumber = "FaxNumber 1",
                CreatedOnUtc = new DateTime(2010, 01, 01),
                Country = new Country
                {
                    Name = "United States",
                    TwoLetterIsoCode = "US",
                    ThreeLetterIsoCode = "USA",
                }
            };
        }

        protected Customer GetTestCustomer()
        {
            return new Customer
            {
                Username = "a@b.com",
                Password = "password",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = "",
                Email = "a@b.com",
                CustomerGuid = Guid.NewGuid(),
                AdminComment = "some comment here",
                IsTaxExempt = true,
                AffiliateId = 1,
                VendorId = 2,
                HasShoppingCartItems = true,
                Active = true,
                Deleted = false,
                IsSystemAccount = true,
                SystemName = "SystemName 1",
                LastIpAddress = "192.168.1.1",
                CreatedOnUtc = new DateTime(2010, 01, 01),
                LastLoginDateUtc = new DateTime(2010, 01, 02),
                LastActivityDateUtc = new DateTime(2010, 01, 03)
            };
        }

        protected Product GetTestProduct()
        {
            return new Product
            {
                Name = "Product name 1",
                CreatedOnUtc = new DateTime(2010, 01, 03),
                UpdatedOnUtc = new DateTime(2010, 01, 04),
            };
        }

        protected Country GetTestCountry()
        {
            return new Country
            {
                Name = "United States",
                AllowsBilling = true,
                AllowsShipping = true,
                TwoLetterIsoCode = "US",
                ThreeLetterIsoCode = "USA",
                NumericIsoCode = 1,
                SubjectToVat = true,
                Published = true,
                DisplayOrder = 1
            };
        }

        protected Store GetTestStore()
        {
            return new Store
            {
                Name = "Store 1",
                Url = "http://www.test.com",
                DisplayOrder = 1
            };
        }

    }
}
