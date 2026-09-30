using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.Rest.Models;

namespace Nop.Plugin.Api.Rest.Mappings
{
    public static class DtoMappings
    {
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

        public static CustomerProductDto ToDto(this ShoppingCartItem shoppingCartItem, Product product = null)
        {
            if (shoppingCartItem == null) return null!;
            return new CustomerProductDto
            {
                Id = shoppingCartItem.Id,
                CustomerId = shoppingCartItem.CustomerId,
                ProductId = shoppingCartItem.ProductId,
                ProductName = product?.Name,
                ProductSku = product?.Sku,
                Quantity = shoppingCartItem.Quantity,
                WishlistId = shoppingCartItem.CustomWishlistId,
                StoreId = shoppingCartItem.StoreId,
                AttributesXml = shoppingCartItem.AttributesXml,
                CustomerEnteredPrice = shoppingCartItem.CustomerEnteredPrice,
                RentalStartDateUtc = shoppingCartItem.RentalStartDateUtc,
                RentalEndDateUtc = shoppingCartItem.RentalEndDateUtc,
                CreatedOnUtc = shoppingCartItem.CreatedOnUtc,
                UpdatedOnUtc = shoppingCartItem.UpdatedOnUtc
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
    }
}
