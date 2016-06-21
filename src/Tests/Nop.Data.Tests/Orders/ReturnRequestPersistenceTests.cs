using System;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Orders
{
    [TestFixture]
    public class ReturnRequestPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_returnRequest()
        {
            var rr = new ReturnRequest
            {
                CustomNumber = "CustomNumber 1",
                StoreId = 1,
                Customer = GetTestCustomer(),
                Quantity = 2,
                ReasonForReturn = "Wrong product",
                RequestedAction = "Refund",
                CustomerComments = "Some comment",
                StaffNotes = "Some notes",
                ReturnRequestStatus = ReturnRequestStatus.ItemsRefunded,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };

            var fromDb = SaveAndLoadEntity(rr);
            fromDb.ShouldNotBeNull();
            fromDb.CustomNumber.ShouldEqual("CustomNumber 1");
            fromDb.StoreId.ShouldEqual(1);
            fromDb.Customer.ShouldNotBeNull();
            fromDb.Quantity.ShouldEqual(2);
            fromDb.ReasonForReturn.ShouldEqual("Wrong product");
            fromDb.RequestedAction.ShouldEqual("Refund");
            fromDb.CustomerComments.ShouldEqual("Some comment");
            fromDb.StaffNotes.ShouldEqual("Some notes");
            fromDb.ReturnRequestStatus.ShouldEqual(ReturnRequestStatus.ItemsRefunded);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.UpdatedOnUtc.ShouldEqual(new DateTime(2010, 01, 02));
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

        protected Order GetTestOrder()
        {
            return new Order
            {
                OrderGuid = Guid.NewGuid(),
                Customer = GetTestCustomer(),
                BillingAddress = new Address
                {
                    Country = new Country
                    {
                        Name = "United States",
                        TwoLetterIsoCode = "US",
                        ThreeLetterIsoCode = "USA",
                    },
                    CreatedOnUtc = new DateTime(2010, 01, 01),
                },
                Deleted = true,
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
        }

    }
}