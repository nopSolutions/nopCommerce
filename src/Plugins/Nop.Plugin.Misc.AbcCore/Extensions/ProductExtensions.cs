using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Nop.Plugin.Misc.AbcCore.Mattresses;
using Nop.Plugin.Misc.AbcCore.Nop;

namespace Nop.Plugin.Misc.AbcCore.Extensions
{
    public static class ProductExtensions
    {
        public const string IsAddToCartKey = "IsAddToCart";
        public const string IsAddToCartWithUserInfoKey = "IsAddToCartWithUserInfo";

        public static async Task<bool> HasDeliveryOptionsAsync(this Product product)
        {
            var abcProductAttributeService = EngineContext.Current.Resolve<IAbcProductAttributeService>();
            return await abcProductAttributeService.ProductHasDeliveryOptionsAsync(product.Id);
        }

        public static bool IsMattress(this Product product)
        {
            var abcMattressModelService = EngineContext.Current.Resolve<IAbcMattressModelService>();
            return abcMattressModelService.IsProductMattress(product.Id);
        }

        public static async Task<bool> IsAddToCartToSeePriceAsync(this Product product)
        {
            var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
            return await genericAttributeService.GetAttributeAsync<bool>(product, IsAddToCartKey) ||
                   await genericAttributeService.GetAttributeAsync<bool>(product, IsAddToCartWithUserInfoKey);
        }

        public static async Task<bool> IsCallOnlyAsync(this Product product)
        {
            var manufacturerService = EngineContext.Current.Resolve<IManufacturerService>();
            var productManufacturers = await manufacturerService.GetProductManufacturersByProductIdAsync(product.Id);

            foreach (var pm in productManufacturers)
            {
                var manufacturer = await manufacturerService.GetManufacturerByIdAsync(pm.ManufacturerId);
                if (new string[]{"asko","subzero","wolf"}.Contains(manufacturer.Name.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }

        public static async Task<bool> IsAddToCartAsync(this Product product)
        {
            var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
            return await genericAttributeService.GetAttributeAsync<bool>(product, IsAddToCartKey);
        }

        public static async Task<bool> IsAddToCartWithUserInfoAsync(this Product product)
        {
            var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
            return await genericAttributeService.GetAttributeAsync<bool>(product, IsAddToCartWithUserInfoKey);
        }

        public static async Task<DateTime?> GetSpecialPriceEndDateAsync(this Product product)
        {
            var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
            return await genericAttributeService.GetAttributeAsync<DateTime?>(product, "SpecialPriceEndDate");
        }
    }
}