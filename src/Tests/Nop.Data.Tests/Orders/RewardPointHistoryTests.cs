using System;
using System.Linq;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Orders
{
    [TestFixture]
    public class RewardPointHistoryTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_rewardPointHistory()
        {
            var rph = new RewardPointsHistory
            {
                StoreId = 1,
                Customer = GetTestCustomer(),
                Points = 2,
                PointsBalance = 3,
                UsedAmount = 4,
                Message = "Message 5",
                CreatedOnUtc= new DateTime(2010, 01, 04)
            };

            var fromDb = SaveAndLoadEntity(rph);
            fromDb.ShouldNotBeNull();
            fromDb.StoreId.ShouldEqual(1);
            fromDb.Customer.ShouldNotBeNull();
            fromDb.Points.ShouldEqual(2);
            fromDb.PointsBalance.ShouldEqual(3);
            fromDb.UsedAmount.ShouldEqual(4);
            fromDb.Message.ShouldEqual("Message 5");
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 04));
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
