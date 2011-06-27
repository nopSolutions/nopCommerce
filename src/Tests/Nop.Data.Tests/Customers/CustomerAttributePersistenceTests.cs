using System;
using Nop.Core.Domain.Customers;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Customers
{
    [TestFixture]
    public class CustomerAttributePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_customerAttribute()
        {
            var customerAttribute = new CustomerAttribute
                               {
                                   Key = "Key 1",
                                   Value = "Value 1",
                                   Customer = GetTestCustomer()
                               };

            var fromDb = SaveAndLoadEntity(customerAttribute);
            fromDb.ShouldNotBeNull();
            fromDb.Key.ShouldEqual("Key 1");
            fromDb.Value.ShouldEqual("Value 1");

            fromDb.Customer.ShouldNotBeNull();
        }
        
        protected Customer GetTestCustomer()
        {
            return new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                AdminComment = "some comment here",
                Active = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                LastActivityDateUtc = new DateTime(2010, 01, 02)
            };
        }
    }
}