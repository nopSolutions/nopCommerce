using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Api.Rest.Models;

namespace Nop.Plugin.Api.Rest.Mappings
{
    public static class DtoMappings
    {
        public static MeDto ToMeDto(this Customer customer)
        {
            if (customer == null) return null!;
            return new MeDto
            {
                Id = customer.Id,
                Email = customer.Email,
                Username = customer.Username,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Company = customer.Company,
                Phone = customer.Phone,
                Active = customer.Active,
                CreatedOnUtc = customer.CreatedOnUtc,
                LastActivityDateUtc = customer.LastActivityDateUtc
            };
        }

        public static CustomerWishlistDto ToDto(this CustomWishlist wishlist, int itemCount = 0)
        {
            if (wishlist == null) return null!;
            return new CustomerWishlistDto
            {
                Id = wishlist.Id,
                CustomerId = wishlist.CustomerId,
                Name = wishlist.Name,
                ItemCount = itemCount,
                CreatedOnUtc = wishlist.CreatedOnUtc
            };
        }

        public static WishlistItemDto ToDto(this ShoppingCartItem item, Product product = null)
        {
            if (item == null) return null!;
            return new WishlistItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = product?.Name,
                ProductSku = product?.Sku,
                Quantity = item.Quantity,
                WishlistId = item.CustomWishlistId,
                StoreId = item.StoreId,
                AttributesXml = item.AttributesXml,
                CustomerEnteredPrice = item.CustomerEnteredPrice,
                RentalStartDateUtc = item.RentalStartDateUtc,
                RentalEndDateUtc = item.RentalEndDateUtc,
                CreatedOnUtc = item.CreatedOnUtc,
                UpdatedOnUtc = item.UpdatedOnUtc
            };
        }

        public static ProductAttributeCombinationSummaryDto ToDto(this ProductAttributeCombination combination)
        {
            if (combination == null) return null!;
            return new ProductAttributeCombinationSummaryDto
            {
                Id = combination.Id,
                Sku = combination.Sku,
                Gtin = combination.Gtin,
                ManufacturerPartNumber = combination.ManufacturerPartNumber,
                StockQuantity = combination.StockQuantity,
                AllowOutOfStockOrders = combination.AllowOutOfStockOrders,
                OverriddenPrice = combination.OverriddenPrice,
                AttributesXml = combination.AttributesXml
            };
        }

        /// <summary>
        /// Builds the storefront projection of a product. Takes the variants, the manufacturer mappings
        /// and the tags separately because each lives on another record.
        /// </summary>
        public static StoreProductDto ToStoreDto(this Product product,
            IList<ProductAttributeCombination> combinations,
            IList<ProductManufacturer> productManufacturers,
            IList<ProductTag> tags)
        {
            if (product == null) return null!;

            var dto = new StoreProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Sku = product.Sku,
                Gtin = product.Gtin,
                ManufacturerPartNumber = product.ManufacturerPartNumber,
                ShortDescription = product.ShortDescription,
                FullDescription = product.FullDescription,
                Price = product.Price,
                OldPrice = product.OldPrice,
                CustomerEntersPrice = product.CustomerEntersPrice,
                DisableBuyButton = product.DisableBuyButton,
                IsShipEnabled = product.IsShipEnabled,
                IsFreeShipping = product.IsFreeShipping,
                IsTaxExempt = product.IsTaxExempt,
                StockQuantity = product.StockQuantity,
                MinStockQuantity = product.MinStockQuantity,
                BackorderModeId = product.BackorderModeId,
                OrderMinimumQuantity = product.OrderMinimumQuantity,
                OrderMaximumQuantity = product.OrderMaximumQuantity,
                ProductTypeId = product.ProductTypeId,
                ParentGroupedProductId = product.ParentGroupedProductId,
                VisibleIndividually = product.VisibleIndividually,
                MarkAsNew = product.MarkAsNew,
                AllowCustomerReviews = product.AllowCustomerReviews,
                DisplayOrder = product.DisplayOrder,
                Weight = product.Weight,
                Length = product.Length,
                Width = product.Width,
                Height = product.Height,
                AvailableStartDateTimeUtc = product.AvailableStartDateTimeUtc,
                AvailableEndDateTimeUtc = product.AvailableEndDateTimeUtc
            };

            if (combinations != null)
            {
                foreach (var combination in combinations)
                    dto.Combinations.Add(combination.ToDto());
            }

            if (productManufacturers != null)
            {
                foreach (var productManufacturer in productManufacturers)
                    dto.ManufacturerIds.Add(productManufacturer.ManufacturerId);
            }

            if (tags != null)
            {
                foreach (var tag in tags)
                    dto.Tags.Add(tag.Name);
            }

