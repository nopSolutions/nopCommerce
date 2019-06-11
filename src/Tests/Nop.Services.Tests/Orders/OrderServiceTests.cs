using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Orders {
    
    [TestFixture]
    public class OrderServiceTests : ServiceTest {
        private Mock<IEventPublisher> _eventPublisher;
        private Mock<IRepository<Customer>> _customerRepository;
        private Mock<IRepository<Order>> _orderRepository;
        private Mock<IRepository<OrderItem>> _orderItemRepository;
        private Mock<IRepository<OrderNote>> _orderNoteRepository;
        private Mock<IRepository<Product>> _productRepository;
        private Mock<IRepository<RecurringPayment>> _recurringPaymentRepository;

        private OrderService _orderService;

        [SetUp]
        public new void SetUp() 
        {
            _eventPublisher = new Mock<IEventPublisher>();
            _customerRepository = new Mock<IRepository<Customer>>();
            _orderRepository = new Mock<IRepository<Order>>();
            _orderItemRepository = new Mock<IRepository<OrderItem>>();
            _orderNoteRepository = new Mock<IRepository<OrderNote>>();
            _productRepository = new Mock<IRepository<Product>>();
            _recurringPaymentRepository = new Mock<IRepository<RecurringPayment>>();

            _orderService = new OrderService(
                _eventPublisher.Object,
                _customerRepository.Object,
                _orderRepository.Object,
                _orderItemRepository.Object,
                _orderNoteRepository.Object,
                _productRepository.Object,
                _recurringPaymentRepository.Object);
        }

        [Test]
        public void it_should_return_null_order_when_order_id_is_zero() 
        {
            var order = _orderService.GetOrderById(0);
            order.ShouldBeNull();
        }

        [Test]
        public void it_should_return_empty_orders_when_order_ids_null() 
        {
            var orders = _orderService.GetOrdersByIds(null);
            orders.Count.ShouldEqual(0);
        }

        [Test]
        public void it_should_return_empty_orders_when_order_ids_empty() 
        {
            var orders = _orderService.GetOrdersByIds(new int[]{});
            orders.Count.ShouldEqual(0);
        }

        [Test]
        public void it_should_return_orders_by_ids() 
        {
            var order = new Order {
                Id = 1
            };

            var order2 = new Order {
                Id = 2
            };

            var order3 = new Order {
                Id = 3
            };

            _orderRepository.Setup(orderRepository => orderRepository.Table).Returns(new []{ order, order2, order3 }.AsQueryable());

            var orders = _orderService.GetOrdersByIds(new int[]{ 1, 2 });
            orders.Count.ShouldEqual(2);
        }

        [Test]
        public void it_should_return_null_when_guid_is_empty() 
        {
            var order = _orderService.GetOrderByGuid(System.Guid.Empty);
            order.ShouldBeNull();
        }

        [Test]
        public void it_should_get_order_by_guid_when_guid_not_empty() 
        {
            var knownGuid = Guid.NewGuid();
            var order = new Order {
                Id = 1,
                OrderGuid = knownGuid
            };

            var order2 = new Order {
                Id = 2,
                OrderGuid = Guid.NewGuid()
            };

            _orderRepository.Setup(orderRepository => orderRepository.Table).Returns(new []{ order, order2 }.AsQueryable());

            var foundedOrder = _orderService.GetOrderByGuid(knownGuid);
            foundedOrder.Id.ShouldEqual(order.Id);
        }

        [Test]
        public void it_should_throw_exception_if_order_is_null_when_delete_order() 
        {
            Assert.Throws(typeof(ArgumentNullException), () => _orderService.DeleteOrder(null));
        }

        [Test]
        public void it_should_delete_order() 
        {
            var order = new Order();

            _orderService.DeleteOrder(order);

            order.Deleted.ShouldBeTrue();
            _orderRepository.Verify(repo => repo.Update(order), Times.Once);
            _eventPublisher.Verify(publisher => publisher.Publish(It.Is<EntityUpdatedEvent<Order>>(e => e.Entity == order)), Times.Once);
        }

        [Test]
        public void it_should_throw_if_order_is_null_when_insert_order()
        {
            Assert.Throws(typeof(ArgumentNullException), () => _orderService.InsertOrder(null));
        }

        [Test]
        public void it_should_insert_order() 
        {
            var order = new Order();

            _orderService.InsertOrder(order);

            _orderRepository.Verify(repo => repo.Insert(order), Times.Once);
            _eventPublisher.Verify(publisher => publisher.Publish(It.Is<EntityInsertedEvent<Order>>(e => e.Entity == order)), Times.Once);
        }

        [Test]
        public void it_should_throw_if_order_is_null_when_update_order()
        {
            Assert.Throws(typeof(ArgumentNullException), () => _orderService.UpdateOrder(null));
        }

        [Test]
        public void it_should_update_order() 
        {
            var order = new Order();

            _orderService.UpdateOrder(order);

            _orderRepository.Verify(repo => repo.Update(order), Times.Once);
            _eventPublisher.Verify(publisher => publisher.Publish(It.Is<EntityUpdatedEvent<Order>>(e => e.Entity == order)), Times.Once);
        }

    
    }
}