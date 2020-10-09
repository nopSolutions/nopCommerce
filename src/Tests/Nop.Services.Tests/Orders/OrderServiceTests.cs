using System;
using FluentAssertions;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Orders;
using NUnit.Framework;

namespace Nop.Services.Tests.Orders {
    
    [TestFixture]
    public class OrderServiceTests : ServiceTest 
    {
        private IOrderService _orderService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _orderService = GetService<IOrderService>();
        }

        [Test]
        public void ItShouldReturnNullOrderWhenOrderIdIsZero() 
        {
            var order = _orderService.GetOrderById(0);
            order.Should().BeNull();
        }

        [Test]
        public void ItShouldReturnEmptyOrdersWhenOrderIdsNull() 
        {
            var orders = _orderService.GetOrdersByIds(null);
            orders.Count.Should().Be(0);
        }

        [Test]
        public void ItShouldReturnEmptyOrdersWhenOrderIdsEmpty() 
        {
            var orders = _orderService.GetOrdersByIds(new int[]{});
            orders.Count.Should().Be(0);
        }

        [Test]
        public void ItShouldReturnOrdersByIds() 
        {
            var orders = _orderService.GetOrdersByIds(new[]{ 1, 2 });
            orders.Count.Should().Be(2);
        }

        [Test]
        public void ItShouldReturnNullWhenGuidIsEmpty() 
        {
            var order = _orderService.GetOrderByGuid(Guid.Empty);
            order.Should().BeNull();
        }

        [Test]
        public void ItShouldGetOrderByGuidWhenGuidNotEmpty()
        {
            var order = _orderService.GetOrderById(1);

            var foundedOrder = _orderService.GetOrderByGuid(order.OrderGuid);
            foundedOrder.Id.Should().Be(order.Id);
        }

        [Test]
        public void ItShouldThrowExceptionIfOrderIsNullWhenDeleteOrder() 
        {
            Assert.Throws(typeof(ArgumentNullException), () => _orderService.DeleteOrder(null));
        }

        [Test]
        public void ItShouldDeleteOrder()
        {
            var order = _orderService.GetOrderById(1);
            _orderService.DeleteOrder(order);

            order.Deleted.Should().BeTrue();
            order = GetService<IRepository<Order>>().GetById(1);
            order.Should().NotBeNull();
            order.Deleted = false;
            _orderService.UpdateOrder(order);
        }

        [Test]
        public void ItShouldThrowIfOrderIsNullWhenInsertOrder()
        {
            Assert.Throws<ArgumentNullException>(() => _orderService.InsertOrder(null));
        }

        [Test]
        public void ItShouldInsertOrder() 
        {
            var order = new Order
            {
                CustomOrderNumber = string.Empty,
                CustomerId = 1,
                BillingAddressId = 1
            };

            _orderService.InsertOrder(order);
            order.Id.Should().BeGreaterThan(0);
            GetService<IRepository<Order>>().Delete(order);
        }

        [Test]
        public void ItShouldThrowIfOrderIsNullWhenUpdateOrder()
        {
            Assert.Throws<ArgumentNullException>(() => _orderService.UpdateOrder(null));
        }
    }
}