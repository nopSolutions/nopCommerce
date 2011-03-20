using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class RecurringPaymentPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_recurringPayment()
        {
            var rp = new RecurringPayment()
            {
                CycleLength = 1,
                CyclePeriod = RecurringProductCyclePeriod.Days,
                TotalCycles= 3, 
                StartDateUtc = new DateTime(2010, 01, 01),
                IsActive= true,
                Deleted = true,
                CreatedOnUtc = new DateTime(2010, 01, 02),
                InitialOrder = GetTestOrder()
            };

            var fromDb = SaveAndLoadEntity(rp);
            fromDb.ShouldNotBeNull();
            fromDb.CycleLength.ShouldEqual(1);
            fromDb.CyclePeriod.ShouldEqual(RecurringProductCyclePeriod.Days);
            fromDb.TotalCycles.ShouldEqual(3);
            fromDb.StartDateUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.IsActive.ShouldEqual(true);
            fromDb.Deleted.ShouldEqual(true);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 02));

            fromDb.InitialOrder.ShouldNotBeNull();
        }

        [Test]
        public void Can_save_and_load_recurringPayment_with_history()
        {
            var rp = new RecurringPayment()
            {
                CycleLength = 1,
                CyclePeriod = RecurringProductCyclePeriod.Days,
                TotalCycles = 3,
                StartDateUtc = new DateTime(2010, 01, 01),
                IsActive = true,
                Deleted = true,
                CreatedOnUtc = new DateTime(2010, 01, 02),
                InitialOrder = GetTestOrder(),
                RecurringPaymentHistory = new List<RecurringPaymentHistory>()
                {
                    new RecurringPaymentHistory()
                    {
                        CreatedOnUtc = new DateTime(2010, 01, 03),
                        Order = GetTestOrder()
                    }
                }
            };

            var fromDb = SaveAndLoadEntity(rp);
            fromDb.ShouldNotBeNull();

            fromDb.RecurringPaymentHistory.ShouldNotBeNull();
            (fromDb.RecurringPaymentHistory.Count == 1).ShouldBeTrue();
            fromDb.RecurringPaymentHistory.First().CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 03));
        }

        protected Customer GetTestCustomer()
        {
            return new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                AdminComment = "some comment here",
                Active = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
        }

        protected Order GetTestOrder()
        {
            return new Order()
            {
                OrderGuid = Guid.NewGuid(),
                Customer = GetTestCustomer(),
                BillingAddress = new Address()
                {
                    Country = new Country()
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