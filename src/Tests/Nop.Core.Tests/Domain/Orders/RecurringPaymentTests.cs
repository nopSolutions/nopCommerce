using System;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Core.Tests.Domain.Orders
{
    [TestFixture]
    public class RecurringPaymentTests
    {
        [Test]
        public void Can_calculate_nextPaymentDate_with_days_as_cycle_period()
        {
            var rp = new RecurringPayment
            {
                CycleLength = 7,
                CyclePeriod = RecurringProductCyclePeriod.Days, 
                TotalCycles = 3,
                StartDateUtc = new DateTime(2010, 3, 1),
                CreatedOnUtc = new DateTime(2010, 1, 1),
                IsActive = true,
            };

            rp.NextPaymentDate.ShouldEqual(new DateTime(2010, 3, 1));

            //add one history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldEqual(new DateTime(2010, 3, 8));
            //add one more history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldEqual(new DateTime(2010, 3, 15));
            //add one more history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldBeNull();
        }

        [Test]
        public void Can_calculate_nextPaymentDate_with_weeks_as_cycle_period()
        {
            var rp = new RecurringPayment
            {
                CycleLength = 2,
                CyclePeriod = RecurringProductCyclePeriod.Weeks,
                TotalCycles = 3,
                StartDateUtc = new DateTime(2010, 3, 1),
                CreatedOnUtc = new DateTime(2010, 1, 1),
                IsActive = true,
            };

            rp.NextPaymentDate.ShouldEqual(new DateTime(2010, 3, 1));

            //add one history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldEqual(new DateTime(2010, 3, 15));
            //add one more history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldEqual(new DateTime(2010, 3, 29));
            //add one more history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldBeNull();
        }

        [Test]
        public void Can_calculate_nextPaymentDate_with_months_as_cycle_period()
        {
            var rp = new RecurringPayment
            {
                CycleLength = 2,
                CyclePeriod = RecurringProductCyclePeriod.Months,
                TotalCycles = 3,
                StartDateUtc = new DateTime(2010, 3, 1),
                CreatedOnUtc = new DateTime(2010, 1, 1),
                IsActive = true,
            };

            rp.NextPaymentDate.ShouldEqual(new DateTime(2010, 3, 1));

            //add one history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldEqual(new DateTime(2010, 5, 1));
            //add one more history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldEqual(new DateTime(2010, 7, 1));
            //add one more history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldBeNull();
        }

        [Test]
        public void Can_calculate_nextPaymentDate_with_years_as_cycle_period()
        {
            var rp = new RecurringPayment
            {
                CycleLength = 2,
                CyclePeriod = RecurringProductCyclePeriod.Years,
                TotalCycles = 3,
                StartDateUtc = new DateTime(2010, 3, 1),
                CreatedOnUtc = new DateTime(2010, 1, 1),
                IsActive = true,
            };

            rp.NextPaymentDate.ShouldEqual(new DateTime(2010, 3, 1));

            //add one history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldEqual(new DateTime(2012, 3, 1));
            //add one more history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldEqual(new DateTime(2014, 3, 1));
            //add one more history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldBeNull();
        }

        [Test]
        public void Next_payment_date_is_null_when_recurring_payment_is_not_active()
        {
            var rp = new RecurringPayment
            {
                CycleLength = 7,
                CyclePeriod = RecurringProductCyclePeriod.Days,
                TotalCycles = 3,
                StartDateUtc = new DateTime(2010, 3, 1),
                CreatedOnUtc = new DateTime(2010, 1, 1),
                IsActive = false,
            };

            rp.NextPaymentDate.ShouldBeNull();

            //add one history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldBeNull();
            //add one more history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldBeNull();
            //add one more history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldBeNull();
        }

        [Test]
        public void Can_calculate_number_of_remaining_cycle()
        {
            var rp = new RecurringPayment
            {
                CycleLength = 2,
                CyclePeriod = RecurringProductCyclePeriod.Days,
                TotalCycles = 3,
                StartDateUtc = new DateTime(2010, 3, 1),
                CreatedOnUtc = new DateTime(2010, 1, 1),
                IsActive = true,
            };

            rp.CyclesRemaining.ShouldEqual(3);

            //add one history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.CyclesRemaining.ShouldEqual(2);
            //add one more history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.CyclesRemaining.ShouldEqual(1);
            //add one more history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.CyclesRemaining.ShouldEqual(0);
            //add one more history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.CyclesRemaining.ShouldEqual(0);
        }

    }
}
