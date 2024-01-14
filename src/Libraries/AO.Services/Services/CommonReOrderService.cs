using AO.Services.Orders.Models;
using AO.Services.Products;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Vendors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AO.Services.Services
{
    public class CommonReOrderService : ICommonReOrderService
    {
        private List<Vendor> _allVendors;
        private List<Manufacturer> _allManufacturers;
        private readonly IRepository<AOReOrderItem> _aoReOrderItemsRepository;
        private readonly IOrderService _orderService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductService _productService;
        private readonly ILogger _logger;
        private readonly IManufacturerService _manufacturerService;

        public CommonReOrderService(IManufacturerService manufacturerService, IVendorService vendorService, IRepository<AOReOrderItem> aoReOrderItemsRepository, IOrderService orderService, IProductAttributeParser productAttributeParser, IProductService productService, ILogger logger)
        {
            this._allManufacturers = manufacturerService.GetAllManufacturersAsync().Result.ToList();
            this._allVendors = vendorService.GetAllVendorsAsync().Result.ToList();
            this._aoReOrderItemsRepository = aoReOrderItemsRepository;
            _orderService = orderService;
            _productAttributeParser = productAttributeParser;
            _productService = productService;
            _logger = logger;
            _manufacturerService = manufacturerService;
        }

        public async Task<int> ChangeQuantityAsync(int reOrderItemId, int quantity)
        {
            var reOrderItem = await GetReOrderItemAsync(reOrderItemId);
            if (reOrderItem == null)
            {
                throw new ArgumentException("No AOReOrderItem found with id: " + reOrderItemId);
            }
            
            reOrderItem.Quantity += quantity; // When negative we decrease
            await _aoReOrderItemsRepository.UpdateAsync(reOrderItem, false);
            return reOrderItem.Quantity;
        }

        public async Task DeleteReOrderItemAsync(int reOrderItemId)
        {
            var reOrderItem = await _aoReOrderItemsRepository.GetByIdAsync(reOrderItemId);
            if (reOrderItem != null)
            {
                await _aoReOrderItemsRepository.DeleteAsync(reOrderItem);
            }
        }

        public async Task RemoveFromReOrderListAsync(int quantityToOrder, int orderItemId)
        {
            var reOrderItems = await _aoReOrderItemsRepository.GetAllAsync(q => q.Where(r => r.OrderItemId == orderItemId));
            if (reOrderItems == null || reOrderItems.Count <= 0)
            {
                // Nothing found on re-orderlist, maybe it has been moved directly from that list
                return;
            }

            var reOrderItem = reOrderItems.FirstOrDefault();
            if (quantityToOrder > reOrderItem.Quantity)
            {
                // If we wanna reduce with more than is on reorderlist.
                // This can happen if we had some in stock on order time
                // But we only have the total quantity from the orderitem here.
                quantityToOrder = reOrderItem.Quantity;
            }

            reOrderItem.Quantity -= quantityToOrder;
            reOrderItem.OrderedQuantity = quantityToOrder;
            await _aoReOrderItemsRepository.UpdateAsync(reOrderItem, false);
        }

        public async Task ReAddToReOrderListAsync(int orderItemId)
        {
            var reOrderItems = await _aoReOrderItemsRepository.GetAllAsync(q => q.Where(r => r.OrderItemId == orderItemId));            
            var reOrderItem = reOrderItems.FirstOrDefault();

            if (reOrderItem == null)
            {
                var orderItem = await _orderService.GetOrderItemByIdAsync(orderItemId);
                if (orderItem == null)
                {
                    await _logger.ErrorAsync($"No OrderItem found when trying to create ReOrderItem. orderItemId: {orderItemId}");
                    return;
                }

                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
                if (product == null)
                {
                    await _logger.ErrorAsync($"No product found when trying to create ReOrderItem. productId: {orderItem.ProductId}");
                    return;
                }

                var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, orderItem.AttributesXml);
                if (combination == null)
                {
                    await _logger.ErrorAsync($"No combination found when trying to create ReOrderItem. productId: {orderItem.ProductId}, orderItem.AttributesXml: {orderItem.AttributesXml}");
                    return;
                }

                reOrderItem = await CreateNewReOrderItem(reOrderItem, orderItem, product, combination);
            }
            else
            {
                reOrderItem.Quantity = (reOrderItem.OrderedQuantity.HasValue && reOrderItem.OrderedQuantity.Value > 0) ? reOrderItem.Quantity += reOrderItem.OrderedQuantity.Value : 1;
                reOrderItem.OrderedQuantity = 0;
                await _aoReOrderItemsRepository.UpdateAsync(reOrderItem, false);
            }
        }

        private async Task<AOReOrderItem> CreateNewReOrderItem(AOReOrderItem reOrderItem, OrderItem orderItem, Product product, ProductAttributeCombination combination)
        {
            int manufacturerId = 0;
            var productManufacturers = await _manufacturerService.GetProductManufacturersByProductIdAsync(orderItem.ProductId);
            if (productManufacturers != null && productManufacturers.Count > 0)
            {
                manufacturerId = productManufacturers.FirstOrDefault().ManufacturerId;
            }

            reOrderItem = new AOReOrderItem()
            {
                EAN = combination == null ? product.Gtin ?? "" : combination.Gtin,
                ManufacturerId = manufacturerId,
                ManufacturerProductId = product.ManufacturerPartNumber,
                OrderItemId = orderItem.Id,
                ProductId = product.Id,
                ProductName = product.Name + orderItem.AttributeDescription,
                Quantity = orderItem.Quantity,
                VendorId = product.VendorId
            };

            await _aoReOrderItemsRepository.InsertAsync(reOrderItem);
            return reOrderItem;
        }

        public async Task<AOReOrderItem> GetReOrderItemAsync(int reOrderItemId)
        {
            var reOrderItem = await _aoReOrderItemsRepository.GetByIdAsync(reOrderItemId);
            if (reOrderItem == null)
            {
                throw new ArgumentException("No AOReOrderItem found with id: " + reOrderItemId);
            }
            return reOrderItem;
        }

        public async Task<AOReOrderItem> GetReOrderItemByOrderItemIdAsync(int orderItemId)
        {
            var rep = await _aoReOrderItemsRepository.GetAllAsync(query =>
            {
                return from reOrderItem in query
                       where reOrderItem.OrderItemId == orderItemId
                       select reOrderItem;
            });

            return rep.FirstOrDefault();
        }
    }
}