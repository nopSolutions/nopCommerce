using FluentAssertions;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Orders;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Orders
{

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
        public async Task ItShouldReturnNullOrderWhenOrderIdIsZero()
        {
            var order = await _orderService.GetOrderByIdAsync(0);
            order.Should().BeNull();
        }

        [Test]
        public async Task ItShouldReturnEmptyOrdersWhenOrderIdsNull()
        {
            var orders = await _orderService.GetOrdersByIdsAsync(null);
            orders.Count.Should().Be(0);
        }

        [Test]
        public async Task ItShouldReturnEmptyOrdersWhenOrderIdsEmpty()
        {
            var orders = await _orderService.GetOrdersByIdsAsync(Array.Empty<int>());
            orders.Count.Should().Be(0);
        }

        [Test]
        public async Task ItShouldReturnOrdersByIds()
        {
            var orders = await _orderService.GetOrdersByIdsAsync(new[] { 1, 2 });
            orders.Count.Should().Be(2);
        }

        [Test]
        public async Task ItShouldReturnNullWhenGuidIsEmpty()
        {
            var order = await _orderService.GetOrderByGuidAsync(Guid.Empty);
            order.Should().BeNull();
        }

        [Test]
        public async Task ItShouldGetOrderByGuidWhenGuidNotEmpty()
        {
            var order = await _orderService.GetOrderByIdAsync(1);

            var foundedOrder = await _orderService.GetOrderByGuidAsync(order.OrderGuid);
            foundedOrder.Id.Should().Be(order.Id);
        }

        [Test]
        public void ItShouldThrowExceptionIfOrderIsNullWhenDeleteOrder()
        {
            Assert.Throws(typeof(AggregateException), () => _orderService.DeleteOrderAsync(null).Wait());
        }

        [Test]
        public async Task ItShouldDeleteOrder()
        {
            var order = await _orderService.GetOrderByIdAsync(1);
            await _orderService.DeleteOrderAsync(order);

            order.Deleted.Should().BeTrue();
            order = await GetService<IRepository<Order>>().GetByIdAsync(1);
            order.Should().NotBeNull();
            order.Deleted = false;
            await _orderService.UpdateOrderAsync(order);
        }

        [Test]
        public void ItShouldThrowIfOrderIsNullWhenInsertOrder()
        {
            Assert.Throws<AggregateException>(() => _orderService.InsertOrderAsync(null).Wait());
        }

        [Test]
        public async Task ItShouldInsertOrder()
        {
            var order = new Order
            {
                CustomOrderNumber = string.Empty,
                CustomerId = 1,
                BillingAddressId = 1
            };

            await _orderService.InsertOrderAsync(order);
            order.Id.Should().BeGreaterThan(0);
            await GetService<IRepository<Order>>().DeleteAsync(order);
        }

        [Test]
        public void ItShouldThrowIfOrderIsNullWhenUpdateOrder()
        {
            Assert.Throws<AggregateException>(() => _orderService.UpdateOrderAsync(null).Wait());
        }
    }
}