using FluentAssertions;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Orders;
using Nop.Tests.Nop.Services.Tests.Payments;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Orders
{
    [TestFixture]
    public class OrderProcessingServiceTests : ServiceTest
    {
        private IOrderService _orderService;
        private OrderProcessingService _orderProcessingService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _orderService = GetService<IOrderService>();
            _orderProcessingService = GetService<OrderProcessingService>();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            TestPaymentMethod.TestSupportRefund = false;
            TestPaymentMethod.TestSupportCapture = false;
            TestPaymentMethod.TestSupportPartiallyRefund = false;
            TestPaymentMethod.TestSupportVoid = false;

            await GetService<IRepository<RecurringPayment>>().TruncateAsync();
        }

        [Test]
        public void EnsureOrderCanOnlyBeCancelledWhenOrderStatusIsNotCanCelledYet()
        {
            var order = new Order();
            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;
                        if (os != OrderStatus.Cancelled)
                            _orderProcessingService.CanCancelOrder(order).Should().BeTrue();
                        else
                            _orderProcessingService.CanCancelOrder(order).Should().BeFalse();
                    }
        }

        [Test]
        public void EnsureOrderCanOnlyBeMarkedAsAuthorizedWhenOrderStatusIsNotCancelledAndPaymentStatusIsPending()
        {
            var order = new Order();
            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;
                        if (os != OrderStatus.Cancelled && ps == PaymentStatus.Pending)
                            _orderProcessingService.CanMarkOrderAsAuthorized(order).Should().BeTrue();
                        else
                            _orderProcessingService.CanMarkOrderAsAuthorized(order).Should().BeFalse();
                    }
        }

        [Test]
        public async Task EnsureOrderCanOnlyBeCapturedWhenOrderStatusIsNotCancelledOrPendingAndPaymentStatusIsAuthorizedAndPaymentModuleSupportsCapture()
        {
            var order = new Order
            {
                PaymentMethodSystemName = "Payments.TestMethod"
            };

            TestPaymentMethod.TestSupportCapture = true;

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        if (os != OrderStatus.Cancelled && os != OrderStatus.Pending
                                                        && ps == PaymentStatus.Authorized)
                        {
                            var canCapture = await _orderProcessingService.CanCaptureAsync(order);
                            canCapture.Should().BeTrue();
                        }
                        else
                        {
                            var canCapture = await _orderProcessingService.CanCaptureAsync(order);
                            canCapture.Should().BeFalse();
                        }
                    }

            TestPaymentMethod.TestSupportCapture = false;

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        var canCapture = await _orderProcessingService.CanCaptureAsync(order);
                        canCapture.Should().BeFalse();
                    }
        }

        [Test]
        public void EnsureOrderCannotBeMarkedAsPaidWhenOrderStatusIsCancelledOrPaymentStatusIsPaidOrRefundedOrVoided()
        {
            var order = new Order();
            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;
                        if (os == OrderStatus.Cancelled || ps == PaymentStatus.Paid || ps == PaymentStatus.Refunded || ps == PaymentStatus.Voided)
                            _orderProcessingService.CanMarkOrderAsPaid(order).Should().BeFalse();
                        else
                            _orderProcessingService.CanMarkOrderAsPaid(order).Should().BeTrue();
                    }
        }

        [Test]
        public async Task EnsureOrderCanOnlyBeRefundedWhenPaymentStatusIsPaidAndPaymentModuleSupportsRefund()
        {
            var order = new Order
            {
                OrderTotal = 1,
                PaymentMethodSystemName = "Payments.TestMethod"
            };

            TestPaymentMethod.TestSupportRefund = true;

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        if (ps == PaymentStatus.Paid)
                        {
                            var canRefund = await _orderProcessingService.CanRefundAsync(order);
                            canRefund.Should().BeTrue();
                        }
                        else
                        {
                            var canRefund = await _orderProcessingService.CanRefundAsync(order);
                            canRefund.Should().BeFalse();
                        }
                    }

            TestPaymentMethod.TestSupportRefund = false;

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        var canRefund = await _orderProcessingService.CanRefundAsync(order);
                        canRefund.Should().BeFalse();
                    }
        }

        [Test]
        public async Task EnsureOrderCannotBeRefundedWhenOrderTotalIsZero()
        {
            var order = new Order
            {
                PaymentMethodSystemName = "Payments.TestMethod"
            };

            TestPaymentMethod.TestSupportRefund = true;

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        var canRefund = await _orderProcessingService.CanRefundAsync(order);
                        canRefund.Should().BeFalse();
                    }

            TestPaymentMethod.TestSupportRefund = false;
        }

        [Test]
        public void EnsureOrderCanOnlyBeRefundedOfflineWhenPaymentStatusIsPaid()
        {
            var order = new Order
            {
                OrderTotal = 1
            };
            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        if (ps == PaymentStatus.Paid)
                            _orderProcessingService.CanRefundOffline(order).Should().BeTrue();
                        else
                            _orderProcessingService.CanRefundOffline(order).Should().BeFalse();
                    }
        }

        [Test]
        public void EnsureOrderCannotBeRefundedOfflineWhenOrdertotalIsZero()
        {
            var order = new Order();

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        _orderProcessingService.CanRefundOffline(order).Should().BeFalse();
                    }
        }

        [Test]
        public async Task EnsureOrderCanOnlyBeVoidedWhenPaymentStatusIsAuthorizedAndPaymentModuleSupportsVoid()
        {
            var order = new Order
            {
                OrderTotal = 1,
                PaymentMethodSystemName = "Payments.TestMethod"
            };

            TestPaymentMethod.TestSupportVoid = true;

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        if (ps == PaymentStatus.Authorized)
                        {
                            var canVoid = await _orderProcessingService.CanVoidAsync(order);
                            canVoid.Should().BeTrue();
                        }
                        else
                        {
                            var canVoid = await _orderProcessingService.CanVoidAsync(order);
                            canVoid.Should().BeFalse();
                        }
                    }

            TestPaymentMethod.TestSupportVoid = false;

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        var canVoid = await _orderProcessingService.CanVoidAsync(order);
                        canVoid.Should().BeFalse();
                    }
        }

        [Test]
        public async Task EnsureOrderCannotBeVoidedWhenOrderTotalIsZero()
        {
            var order = new Order
            {
                PaymentMethodSystemName = "Payments.TestMethod"
            };

            TestPaymentMethod.TestSupportVoid = true;

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        var canVoid = await _orderProcessingService.CanVoidAsync(order);
                        canVoid.Should().BeFalse();
                    }

            TestPaymentMethod.TestSupportVoid = false;
        }

        [Test]
        public void EnsureOrderCanOnlyBeVoidedOfflineWhenPaymentStatusIsAuthorized()
        {
            var order = new Order
            {
                OrderTotal = 1
            };
            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        if (ps == PaymentStatus.Authorized)
                            _orderProcessingService.CanVoidOffline(order).Should().BeTrue();
                        else
                            _orderProcessingService.CanVoidOffline(order).Should().BeFalse();
                    }
        }

        [Test]
        public void EnsureOrderCannotBeVoidedOfflineWhenOrderTotalIsZero()
        {
            var order = new Order();

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        _orderProcessingService.CanVoidOffline(order).Should().BeFalse();
                    }
        }

        [Test]
        public async Task EnsureOrderCanOnlyBePartiallyRefundedWhenPaymentStatusIsPaidOrPartiallyRefundedAndPaymentModuleSupportsPartialRefund()
        {
            var order = new Order
            {
                OrderTotal = 100,
                PaymentMethodSystemName = "Payments.TestMethod"
            };

            TestPaymentMethod.TestSupportPartiallyRefund = true;

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        if (ps == PaymentStatus.Paid || order.PaymentStatus == PaymentStatus.PartiallyRefunded)
                        {
                            var canPartiallyRefund = await _orderProcessingService.CanPartiallyRefundAsync(order, 10);
                            canPartiallyRefund.Should().BeTrue();
                        }
                        else
                        {
                            var canPartiallyRefund = await _orderProcessingService.CanPartiallyRefundAsync(order, 10);
                            canPartiallyRefund.Should().BeFalse();
                        }
                    }

            TestPaymentMethod.TestSupportPartiallyRefund = false;

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        var canPartiallyRefund = await _orderProcessingService.CanPartiallyRefundAsync(order, 10);
                        canPartiallyRefund.Should().BeFalse();
                    }
        }

        [Test]
        public async Task EnsureOrderCannotBePartiallyRefundedWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded()
        {
            var order = new Order
            {
                OrderTotal = 100,
                RefundedAmount = 30, //100-30=70 can be refunded
                PaymentMethodSystemName = "Payments.TestMethod"
            };

            TestPaymentMethod.TestSupportPartiallyRefund = true;

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        var canPartiallyRefund = await _orderProcessingService.CanPartiallyRefundAsync(order, 80);
                        canPartiallyRefund.Should().BeFalse();
                    }

            TestPaymentMethod.TestSupportPartiallyRefund = false;
        }

        [Test]
        public void EnsureOrderCanOnlyBePartiallyRefundedOfflineWhenPaymentStatusIsPaidOrPartiallyRefunded()
        {
            var order = new Order
            {
                OrderTotal = 100
            };

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        {
                            order.OrderStatus = os;
                            order.PaymentStatus = ps;
                            order.ShippingStatus = ss;

                            if (ps == PaymentStatus.Paid || order.PaymentStatus == PaymentStatus.PartiallyRefunded)
                                _orderProcessingService.CanPartiallyRefundOffline(order, 10).Should().BeTrue();
                            else
                                _orderProcessingService.CanPartiallyRefundOffline(order, 10).Should().BeFalse();
                        }
                    }
        }

        [Test]
        public void EnsureOrderCannotBePartiallyRefundedOfflineWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded()
        {
            var order = new Order
            {
                OrderTotal = 100,
                RefundedAmount = 30 //100-30=70 can be refunded
            };

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        _orderProcessingService.CanPartiallyRefundOffline(order, 80).Should().BeFalse();
                    }
        }

        //RecurringPaymentHistory
        [Test]
        public async Task CanCalculateNextPaymentDateWithDaysAsCyclePeriod()
        {
            var rp = new RecurringPayment
            {
                CycleLength = 7,
                CyclePeriod = RecurringProductCyclePeriod.Days,
                TotalCycles = 3,
                StartDateUtc = new DateTime(2010, 3, 1),
                CreatedOnUtc = new DateTime(2010, 1, 1),
                IsActive = true,
                InitialOrderId = 1
            };

            await _orderService.InsertRecurringPaymentAsync(rp);

            var nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
            nextPaymentDate.Should().Be(new DateTime(2010, 3, 1));

            //add one history record
            await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
            nextPaymentDate.Should().Be(new DateTime(2010, 3, 8));

            //add one more history record
            await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
            nextPaymentDate.Should().Be(new DateTime(2010, 3, 15));

            //add one more history record
            await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
            nextPaymentDate.Should().BeNull();
        }

        [Test]
        public async Task CanCalculateNextPaymentDateWithWeeksAsCyclePeriod()
        {
            var rp = new RecurringPayment
            {
                CycleLength = 2,
                CyclePeriod = RecurringProductCyclePeriod.Weeks,
                TotalCycles = 3,
                StartDateUtc = new DateTime(2010, 3, 1),
                CreatedOnUtc = new DateTime(2010, 1, 1),
                IsActive = true,
                InitialOrderId = 1
            };

            await _orderService.InsertRecurringPaymentAsync(rp);

            var nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
            nextPaymentDate.Should().Be(new DateTime(2010, 3, 1));

            //add one history record
            await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
            nextPaymentDate.Should().Be(new DateTime(2010, 3, 15));
            //add one more history record
            await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
            nextPaymentDate.Should().Be(new DateTime(2010, 3, 29));
            //add one more history record
            await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
            nextPaymentDate.Should().BeNull();
        }

        [Test]
        public async Task CanCalculateNextPaymentDateWithMonthsAsCyclePeriod()
        {
            var rp = new RecurringPayment
            {
                CycleLength = 2,
                CyclePeriod = RecurringProductCyclePeriod.Months,
                TotalCycles = 3,
                StartDateUtc = new DateTime(2010, 3, 1),
                CreatedOnUtc = new DateTime(2010, 1, 1),
                IsActive = true,
                InitialOrderId = 1
            };

            await _orderService.InsertRecurringPaymentAsync(rp);

            var nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
            nextPaymentDate.Should().Be(new DateTime(2010, 3, 1));

            //add one history record
            await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
            nextPaymentDate.Should().Be(new DateTime(2010, 5, 1));
            //add one more history record
            await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
            nextPaymentDate.Should().Be(new DateTime(2010, 7, 1));
            //add one more history record
            await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
            nextPaymentDate.Should().BeNull();
        }

        [Test]
        public async Task CanCalculateNextPaymentDateWithYearsAsCyclePeriod()
        {
            var rp = new RecurringPayment
            {
                CycleLength = 2,
                CyclePeriod = RecurringProductCyclePeriod.Years,
                TotalCycles = 3,
                StartDateUtc = new DateTime(2010, 3, 1),
                CreatedOnUtc = new DateTime(2010, 1, 1),
                IsActive = true,
                InitialOrderId = 1
            };

            await _orderService.InsertRecurringPaymentAsync(rp);

            var nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
            nextPaymentDate.Should().Be(new DateTime(2010, 3, 1));

            //add one history record
            await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
            nextPaymentDate.Should().Be(new DateTime(2012, 3, 1));
            //add one more history record
            await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
            nextPaymentDate.Should().Be(new DateTime(2014, 3, 1));
            //add one more history record
            await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
            nextPaymentDate.Should().BeNull();
        }

        [Test]
        public async Task NextPaymentDateIsNullWhenRecurringPaymentIsNotActive()
        {
            var rp = new RecurringPayment
            {
                CycleLength = 7,
                CyclePeriod = RecurringProductCyclePeriod.Days,
                TotalCycles = 3,
                StartDateUtc = new DateTime(2010, 3, 1),
                CreatedOnUtc = new DateTime(2010, 1, 1),
                IsActive = false,
                InitialOrderId = 1
            };

            var nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
            nextPaymentDate.Should().BeNull();

            await _orderService.InsertRecurringPaymentAsync(rp);

            //add one history record
            await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
            nextPaymentDate.Should().BeNull();
            //add one more history record
            await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
            nextPaymentDate.Should().BeNull();
            //add one more history record
            await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
            nextPaymentDate.Should().BeNull();
        }

        [Test]
        public async Task CanCalculateNumberOfRemainingCycle()
        {
            var rp = new RecurringPayment
            {
                CycleLength = 2,
                CyclePeriod = RecurringProductCyclePeriod.Days,
                TotalCycles = 3,
                StartDateUtc = new DateTime(2010, 3, 1),
                CreatedOnUtc = new DateTime(2010, 1, 1),
                IsActive = true,
                InitialOrderId = 1
            };

            await _orderService.InsertRecurringPaymentAsync(rp);

            var cyclesRemaining = await _orderProcessingService.GetCyclesRemainingAsync(rp);
            cyclesRemaining.Should().Be(3);

            //add one history record
            await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            cyclesRemaining = await _orderProcessingService.GetCyclesRemainingAsync(rp);
            cyclesRemaining.Should().Be(2);
            //add one more history record
            await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            cyclesRemaining = await _orderProcessingService.GetCyclesRemainingAsync(rp);
            cyclesRemaining.Should().Be(1);
            //add one more history record
            await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            cyclesRemaining = await _orderProcessingService.GetCyclesRemainingAsync(rp);
            cyclesRemaining.Should().Be(0);
            //add one more history record
            await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            cyclesRemaining = await _orderProcessingService.GetCyclesRemainingAsync(rp);
            cyclesRemaining.Should().Be(0);
        }
    }
}
