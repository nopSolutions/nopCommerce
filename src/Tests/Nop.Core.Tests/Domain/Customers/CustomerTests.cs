using System.Linq;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Core.Tests.Domain.Customers
{
    [TestFixture]
    public class CustomerTests
    {
        [Test]
        public void New_customer_has_clear_password_type()
        {
            var customer = new Customer();
            customer.PasswordFormat.ShouldEqual(PasswordFormat.Clear);
        }

        [Test]
        public void Can_add_address()
        {
            var customer = new Customer();
            var address = new Address { Id = 1 };

            customer.Addresses.Add(address);

            customer.Addresses.Count.ShouldEqual(1);
            customer.Addresses.First().Id.ShouldEqual(1);
        }
        
        [Test]
        public void Can_remove_address_assigned_as_billing_address()
        {
            var customer = new Customer();
            var address = new Address { Id = 1 };

            customer.Addresses.Add(address);
            customer.BillingAddress  = address;

            customer.BillingAddress.ShouldBeTheSameAs(customer.Addresses.First());

            customer.RemoveAddress(address);
            customer.Addresses.Count.ShouldEqual(0);
            customer.BillingAddress.ShouldBeNull();
        }

        [Test]
        public void Can_add_rewardPointsHistoryEntry()
        {
            var customer = new Customer();
            customer.AddRewardPointsHistoryEntry(1, "Points for registration");

            customer.RewardPointsHistory.Count.ShouldEqual(1);
            customer.RewardPointsHistory.First().Points.ShouldEqual(1);
        }

        [Test]
        public void Can_get_rewardPointsHistoryBalance()
        {
            var customer = new Customer();
            customer.AddRewardPointsHistoryEntry(1, "Points for registration");
            //customer.AddRewardPointsHistoryEntry(3, "Points for registration");

            customer.GetRewardPointsBalance().ShouldEqual(1);
        }

        [Test]
        public void Can_get_add_remove_giftCardCouponCodes()
        {
            var customer = new Customer();
            customer.ApplyGiftCardCouponCode("code1");
            customer.ApplyGiftCardCouponCode("code2");
            customer.RemoveGiftCardCouponCode("code2");
            customer.ApplyGiftCardCouponCode("code3");

            var codes = customer.ParseAppliedGiftCardCouponCodes();
            codes.Length.ShouldEqual(2);
            codes[0].ShouldEqual("code1");
            codes[1].ShouldEqual("code3");
        }
        [Test]
        public void Can_not_add_duplicate_giftCardCouponCodes()
        {
            var customer = new Customer();
            customer.ApplyGiftCardCouponCode("code1");
            customer.ApplyGiftCardCouponCode("code2");
            customer.ApplyGiftCardCouponCode("code1");

            var codes = customer.ParseAppliedGiftCardCouponCodes();
            codes.Length.ShouldEqual(2);
            codes[0].ShouldEqual("code1");
            codes[1].ShouldEqual("code2");
        }
    }
}
