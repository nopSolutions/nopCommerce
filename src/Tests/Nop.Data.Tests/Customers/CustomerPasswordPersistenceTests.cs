using System;
using Nop.Core.Domain.Customers;
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
            var customerPassword = GetTestCustomerPassword();

            var fromDb = SaveAndLoadEntity(customerPassword);
            fromDb.ShouldNotBeNull();
            fromDb.PasswordFormat.ShouldEqual(PasswordFormat.Clear);
            fromDb.Password.ShouldEqual("password");
        }

        protected Customer GetTestCustomer()
        {
            return new Customer
            {
                Username = "a@b.com",
                Email = "a@b.com",
                CustomerGuid = Guid.NewGuid(),
                AdminComment = "some comment here",
                IsTaxExempt = true,
                AffiliateId = 1,
                VendorId = 2,
                HasShoppingCartItems = true,
                RequireReLogin = true,
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

        protected CustomerPassword GetTestCustomerPassword()
        {
            var customer = GetTestCustomer();
            return new CustomerPassword
            {
                Customer = SaveAndLoadEntity(customer),
                Password = "password",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
        }
    }
}