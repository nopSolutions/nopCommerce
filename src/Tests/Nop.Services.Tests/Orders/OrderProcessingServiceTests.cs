using System;
using FluentAssertions;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Orders;
using Nop.Services.Tests.Payments;
using NUnit.Framework;

namespace Nop.Services.Tests.Orders
{
    [TestFixture]
    public class OrderProcessingServiceTests : ServiceTest
    {
        private IOrderService _orderService;
        private OrderProcessingService _orderProcessingService;

        [SetUp]
        public void SetUp()
        {
            _orderService = GetService<IOrderService>();
            _orderProcessingService = GetService<OrderProcessingService>();
        }

        [TearDown]
        public void TearDown()
        {
            TestPaymentMethod.TestSupportRefund = false;
            TestPaymentMethod.TestSupportCapture = false;
            TestPaymentMethod.TestSupportPartiallyRefund = false;
            TestPaymentMethod.TestSupportVoid = false;
            
            GetService<IRepository<RecurringPayment>>().Truncate();
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
        public void EnsureOrderCanOnlyBeCapturedWhenOrderStatusIsNotCancelledOrPendingAndPaymentStatusIsAuthorizedAndPaymentModuleSupportsCapture()
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
                            _orderProcessingService.CanCapture(order).Should().BeTrue();
                        else
                            _orderProcessingService.CanCapture(order).Should().BeFalse();
                    }

            TestPaymentMethod.TestSupportCapture = false;

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        _orderProcessingService.CanCapture(order).Should().BeFalse();
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
        public void EnsureOrderCanOnlyBeRefundedWhenPaymentStatusIsPaidAndPaymentModuleSupportsRefund()
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
                            _orderProcessingService.CanRefund(order).Should().BeTrue();
                        else
                            _orderProcessingService.CanRefund(order).Should().BeFalse();
                    }

