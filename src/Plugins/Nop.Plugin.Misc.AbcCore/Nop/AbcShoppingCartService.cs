using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Discounts;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Misc.AbcCore.Services;
using Nop.Plugin.Misc.AbcCore.Domain;
using Nop.Services.Directory;
using Nop.Services.Customers;
using Nop.Services.Caching;
using Nop.Services.Orders;
using Nop.Core.Domain.Discounts;
using Nop.Services.Security;
using Nop.Services.Shipping.Date;
using Nop.Services.Common;
using Nop.Services.Stores;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Nop.Services.Helpers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using System.Threading.Tasks;
using SevenSpikes.Nop.Plugins.StoreLocator.Domain.Shops;
using SevenSpikes.Nop.Plugins.StoreLocator.Services;
using Nop.Plugin.Misc.AbcCore.Models;
using Nop.Plugin.Misc.AbcCore.Nop;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Plugin.Misc.AbcCore.Nop
{
    //a custom price calculation service to modify cart price as needed
    class AbcShoppingCartService : ShoppingCartService, IAbcShoppingCartService
    {
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IAttributeUtilities _attributeUtilities;
        private readonly IRepository<HiddenAttributeValue> _hiddenAttributeValueRepository;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IPriceCalculationService _priceCalculationService;

        private readonly ICustomerShopService _customerShopService;
        private readonly IBackendStockService _backendStockService;
        private readonly IShopService _shopService;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;

        private readonly IAbcCategoryService _abcCategoryService;
        private readonly ILocalizationService _localizationService;

        public AbcShoppingCartService(
            CatalogSettings catalogSettings,
            IAclService aclService,
            IActionContextAccessor actionContextAccessor,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeService checkoutAttributeService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IDateRangeService dateRangeService,
            IDateTimeHelper dateTimeHelper,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IRepository<ShoppingCartItem> sciRepository,
            IShippingService shippingService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IUrlHelperFactory urlHelperFactory,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            OrderSettings orderSettings,
            ShoppingCartSettings shoppingCartSettings,
            // custom
            IAttributeUtilities attributeUtilities,
            IRepository<HiddenAttributeValue> hiddenAttributeValueRepository,
            ICustomerShopService customerShopService,
            IBackendStockService backendStockService,
            IShopService shopService,
            IAbcCategoryService abcCategoryService
        )
            : base(catalogSettings, aclService, actionContextAccessor,
                checkoutAttributeParser, checkoutAttributeService, currencyService,
                customerService, dateRangeService, dateTimeHelper,
                genericAttributeService, localizationService, permissionService,
                priceCalculationService, priceFormatter, productAttributeParser,
                productAttributeService, productService, sciRepository,
                shippingService, staticCacheManager, storeContext, storeMappingService,
                urlHelperFactory, urlRecordService, workContext, orderSettings,
                shoppingCartSettings)
        {
            _hiddenAttributeValueRepository =
                    EngineContext.Current.Resolve<IRepository<HiddenAttributeValue>>();

            _productAttributeParser = productAttributeParser;
            _attributeUtilities = attributeUtilities;
            _shoppingCartSettings = shoppingCartSettings;
            _priceCalculationService = priceCalculationService;

            _customerShopService = customerShopService;
            _attributeUtilities = attributeUtilities;
            _backendStockService = backendStockService;
            _shopService = shopService;
            _productAttributeParser = productAttributeParser;
            _productService = productService;
            _workContext = workContext;

            _abcCategoryService = abcCategoryService;
            _localizationService = localizationService;
        }

        public async Task<bool> IsCartEligibleForCheckoutAsync(object model)
        {
            dynamic items = null;
            if (model is ShoppingCartModel)
            {
                items = (model as ShoppingCartModel).Items;
            }
            else if (model is MiniShoppingCartModel)
            {
                items = (model as MiniShoppingCartModel).Items;
            }
            else
            {
                throw new NopException("ShoppingCartModel or MiniShoppingCartModel not passed in.");
            }
                
            foreach (var item in items)
            {
                var pcs = await _abcCategoryService.GetProductCategoriesByProductIdAsync(item.ProductId);
                foreach (var pc in pcs)
                {
                    if (await _abcCategoryService.IsCategoryIdClearance(pc.CategoryId))
                    {
                        return false;
                    }

                    var product = await _productService.GetProductByIdAsync(pc.ProductId);
                    if (product.DisableBuyButton || !product.Published)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override async Task MigrateShoppingCartAsync(Customer fromCustomer, Customer toCustomer, bool includeCouponCodes)
        {
            await base.MigrateShoppingCartAsync(fromCustomer, toCustomer, includeCouponCodes);

            if (fromCustomer.Id == toCustomer.Id)
                return;

            var fromCsm = _customerShopService.GetCurrentCustomerShopMapping(fromCustomer.Id);

            if (fromCsm == null)
            {
                //old customer has no pickup items to update, do nothing
                return;
            }

            //update tocustomer's shop mapping
            _customerShopService.InsertOrUpdateCustomerShop(toCustomer.Id, fromCsm.ShopId);
            var csm = _customerShopService.GetCurrentCustomerShopMapping(toCustomer.Id);
            Shop shop = await _shopService.GetShopByIdAsync(csm.ShopId);

            //used to merge together products that will now have the same attributes
            Dictionary<int, ShoppingCartItem> productIdToPickupSci = new Dictionary<int, ShoppingCartItem>();
            Dictionary<int, ShoppingCartItem> productIdToDeliverySci = new Dictionary<int, ShoppingCartItem>();

            List<ShoppingCartItem> toDelete = new List<ShoppingCartItem>();

            //update all pickup items in the cart to current availability status
            foreach (ShoppingCartItem sci in (await GetShoppingCartAsync(toCustomer)).Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart))
            {

                ProductAttributeMapping pickupAttribute = await _attributeUtilities.GetPickupAttributeMappingAsync(sci.AttributesXml);

                if (pickupAttribute != null)
                {
                    //if we already have an existing shoppingcart item for this product update its quantity
                    if (productIdToPickupSci.ContainsKey(sci.ProductId))
                    {
                        var existingSci = productIdToPickupSci[sci.ProductId];
                        await base.UpdateShoppingCartItemAsync(toCustomer, existingSci.Id,
                            existingSci.AttributesXml, existingSci.CustomerEnteredPrice, null, null, existingSci.Quantity + sci.Quantity, false);
                        toDelete.Add(sci);
                    }
                    else
                    {
                        // check if product is available at the selected store
                        StockResponse stockResponse = await _backendStockService.GetApiStockAsync(sci.ProductId);
                        bool available = false;
                        if (stockResponse != null)
                        {
                            available = stockResponse.ProductStocks.Where(ps => ps.Available && ps.Shop.Id == csm.ShopId).Any();
                        }

                        //if available clean and re add the pickup attribute
                        if (available)
                        {
                            string removedAttr = _productAttributeParser.RemoveProductAttribute(sci.AttributesXml, pickupAttribute);

                            sci.AttributesXml = await _attributeUtilities.InsertPickupAttributeAsync(await _productService.GetProductByIdAsync(sci.ProductId), stockResponse, removedAttr, shop);
                            productIdToPickupSci[sci.ProductId] = sci;
                        }
                        else
                        {
                            //else we switch it to home delivery
                            //merge home delivery if it exists
                            if (productIdToDeliverySci.ContainsKey(sci.ProductId))
                            {
                                var existingSci = productIdToDeliverySci[sci.ProductId];
                                await base.UpdateShoppingCartItemAsync(toCustomer, existingSci.Id,
                                    existingSci.AttributesXml, existingSci.CustomerEnteredPrice, null, null, existingSci.Quantity + sci.Quantity, false);
                                toDelete.Add(sci);
                                continue;
                            }
                            else
                            {
                                //else replace the pickup attribute with home delivery
                                sci.AttributesXml = await _attributeUtilities.InsertHomeDeliveryAttributeAsync(
                                    await _productService.GetProductByIdAsync(sci.ProductId), sci.AttributesXml
                                );
                                productIdToDeliverySci[sci.ProductId] = sci;
                            }


                        }
                        //update the sci with new attributes
                        await base.UpdateShoppingCartItemAsync(toCustomer, sci.Id,
                                sci.AttributesXml, sci.CustomerEnteredPrice, null, null, sci.Quantity, false);
                    }

                }
                else
                {
                    //if not a pickup item, keep track for later merging
                    productIdToDeliverySci[sci.ProductId] = sci;
                }
            }

            for (int i = 0; i < toDelete.Count; ++i)
            {
                await base.DeleteShoppingCartItemAsync(toDelete[i]);
            }
        }

        public async override Task<(decimal unitPrice, decimal discountAmount, List<Discount> appliedDiscounts)> GetUnitPriceAsync(
            Product product,
            Customer customer,
            ShoppingCartType shoppingCartType,
            int quantity,
            string attributesXml,
            decimal customerEnteredPrice,
            DateTime? rentalStartDate, DateTime? rentalEndDate,
            bool includeDiscounts)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var discountAmount = decimal.Zero;
            var appliedDiscounts = new List<Discount>();

            decimal finalPrice;

            var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);
            if (combination?.OverriddenPrice.HasValue ?? false)
            {
                (_, finalPrice, discountAmount, appliedDiscounts) =  await _priceCalculationService.GetFinalPriceAsync(product,
                        customer,
                        combination.OverriddenPrice.Value,
                        decimal.Zero,
                        includeDiscounts,
                        quantity,
                        product.IsRental ? rentalStartDate : null,
                        product.IsRental ? rentalEndDate : null);
            }
            else
            {
                // custom
                // summarize price of all attributes except warranties (warranties processed separately)
                decimal warrantyPrice = decimal.Zero;
                ProductAttributeMapping warrantyPam = await _attributeUtilities.GetWarrantyAttributeMappingAsync(attributesXml);

                //summarize price of all attributes
                var attributesTotalPrice = decimal.Zero;
                var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(attributesXml);

                // custom - considers warranties
                if (attributeValues != null)
                {
                    foreach (var attributeValue in attributeValues)
                    {
                        if (warrantyPam != null && attributeValue.ProductAttributeMappingId == warrantyPam.Id)
                        {
                            warrantyPrice =
                                await _priceCalculationService.GetProductAttributeValuePriceAdjustmentAsync(
                                    product,
                                    attributeValue,
                                    customer,
                                    product.CustomerEntersPrice ? (decimal?)customerEnteredPrice : null
                                );
                        }
                        else
                        {
                            attributesTotalPrice += await _priceCalculationService.GetProductAttributeValuePriceAdjustmentAsync(
                                product,
                                attributeValue,
                                customer,
                                product.CustomerEntersPrice ? (decimal?)customerEnteredPrice : null
                            );
                        }
                    }
                }

                //get price of a product (with previously calculated price of all attributes)
                if (product.CustomerEntersPrice)
                {
                    finalPrice = customerEnteredPrice;
                }
                else
                {
                    int qty;
                    if (_shoppingCartSettings.GroupTierPricesForDistinctShoppingCartItems)
                    {
                        //the same products with distinct product attributes could be stored as distinct "ShoppingCartItem" records
                        //so let's find how many of the current products are in the cart                        
                        qty = (await GetShoppingCartAsync(customer, shoppingCartType: shoppingCartType, productId: product.Id))
                            .Sum(x => x.Quantity);

                        if (qty == 0)
                        {
                            qty = quantity;
                        }
                    }
                    else
                    {
                        qty = quantity;
                    }

                    (_, finalPrice, discountAmount, appliedDiscounts) = await _priceCalculationService.GetFinalPriceAsync(product,
                        customer,
                        attributesTotalPrice,
                        includeDiscounts,
                        qty,
                        product.IsRental ? rentalStartDate : null,
                        product.IsRental ? rentalEndDate : null);
                    finalPrice += warrantyPrice;
                }
            }

            //rounding
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                finalPrice = await _priceCalculationService.RoundPriceAsync(finalPrice);

            return (finalPrice, discountAmount, appliedDiscounts);
        }

        public async override Task<(decimal unitPrice, decimal discountAmount, List<Discount> appliedDiscounts)> GetUnitPriceAsync(
            ShoppingCartItem shoppingCartItem,
            bool includeDiscounts)
        {
            var appliedDiscounts = new List<Discount>();
            var baseResult = await base.GetUnitPriceAsync(
                shoppingCartItem,
                includeDiscounts
            );

            if (_hiddenAttributeValueRepository == null)
            {
                return baseResult;
            }

            var hiddenAttrVals = _hiddenAttributeValueRepository.Table.Where(hav => hav.ShoppingCartItem_Id == shoppingCartItem.Id);

            foreach (var hav in hiddenAttrVals)
            {
                baseResult.unitPrice += hav.PriceAdjustment;
            }

            return baseResult;
        }


        protected override async Task<IList<string>> GetStandardWarningsAsync(Customer customer, ShoppingCartType shoppingCartType, Product product,
            string attributesXml, decimal customerEnteredPrice, int quantity, int shoppingCartItemId, int storeId)
        {
            var warnings = await base.GetStandardWarningsAsync(customer, shoppingCartType, product, attributesXml, customerEnteredPrice, quantity, shoppingCartItemId, storeId);
            var needsReplacement = false;
            var toReplace = await _localizationService.GetResourceAsync("ShoppingCart.BuyingDisabled");

            foreach (var warning in warnings)
            {
                if (warning.Equals(toReplace))
                {
                    var productCategories = await _abcCategoryService.GetProductCategoriesByProductIdAsync(product.Id);
                    foreach (var pc in productCategories)
                    {
                        if (await _abcCategoryService.IsCategoryIdClearance(pc.CategoryId))
                        {
                            needsReplacement = true;
                            break;
                        }
                    }
                }
            }

            if (needsReplacement)
            {
                warnings.Remove(toReplace);
                warnings.Add("Clearance Item, In-Store Only");
            }

            return warnings;
        }
        // TODO: Tabling this for now
        // private async Task<(decimal unitPrice, decimal discountAmount, List<Discount> appliedDiscounts)> ProcessCustomDiscountAsync(
        //     decimal unitPrice,
        //     decimal discountAmount,
        //     List<Discount> appliedDiscounts
        // )
        // {
        //     if (discountAmount == 0 || !appliedDiscounts.Any()) return (unitPrice, discountAmount, appliedDiscounts);

        //     var buyOneGetDiscountGrouped = appliedDiscounts.FirstOrDefault(d => d.Name == "BuyOneGetDiscountGrouped");
        //     if (buyOneGetDiscountGrouped == null) return (unitPrice, discountAmount, appliedDiscounts);
            
        //     var groupedProductIds = (await _productService.GetProductsWithAppliedDiscountAsync(buyOneGetDiscountGrouped.Id)).Select(p => p.Id);
        //     var customer = await _workContext.GetCurrentCustomerAsync();
        //     var cart = await GetShoppingCartAsync(customer);
        //     var cartProductIds = cart.Select(sci => sci.ProductId);
        //     var commonProductIds = groupedProductIds.Intersect(cartProductIds);

        //     // If this is not the second item (via quantity or multi product), remove discount
        //     if (commonProductIds.Count() == 1)
        //     {
        //         var sci = cart.FirstOrDefault(sci => sci.ProductId == commonProductIds.First());
        //         if (sci.Quantity == 1)
        //         {
        //             unitPrice += discountAmount;
        //             discountAmount = 0;
        //             appliedDiscounts.Remove(buyOneGetDiscountGrouped);
        //         }
        //     }

        //     // TODO: With 2 or more cart items, need to determine lowest price item
        //     if (commonProductIds.Count() > 1)
        //     {
        //         var eligibleScis = cart.Where(sci => commonProductIds.Contains(sci.ProductId));
        //         foreach (var sci in eligibleScis)
        //         {
        //             var a = 1;
        //         }
            
        //     // TODO: If matching prices, mark sci as discounted
        //     }

        //     return (unitPrice, discountAmount, appliedDiscounts);
        // }
    }
}
