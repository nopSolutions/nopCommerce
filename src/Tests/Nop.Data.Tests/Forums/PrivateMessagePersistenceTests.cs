using System;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Stores;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Forums
{
    [TestFixture]
    public class PrivateMessagePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_privatemessage()
        {
            var store = GetTestStore();
            var storeFromDb = SaveAndLoadEntity(store);
            storeFromDb.ShouldNotBeNull();

            var customer1 = GetTestCustomer();
            var customer1FromDb = SaveAndLoadEntity(customer1);
            customer1FromDb.ShouldNotBeNull();

            var customer2 = GetTestCustomer();
            var customer2FromDb = SaveAndLoadEntity(customer2);
            customer2FromDb.ShouldNotBeNull();

            var privateMessage = new PrivateMessage
            {
                Subject = "Private Message 1 Subject",
                Text = "Private Message 1 Text",
                IsDeletedByAuthor = false,
                IsDeletedByRecipient = false,
                IsRead = false,
                CreatedOnUtc = DateTime.UtcNow,
                FromCustomerId = customer1FromDb.Id,
                ToCustomerId = customer2FromDb.Id,
                StoreId = store.Id,
            };

            var fromDb = SaveAndLoadEntity(privateMessage);
            fromDb.ShouldNotBeNull();
            fromDb.Subject.ShouldEqual("Private Message 1 Subject");
            fromDb.Text.ShouldEqual("Private Message 1 Text");
            fromDb.IsDeletedByAuthor.ShouldBeFalse();
            fromDb.IsDeletedByRecipient.ShouldBeFalse();
            fromDb.IsRead.ShouldBeFalse();
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
