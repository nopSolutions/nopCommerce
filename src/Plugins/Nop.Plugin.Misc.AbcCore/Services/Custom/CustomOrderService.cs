using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Plugin.Misc.AbcCore.Extensions;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcExportOrder.Services
{
    public class CustomOrderService : OrderService, ICustomOrderService
    {
        private readonly IRepository<Order> _orderRepository;

        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeService _productAttributeService;

        public CustomOrderService(
            IProductService productService,
            IRepository<Address> addressRepository,
            IRepository<Customer> customerRepository,
            IRepository<Order> orderRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<OrderNote> orderNoteRepository,
            IRepository<Product> productRepository,
            IRepository<ProductWarehouseInventory> productWarehouseInventoryRepository,
            IRepository<RecurringPayment> recurringPaymentRepository,
            IRepository<RecurringPaymentHistory> recurringPaymentHistoryRepository,
            IShipmentService shipmentService,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService
        ) : base(productService, addressRepository, customerRepository,
                 orderRepository, orderItemRepository, orderNoteRepository,
                 productRepository, productWarehouseInventoryRepository,
                 recurringPaymentRepository, recurringPaymentHistoryRepository,
                 shipmentService)
        {
            _orderRepository = orderRepository;
            _productAttributeParser = productAttributeParser;
            _productAttributeService = productAttributeService;
        }

        public async Task<ProductAttributeValue> GetOrderItemWarrantyAsync(OrderItem orderItem)
        {
            if (!orderItem.HasWarranty()) { return null; }

            var productAttributeValues =
                await _productAttributeParser.ParseProductAttributeValuesAsync(
                    orderItem.AttributesXml
                );

            foreach (var pav in productAttributeValues)
            {
                var pam = await _productAttributeService.GetProductAttributeMappingByIdAsync(pav.ProductAttributeMappingId);
                var pa = await _productAttributeService.GetProductAttributeByIdAsync(pam.ProductAttributeId);
                if (IsWarranty(pa)) { return pav; }
            }

            return null;
        }

        public IList<Order> GetUnsubmittedOrders()
        {
            var lastMonth = DateTime.Today.AddMonths(-1);
            return _orderRepository.Table.Where(o => o.CreatedOnUtc > lastMonth && o.CardNumber != null && !o.Deleted).ToList();
        }

        private static bool IsWarranty(ProductAttribute productAttribute)
        {
            return productAttribute.Name == "Warranty";
        }
    }
}