            return dto;
        }

        public static OrderDto ToDto(this Order order)
        {
            if (order == null) return null!;
            return new OrderDto
            {
                Id = order.Id,
                OrderTotal = order.OrderTotal,
                CreatedOnUtc = order.CreatedOnUtc,
                OrderStatus = order.OrderStatus.ToString(),
                PaymentStatus = order.PaymentStatus.ToString(),
                ShippingStatus = order.ShippingStatus.ToString(),
                CustomerId = order.CustomerId
            };
        }

        public static CustomerDto ToDto(this Customer customer)
        {
            if (customer == null) return null!;
            return new CustomerDto
            {
                Id = customer.Id,
                Email = customer.Email,
                Username = customer.Username,
                CreatedOnUtc = customer.CreatedOnUtc
            };
        }

        public static ProductDetailDto ToDto(this Product product)
        {
            if (product == null) return null!;
            return new ProductDetailDto
            {
                Id = product.Id,
                Name = product.Name,
                Sku = product.Sku,
                Gtin = product.Gtin,
                ManufacturerPartNumber = product.ManufacturerPartNumber,
                ShortDescription = product.ShortDescription,
                FullDescription = product.FullDescription,
                ProductTypeId = product.ProductTypeId,
                ProductTemplateId = product.ProductTemplateId,
                VendorId = product.VendorId,
                ParentGroupedProductId = product.ParentGroupedProductId,
                VisibleIndividually = product.VisibleIndividually,
                Published = product.Published,
                CallForPrice = product.CallForPrice,
                CustomerEntersPrice = product.CustomerEntersPrice,
                Price = product.Price,
                OldPrice = product.OldPrice,
                ProductCost = product.ProductCost,
                IsShipEnabled = product.IsShipEnabled,
                IsFreeShipping = product.IsFreeShipping,
                IsTaxExempt = product.IsTaxExempt,
                ManageInventoryMethodId = product.ManageInventoryMethodId,
                StockQuantity = product.StockQuantity,
                MinStockQuantity = product.MinStockQuantity,
                BackorderModeId = product.BackorderModeId,
                OrderMinimumQuantity = product.OrderMinimumQuantity,
                OrderMaximumQuantity = product.OrderMaximumQuantity,
                Weight = product.Weight,
                Length = product.Length,
                Width = product.Width,
                Height = product.Height,
                MarkAsNew = product.MarkAsNew,
                DisplayOrder = product.DisplayOrder,
                AllowCustomerReviews = product.AllowCustomerReviews,
                DisableBuyButton = product.DisableBuyButton,
                AvailableStartDateTimeUtc = product.AvailableStartDateTimeUtc,
                AvailableEndDateTimeUtc = product.AvailableEndDateTimeUtc,
                CreatedOnUtc = product.CreatedOnUtc,
                UpdatedOnUtc = product.UpdatedOnUtc
            };
        }

        public static ProductDto ToSummaryDto(this Product product)
        {
            if (product == null) return null!;
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Sku = product.Sku,
                Price = product.Price
            };
        }

        public static CategoryDto ToDto(this Category category)
        {
            if (category == null) return null!;
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ParentCategoryId = category.ParentCategoryId,
                Published = category.Published,
                ShowOnHomepage = category.ShowOnHomepage,
                DisplayOrder = category.DisplayOrder,
                PictureId = category.PictureId,
                CreatedOnUtc = category.CreatedOnUtc,
                UpdatedOnUtc = category.UpdatedOnUtc
            };
        }

        public static ManufacturerDto ToDto(this Manufacturer manufacturer)
        {
            if (manufacturer == null) return null!;
            return new ManufacturerDto
            {
                Id = manufacturer.Id,
                Name = manufacturer.Name,
                Description = manufacturer.Description,
                Published = manufacturer.Published,
                DisplayOrder = manufacturer.DisplayOrder,
                PictureId = manufacturer.PictureId,
                CreatedOnUtc = manufacturer.CreatedOnUtc,
                UpdatedOnUtc = manufacturer.UpdatedOnUtc
            };
        }

        public static AddressDto ToDto(this Address address)
        {
            if (address == null) return null!;
            return new AddressDto
            {
                Id = address.Id,
                FirstName = address.FirstName,
                LastName = address.LastName,
                Email = address.Email,
                Company = address.Company,
                CountryId = address.CountryId,
                StateProvinceId = address.StateProvinceId,
                County = address.County,
                City = address.City,
                Address1 = address.Address1,
                Address2 = address.Address2,
                ZipPostalCode = address.ZipPostalCode,
                PhoneNumber = address.PhoneNumber,
                FaxNumber = address.FaxNumber,
                CreatedOnUtc = address.CreatedOnUtc
            };
        }

        public static MetafieldDto ToDto(this GenericAttribute attribute)
        {
            if (attribute == null) return null!;
            return new MetafieldDto
            {
                Id = attribute.Id,
                EntityId = attribute.EntityId,
                KeyGroup = attribute.KeyGroup,
                Key = attribute.Key,
                Value = attribute.Value,
                StoreId = attribute.StoreId,
                CreatedOrUpdatedDateUTC = attribute.CreatedOrUpdatedDateUTC
            };
        }

        public static LogDto ToDto(this Log log)
        {
            if (log == null) return null!;
            return new LogDto
            {
                Id = log.Id,
                LogLevelId = log.LogLevelId,
                ShortMessage = log.ShortMessage,
                FullMessage = log.FullMessage,
                IpAddress = log.IpAddress,
                CustomerId = log.CustomerId,
                PageUrl = log.PageUrl,
                ReferrerUrl = log.ReferrerUrl,
                CreatedOnUtc = log.CreatedOnUtc
            };
        }

        public static ProductAttributeValueDto ToDto(this ProductAttributeValue value)
        {
            if (value == null) return null!;
            return new ProductAttributeValueDto
            {
                Id = value.Id,
                Name = value.Name,
                AttributeValueTypeId = value.AttributeValueTypeId,
                AssociatedProductId = value.AssociatedProductId,
                IsPreSelected = value.IsPreSelected,
                CustomerEntersQty = value.CustomerEntersQty,
                Quantity = value.Quantity,
                PriceAdjustment = value.PriceAdjustment,
                PriceAdjustmentUsePercentage = value.PriceAdjustmentUsePercentage,
                WeightAdjustment = value.WeightAdjustment,
                ColorSquaresRgb = value.ColorSquaresRgb,
                DisplayOrder = value.DisplayOrder
            };
        }

        public static ShipmentDto ToDto(this Shipment shipment, IList<ShipmentItem> items = null)
        {
            if (shipment == null) return null!;
            return new ShipmentDto
            {
                Id = shipment.Id,
                OrderId = shipment.OrderId,
                TrackingNumber = shipment.TrackingNumber,
                TotalWeight = shipment.TotalWeight,
                ShippedDateUtc = shipment.ShippedDateUtc,
                DeliveryDateUtc = shipment.DeliveryDateUtc,
                ReadyForPickupDateUtc = shipment.ReadyForPickupDateUtc,
                AdminComment = shipment.AdminComment,
                CreatedOnUtc = shipment.CreatedOnUtc,
                Items = items == null
                    ? new List<ShipmentItemDto>()
                    : new List<ShipmentItemDto>(items.Select(i => i.ToDto()))
            };
        }

        public static ShipmentItemDto ToDto(this ShipmentItem shipmentItem)
        {
            if (shipmentItem == null) return null!;
            return new ShipmentItemDto
            {
                Id = shipmentItem.Id,
                OrderItemId = shipmentItem.OrderItemId,
                Quantity = shipmentItem.Quantity,
                WarehouseId = shipmentItem.WarehouseId
            };
        }

        public static OrderItemDto ToDto(this OrderItem orderItem, Product product = null)
        {
            if (orderItem == null) return null!;
            return new OrderItemDto
            {
                Id = orderItem.Id,
                ProductId = orderItem.ProductId,
                Quantity = orderItem.Quantity,
                UnitPriceInclTax = orderItem.UnitPriceInclTax,
                PriceInclTax = orderItem.PriceInclTax,
                DiscountAmountInclTax = orderItem.DiscountAmountInclTax,
                OriginalProductCost = orderItem.OriginalProductCost,
                ProductName = product?.Name,
                ProductSku = product?.Sku,
                AttributeDescription = orderItem.AttributeDescription,
                AttributesXml = orderItem.AttributesXml
            };
        }

        /// <summary>
        /// Builds the order detail projection. Takes the line items and the addresses separately because
        /// they live on other records, and the product lookup separately so a detail response does not
        /// issue one query per line item.
        /// </summary>
        public static OrderDetailDto ToDetailDto(this Order order,
            IList<OrderItem> orderItems,
            Address billingAddress,
            Address shippingAddress,
            IDictionary<int, Product> productsById = null)
        {
            if (order == null) return null!;

            var detail = new OrderDetailDto
            {
                Id = order.Id,
                OrderTotal = order.OrderTotal,
                CreatedOnUtc = order.CreatedOnUtc,
                OrderStatus = order.OrderStatus.ToString(),
                PaymentStatus = order.PaymentStatus.ToString(),
                ShippingStatus = order.ShippingStatus.ToString(),
                CustomerId = order.CustomerId,
                CustomOrderNumber = order.CustomOrderNumber,
                OrderGuid = order.OrderGuid.ToString(),
                OrderSubtotalInclTax = order.OrderSubtotalInclTax,
                OrderShippingInclTax = order.OrderShippingInclTax,
                RefundedAmount = order.RefundedAmount,
                PaidDateUtc = order.PaidDateUtc,
                ShippingMethod = order.ShippingMethod,
                PaymentMethodSystemName = order.PaymentMethodSystemName,
                CheckoutAttributeDescription = order.CheckoutAttributeDescription,
                BillingAddress = billingAddress.ToDto(),
                ShippingAddress = (shippingAddress ?? billingAddress).ToDto()
            };

            if (orderItems != null)
            {
                foreach (var orderItem in orderItems)
                {
                    Product product = null;
                    if (productsById != null)
                        productsById.TryGetValue(orderItem.ProductId, out product);

                    detail.Items.Add(orderItem.ToDto(product));
                }
            }

            return detail;
        }
    }
}

