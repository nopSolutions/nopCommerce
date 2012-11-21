using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Discounts;
using Nop.Services.Localization;

namespace Nop.Plugin.DiscountRules.HasOneProduct
{
    public partial class HasOneProductDiscountRequirementRule : BasePlugin, IDiscountRequirementRule
    {
        private readonly ISettingService _settingService;

        public HasOneProductDiscountRequirementRule(ISettingService settingService)
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

            var restrictedProductVariantIds = _settingService.GetSettingByKey<string>(string.Format("DiscountRequirement.RestrictedProductVariantIds-{0}", request.DiscountRequirement.Id));

            if (String.IsNullOrWhiteSpace(restrictedProductVariantIds))
                return true;

            if (request.Customer == null)
                return false;

            //we support three ways of specifying product variants:
            //1. The comma-separated list of product variant identifiers (e.g. 77, 123, 156).
            //2. The comma-separated list of product variant identifiers with quantities.
            //      {Product variant ID}:{Quantity}. For example, 77:1, 123:2, 156:3
            //3. The comma-separated list of product variant identifiers with quantity range.
            //      {Product variant ID}:{Min quantity}-{Max quantity}. For example, 77:1-3, 123:2-5, 156:3-8
            var restrictedProductVariants = restrictedProductVariantIds
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();
            if (restrictedProductVariants.Count == 0)
                return false;
            
            //group products in the cart by product variant ID
            //it could be the same product variant with distinct product attributes
            //that's why we get the total quantity of this product variant
            var cartQuery = from sci in request.Customer.ShoppingCartItems
                                   where sci.ShoppingCartType == ShoppingCartType.ShoppingCart
                                   group sci by sci.ProductVariantId into g
                                   select new {ProductVariantId = g.Key, TotalQuantity = g.Sum(x => x.Quantity)};
            var cart = cartQuery.ToList();

            //process
            bool found = false;
            foreach (var restrictedPv in restrictedProductVariants)
            {
                if (String.IsNullOrWhiteSpace(restrictedPv))
                    continue;

                foreach (var sci in cart)
                {
                    if (restrictedPv.Contains(":"))
                    {
                        if (restrictedPv.Contains("-"))
                        {
                            //the third way (the quantity rage specified)
                            //{Product variant ID}:{Min quantity}-{Max quantity}. For example, 77:1-3, 123:2-5, 156:3-8
                            int restrictedPvId = 0;
                            if (!int.TryParse(restrictedPv.Split(new[] { ':' })[0], out restrictedPvId))
                                //parsing error; exit;
                                return false;
                            int quantityMin = 0;
                            if (!int.TryParse(restrictedPv.Split(new[] { ':' })[1].Split(new[] { '-' })[0], out quantityMin))
                                //parsing error; exit;
                                return false;
                            int quantityMax = 0;
                            if (!int.TryParse(restrictedPv.Split(new[] { ':' })[1].Split(new[] { '-' })[1], out quantityMax))
                                //parsing error; exit;
                                return false;

                            if (sci.ProductVariantId == restrictedPvId && quantityMin <= sci.TotalQuantity && sci.TotalQuantity <= quantityMax)
                            {
                                found = true;
                                break;
                            }
                        }
                        else
                        {
                            //the second way (the quantity specified)
                            //{Product variant ID}:{Quantity}. For example, 77:1, 123:2, 156:3
                            int restrictedPvId = 0;
                            if (!int.TryParse(restrictedPv.Split(new[] { ':' })[0], out restrictedPvId))
                                //parsing error; exit;
                                return false;
                            int quantity = 0;
                            if (!int.TryParse(restrictedPv.Split(new[] { ':' })[1], out quantity))
                                //parsing error; exit;
                                return false;

                            if (sci.ProductVariantId == restrictedPvId && sci.TotalQuantity == quantity)
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        //the first way (the quantity is not specified)
                        int restrictedPvId = int.Parse(restrictedPv);
                        if (sci.ProductVariantId == restrictedPvId)
                        {
                            found = true;
                            break;
                        }
                    }
                }

                if (found)
                {
                    break;
                }
            }

            if (found)
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
            string result = "Plugins/DiscountRulesHasOneProduct/Configure/?discountId=" + discountId;
            if (discountRequirementId.HasValue)
                result += string.Format("&discountRequirementId={0}", discountRequirementId.Value);
            return result;
        }

        public override void Install()
        {
            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.HasOneProduct.Fields.ProductVariants", "Restricted product variants");
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.HasOneProduct.Fields.ProductVariants.Hint", "The comma-separated list of product variant identifiers (e.g. 77, 123, 156). You can find a product variant ID on its details page. You can also specify the comma-separated list of product variant identifiers with quantities ({Product variant ID}:{Quantity}. for example, 77:1, 123:2, 156:3). And you can also specify the comma-separated list of product variant identifiers with quantity range ({Product variant ID}:{Min quantity}-{Max quantity}. for example, 77:1-3, 123:2-5, 156:3-8).");
            base.Install();
        }

        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Plugins.DiscountRules.HasOneProduct.Fields.ProductVariants");
            this.DeletePluginLocaleResource("Plugins.DiscountRules.HasOneProduct.Fields.ProductVariants.Hint");
            base.Uninstall();
        }
    }
}