            TestPaymentMethod.TestSupportRefund = false;

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        _orderProcessingService.CanRefund(order).Should().BeFalse();
                    }
        }

        [Test]
        public void EnsureOrderCannotBeRefundedWhenOrderTotalIsZero()
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

                        _orderProcessingService.CanRefund(order).Should().BeFalse();
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
        public void EnsureOrderCanOnlyBeVoidedWhenPaymentStatusIsAuthorizedAndPaymentModuleSupportsVoid()
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
                            _orderProcessingService.CanVoid(order).Should().BeTrue();
                        else
                            _orderProcessingService.CanVoid(order).Should().BeFalse();
                    }

            TestPaymentMethod.TestSupportVoid = false;

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        _orderProcessingService.CanVoid(order).Should().BeFalse();
                    }
        }

        [Test]
        public void EnsureOrderCannotBeVoidedWhenOrderTotalIsZero()
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

                        _orderProcessingService.CanVoid(order).Should().BeFalse();
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
        public void EnsureOrderCanOnlyBePartiallyRefundedWhenPaymentStatusIsPaidOrPartiallyRefundedAndPaymentModuleSupportsPartialRefund()
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
                            _orderProcessingService.CanPartiallyRefund(order, 10).Should().BeTrue();
                        else
                            _orderProcessingService.CanPartiallyRefund(order, 10).Should().BeFalse();
                    }

            TestPaymentMethod.TestSupportPartiallyRefund = false;

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        _orderProcessingService.CanPartiallyRefund(order, 10).Should().BeFalse();
                    }
        }

        [Test]
        public void EnsureOrderCannotBePartiallyRefundedWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded()
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

                        _orderProcessingService.CanPartiallyRefund(order, 80).Should().BeFalse();
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
        public void CanCalculateNextPaymentDateWithDaysAsCyclePeriod()
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

            _orderService.InsertRecurringPayment(rp);

            _orderProcessingService.GetNextPaymentDate(rp).Should().Be(new DateTime(2010, 3, 1));

            //add one history record
            _orderService.InsertRecurringPaymentHistory(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            _orderProcessingService.GetNextPaymentDate(rp).Should().Be(new DateTime(2010, 3, 8));

            //add one more history record
            _orderService.InsertRecurringPaymentHistory(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            _orderProcessingService.GetNextPaymentDate(rp).Should().Be(new DateTime(2010, 3, 15));

            //add one more history record
            _orderService.InsertRecurringPaymentHistory(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            _orderProcessingService.GetNextPaymentDate(rp).Should().BeNull();
        }

        [Test]
        public void CanCalculateNextPaymentDateWithWeeksAsCyclePeriod()
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

            _orderService.InsertRecurringPayment(rp);

            _orderProcessingService.GetNextPaymentDate(rp).Should().Be(new DateTime(2010, 3, 1));

            //add one history record
            _orderService.InsertRecurringPaymentHistory(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            _orderProcessingService.GetNextPaymentDate(rp).Should().Be(new DateTime(2010, 3, 15));
            //add one more history record
            _orderService.InsertRecurringPaymentHistory(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            _orderProcessingService.GetNextPaymentDate(rp).Should().Be(new DateTime(2010, 3, 29));
            //add one more history record
            _orderService.InsertRecurringPaymentHistory(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            _orderProcessingService.GetNextPaymentDate(rp).Should().BeNull();
        }

        [Test]
        public void CanCalculateNextPaymentDateWithMonthsAsCyclePeriod()
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

            _orderService.InsertRecurringPayment(rp);

            _orderProcessingService.GetNextPaymentDate(rp).Should().Be(new DateTime(2010, 3, 1));

            //add one history record
            _orderService.InsertRecurringPaymentHistory(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            _orderProcessingService.GetNextPaymentDate(rp).Should().Be(new DateTime(2010, 5, 1));
            //add one more history record
            _orderService.InsertRecurringPaymentHistory(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            _orderProcessingService.GetNextPaymentDate(rp).Should().Be(new DateTime(2010, 7, 1));
            //add one more history record
            _orderService.InsertRecurringPaymentHistory(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            _orderProcessingService.GetNextPaymentDate(rp).Should().BeNull();
        }

        [Test]
        public void CanCalculateNextPaymentDateWithYearsAsCyclePeriod()
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

            _orderService.InsertRecurringPayment(rp);

            _orderProcessingService.GetNextPaymentDate(rp).Should().Be(new DateTime(2010, 3, 1));

            //add one history record
            _orderService.InsertRecurringPaymentHistory(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            _orderProcessingService.GetNextPaymentDate(rp).Should().Be(new DateTime(2012, 3, 1));
            //add one more history record
            _orderService.InsertRecurringPaymentHistory(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            _orderProcessingService.GetNextPaymentDate(rp).Should().Be(new DateTime(2014, 3, 1));
            //add one more history record
            _orderService.InsertRecurringPaymentHistory(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            _orderProcessingService.GetNextPaymentDate(rp).Should().BeNull();
        }

        [Test]
        public void NextPaymentDateIsNullWhenRecurringPaymentIsNotActive()
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

            _orderProcessingService.GetNextPaymentDate(rp).Should().BeNull();

            _orderService.InsertRecurringPayment(rp);

            //add one history record
            _orderService.InsertRecurringPaymentHistory(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            _orderProcessingService.GetNextPaymentDate(rp).Should().BeNull();
            //add one more history record
            _orderService.InsertRecurringPaymentHistory(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            _orderProcessingService.GetNextPaymentDate(rp).Should().BeNull();
            //add one more history record
            _orderService.InsertRecurringPaymentHistory(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            _orderProcessingService.GetNextPaymentDate(rp).Should().BeNull();
        }

        [Test]
        public void CanCalculateNumberOfRemainingCycle()
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

            _orderService.InsertRecurringPayment(rp);

            _orderProcessingService.GetCyclesRemaining(rp).Should().Be(3);

            //add one history record
            _orderService.InsertRecurringPaymentHistory(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            _orderProcessingService.GetCyclesRemaining(rp).Should().Be(2);
            //add one more history record
            _orderService.InsertRecurringPaymentHistory(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            _orderProcessingService.GetCyclesRemaining(rp).Should().Be(1);
            //add one more history record
            _orderService.InsertRecurringPaymentHistory(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            _orderProcessingService.GetCyclesRemaining(rp).Should().Be(0);
            //add one more history record
            _orderService.InsertRecurringPaymentHistory(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
            _orderProcessingService.GetCyclesRemaining(rp).Should().Be(0);
        }
    }
}
