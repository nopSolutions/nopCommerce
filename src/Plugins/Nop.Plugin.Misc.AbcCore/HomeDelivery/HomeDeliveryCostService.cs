using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Misc.AbcCore.Extensions;
using Nop.Services.Catalog;
using Nop.Services.Shipping;

namespace Nop.Plugin.Misc.AbcCore.HomeDelivery
{
    public class HomeDeliveryCostService : IHomeDeliveryCostService
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeFormatter _productAttributeFormatter;

        public HomeDeliveryCostService(
            ICategoryService categoryService,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IProductAttributeParser productAttributeParser,
            IProductAttributeFormatter productAttributeFormatter
        )
        {
            _categoryService = categoryService;
            _productAttributeService = productAttributeService;
            _productService = productService;
            _productAttributeParser = productAttributeParser;
            _productAttributeFormatter = productAttributeFormatter;
        }

        public async Task<decimal> GetHomeDeliveryCostAsync(IList<OrderItem> orderItems)
        {
            var result = 0M;
            // skip any mattresses after finding one
            var skipMattress = false;

            foreach (var orderItem in orderItems.OrderByDescending(oi => oi.PriceExclTax))
            {
                var isMattress = orderItem.GetMattressSize() != null;
                if (isMattress && skipMattress) { continue; }

                result += await GetHomeDeliveryCostOfItem(orderItem);

                if (isMattress) { skipMattress = true; }
            }

            return result;
        }

        public async Task<decimal> GetHomeDeliveryCostAsync(IList<GetShippingOptionRequest.PackageItem> packageItems)
        {
            var result = 0M;
            // skip any mattresses after finding one
            var skipMattress = false;

            // sort the package items
            packageItems = await packageItems.OrderByDescendingAwait(
                async pi => await GetMattressItemCostAsync(pi.Product.Id, pi.ShoppingCartItem.AttributesXml)
            ).ToListAsync();

            foreach (var packageItem in packageItems)
            {
                var productAttributes = await _productAttributeFormatter.FormatAttributesAsync(
                    packageItem.Product, packageItem.ShoppingCartItem.AttributesXml
                );
                var isMattress =
                    productAttributes.Contains("Mattress Size:");
                if (isMattress && skipMattress) { continue; }

                result += await GetHomeDeliveryCostOfItem(packageItem);

                if (isMattress) { skipMattress = true; }
            }

            return result;
        }

        private async Task<decimal> GetHomeDeliveryCostOfItem(OrderItem orderItem)
        {
            return await GetCostAsync(orderItem.ProductId, orderItem.AttributesXml, orderItem.Quantity);
        }

        private async Task<decimal> GetHomeDeliveryCostOfItem(GetShippingOptionRequest.PackageItem packageItem)
        {
            return await GetCostAsync(packageItem.Product.Id, packageItem.ShoppingCartItem.AttributesXml, packageItem.ShoppingCartItem.Quantity);
        }

        private async Task<decimal> GetCostAsync(int productId, string attributesXml, int quantity)
        {
            if (!attributesXml.Contains("delivered to you")) { return 0; }

            var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(productId);
            List<Category> categories = new List<Category>();
            foreach (var pc in productCategories)
            {
                categories.Add(await _categoryService.GetCategoryByIdAsync(pc.CategoryId));
            }

            if (categories.Any(c => new string[]
                { "recliners", "lift chairs", "massage chairs" }.Contains(c.Name.ToLower()))
            )
            {
                return 49.00M * quantity;
            }

            // Since that Package Item doesn't carry the actual item 
            if (categories.Any(c => new string[]
                { "twin", "twin extra long", "full", "queen", "king", "california king" }
                .Contains(c.Name.ToLower()))
            )
            {
                // Mattresses do not include quantity
                return await GetMattressHomeDeliveryCostAsync(attributesXml, productId);
            }

            // default
            return 14.75M * quantity;
        }

        private async Task<decimal> GetMattressHomeDeliveryCostAsync(string attributesXml, int productId)
        {
            return await GetMattressItemCostAsync(productId, attributesXml) >= 697.00M ?
                0 :
                99M;
        }

        // If non-mattress passed in, you'll get the normal price
        private async Task<decimal> GetMattressItemCostAsync(int productId, string attributesXml)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            var productAttributes = await _productAttributeService.GetAllProductAttributesAsync();
            var mattressProductAttributeIds =
                productAttributes
                .Where(pa => pa.Name.Contains("Mattress Size") || pa.Name.Contains("Base ("))
                .Select(pa => pa.Id);

            var pams = await _productAttributeParser.ParseProductAttributeMappingsAsync(attributesXml);

            return product.Price +
                pams
                .Where(pam => mattressProductAttributeIds.Contains(pam.ProductAttributeId))
                .Select(pam => _productAttributeParser.ParseValues(attributesXml, pam.Id).FirstOrDefault())
                .Select(async idasstring => await _productAttributeService.GetProductAttributeValueByIdAsync(int.Parse(idasstring)))
                .Select(t => t.Result)
                .Sum(pav => pav.PriceAdjustment);
        }
    }
}
