using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Discounts;
using Nop.Services.Localization;

namespace Nop.Plugin.DiscountRules.HasAllProducts
{
    public partial class HasAllProductsDiscountRequirementRule : BasePlugin, IDiscountRequirementRule
    {
        private readonly ISettingService _settingService;

        public HasAllProductsDiscountRequirementRule(ISettingService settingService)
        {
            this._settingService = settingService;
        }

        /// <summary>
        /// Check discount requirement
        /// </summary>
        /// <param name="request">Object that contains all information required to check the requirement (Current customer, discount, etc)</param>
        /// <returns>true - requirement is met; otherwise, false</returns>
        public bool CheckRequirement(CheckDiscountRequirementRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.DiscountRequirement == null)
                throw new NopException("Discount requirement is not set");

            var restrictedProductIds = _settingService.GetSettingByKey<string>(string.Format("DiscountRequirement.RestrictedProductIds-{0}", request.DiscountRequirement.Id));

            if (String.IsNullOrWhiteSpace(restrictedProductIds))
                return true;

            if (request.Customer == null)
                return false;

            //we support three ways of specifying products:
            //1. The comma-separated list of product identifiers (e.g. 77, 123, 156).
            //2. The comma-separated list of product identifiers with quantities.
            //      {Product ID}:{Quantity}. For example, 77:1, 123:2, 156:3
            //3. The comma-separated list of product identifiers with quantity range.
            //      {Product ID}:{Min quantity}-{Max quantity}. For example, 77:1-3, 123:2-5, 156:3-8
            var restrictedProducts = restrictedProductIds
                .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();
            if (restrictedProducts.Count == 0)
                return false;

            //group products in the cart by product ID
            //it could be the same product with distinct product attributes
            //that's why we get the total quantity of this product
            var cartQuery = from sci in request.Customer.ShoppingCartItems
                            where sci.ShoppingCartType == ShoppingCartType.ShoppingCart &&
                            sci.StoreId == request.Store.Id
                            group sci by sci.ProductId into g
                            select new { ProductId = g.Key, TotalQuantity = g.Sum(x => x.Quantity) };
            var cart = cartQuery.ToList();

            bool allFound = true;
            foreach (var restrictedProduct in restrictedProducts)
            {
                if (String.IsNullOrWhiteSpace(restrictedProduct))
                    continue;

                bool found1 = false;
                foreach (var sci in cart)
                {
                    if (restrictedProduct.Contains(":"))
                    {
                        if (restrictedProduct.Contains("-"))
                         {
                             //the third way (the quantity rage specified)
                             //{Product ID}:{Min quantity}-{Max quantity}. For example, 77:1-3, 123:2-5, 156:3-8
                             int restrictedProductId = 0;
                             if (!int.TryParse(restrictedProduct.Split(new[] { ':' })[0], out restrictedProductId))
                                 //parsing error; exit;
                                 return false;
                             int quantityMin = 0;
                             if (!int.TryParse(restrictedProduct.Split(new[] { ':' })[1].Split(new[] { '-' })[0], out quantityMin))
                                 //parsing error; exit;
                                 return false;
                             int quantityMax = 0;
                             if (!int.TryParse(restrictedProduct.Split(new[] { ':' })[1].Split(new[] { '-' })[1], out quantityMax))
                                 //parsing error; exit;
                                 return false;

                             if (sci.ProductId == restrictedProductId && quantityMin <= sci.TotalQuantity && sci.TotalQuantity <= quantityMax)
                             {
                                 found1 = true;
                                 break;
                             }
                         }
                         else
                         {
                             //the second way (the quantity specified)
                             //{Product ID}:{Quantity}. For example, 77:1, 123:2, 156:3
                             int restrictedProductId = 0;
                             if (!int.TryParse(restrictedProduct.Split(new[] { ':' })[0], out restrictedProductId))
                                 //parsing error; exit;
                                 return false;
                             int quantity = 0;
                             if (!int.TryParse(restrictedProduct.Split(new[] { ':' })[1], out quantity))
                                 //parsing error; exit;
                                 return false;

                             if (sci.ProductId == restrictedProductId && sci.TotalQuantity == quantity)
                             {
                                 found1 = true;
                                 break;
                             }
                         }
                    }
                    else
                    {
                        //the first way (the quantity is not specified)
                        int restrictedProductId = int.Parse(restrictedProduct);
                        if (sci.ProductId == restrictedProductId)
                        {
                            found1 = true;
                            break;
                        }
                    }
                }

                if (!found1)
                {
                    allFound = false;
                    break;
                }
            }

            if (allFound)
                return true;

            return false;
        }

        /// <summary>
        /// Get URL for rule configuration
        /// </summary>
        /// <param name="discountId">Discount identifier</param>
        /// <param name="discountRequirementId">Discount requirement identifier (if editing)</param>
        /// <returns>URL</returns>
        public string GetConfigurationUrl(int discountId, int? discountRequirementId)
        {
            //configured in RouteProvider.cs
            string result = "Plugins/DiscountRulesHasAllProducts/Configure/?discountId=" + discountId;
            if (discountRequirementId.HasValue)
                result += string.Format("&discountRequirementId={0}", discountRequirementId.Value);
            return result;
        }

        public override void Install()
        {
            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.HasAllProducts.Fields.Products", "Restricted products");
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.HasAllProducts.Fields.Products.Hint", "The comma-separated list of product identifiers (e.g. 77, 123, 156). You can find a product ID on its details page. You can also specify the comma-separated list of product identifiers with quantities ({Product ID}:{Quantity}. for example, 77:1, 123:2, 156:3). And you can also specify the comma-separated list of product identifiers with quantity range ({Product ID}:{Min quantity}-{Max quantity}. for example, 77:1-3, 123:2-5, 156:3-8).");
            base.Install();
        }

        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Plugins.DiscountRules.HasAllProducts.Fields.Products");
            this.DeletePluginLocaleResource("Plugins.DiscountRules.HasAllProducts.Fields.Products.Hint");
            base.Uninstall();
        }
    }
}