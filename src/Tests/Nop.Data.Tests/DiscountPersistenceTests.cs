using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class DiscountPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_discount()
        {
            var discount = new Discount
                               {
                                   DiscountType = DiscountType.AssignedToCategories,
                                   Name = "Discount 1",
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

            var fromDb = SaveAndLoadEntity(discount);
            fromDb.ShouldNotBeNull();
            fromDb.DiscountType.ShouldEqual(DiscountType.AssignedToCategories);
            fromDb.Name.ShouldEqual("Discount 1");
            fromDb.UsePercentage.ShouldEqual(true);
            fromDb.DiscountPercentage.ShouldEqual(1);
            fromDb.DiscountAmount.ShouldEqual(2);
            fromDb.StartDateUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.EndDateUtc.ShouldEqual(new DateTime(2010, 01, 02));
            fromDb.RequiresCouponCode.ShouldEqual(true);
            fromDb.CouponCode.ShouldEqual("SecretCode");
            fromDb.DiscountLimitation.ShouldEqual(DiscountLimitationType.Unlimited);
            fromDb.LimitationTimes.ShouldEqual(3);
        }

        [Test]
        public void Can_save_and_load_discount_with_discountRequirements()
        {
            var discount = new Discount
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
                DiscountRequirements = new List<DiscountRequirement>()
                {
                     new DiscountRequirement()
                     {
                         DiscountRequirementRuleSystemName = "BillingCountryIs",
                         SpentAmount = 1,
                         BillingCountryId = 2,
                         ShippingCountryId =3,
                     }
                }
            };

            var fromDb = SaveAndLoadEntity(discount);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Discount 1");
            
            fromDb.DiscountRequirements.ShouldNotBeNull();
            (fromDb.DiscountRequirements.Count == 1).ShouldBeTrue();
            fromDb.DiscountRequirements.First().DiscountRequirementRuleSystemName.ShouldEqual("BillingCountryIs");
        }
    }
}