using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Tests;
using NUnit.Framework;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class DiscountUsageHistoryPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_discountUsageHistory()
        {
            var discount = new DiscountUsageHistory()
                    {
                        Discount = GetTestDiscount(),
                        Order = GetTestOrder(),
                        CreatedOnUtc = new DateTime(2010, 01, 01)
                    };

            var fromDb = SaveAndLoadEntity(discount);
            fromDb.ShouldNotBeNull();
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));

            fromDb.Discount.ShouldNotBeNull();
            fromDb.Order.ShouldNotBeNull();
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
            return new Order
            {
                OrderGuid = Guid.NewGuid(),
                Customer = GetTestCustomer(),
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
        }

        protected Discount GetTestDiscount()
        {
            return new Discount
            {
                Name = "Discount 1",
                DiscountType = DiscountType.AssignedToCategories,
                UsePercentage = true,
                DiscountPercentage = 1,
                DiscountAmount = 2,
                StartDateUtc = new DateTime(2010, 01, 01),
                EndDateUtc = new DateTime(2010, 01, 02),
                RequiresCouponCode = true,
                CouponCode = "SecretCode",
                DiscountLimitation = DiscountLimitationType.Unlimited,
                LimitationTimes = 3,
            };
        }
    }
}