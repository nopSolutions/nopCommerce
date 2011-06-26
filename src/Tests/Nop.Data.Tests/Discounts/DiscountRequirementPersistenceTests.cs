using System;
using Nop.Core.Domain.Discounts;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Discounts
{
    [TestFixture]
    public class DiscountRequirementPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_discountRequirement()
        {
            var discountRequirement = new DiscountRequirement
            {
                DiscountRequirementRuleSystemName = "BillingCountryIs",
                SpentAmount = 1.1M,
                BillingCountryId = 2,
                ShippingCountryId =3,
                Discount = GetTestDiscount()
            };

            var fromDb = SaveAndLoadEntity(discountRequirement);
            fromDb.ShouldNotBeNull();
            fromDb.DiscountRequirementRuleSystemName.ShouldEqual("BillingCountryIs");
            fromDb.SpentAmount.ShouldEqual(1.1M);
            fromDb.BillingCountryId.ShouldEqual(2);
            fromDb.ShippingCountryId.ShouldEqual(3);


            fromDb.Discount.ShouldNotBeNull();
            fromDb.Discount.Name.ShouldEqual("Discount 1");
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