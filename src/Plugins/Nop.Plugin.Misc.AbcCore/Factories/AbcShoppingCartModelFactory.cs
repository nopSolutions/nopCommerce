using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.ShoppingCart;
using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Http.Extensions;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Common;
using Nop.Web.Models.Media;
using Nop.Services.Logging;

namespace Nop.Plugin.Misc.AbcCore.Factories
{
    public class AbcShoppingCartModelFactory : ShoppingCartModelFactory, IShoppingCartModelFactory
    {
        private readonly IPriceFormatter _priceFormatter;
        
        public AbcShoppingCartModelFactory(
            AddressSettings addressSettings,
            CaptchaSettings captchaSettings,
            CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            CustomerSettings customerSettings,
            IAddressModelFactory addressModelFactory,
            ICheckoutAttributeFormatter checkoutAttributeFormatter,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeService checkoutAttributeService,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IDiscountService discountService,
            IDownloadService downloadService,
            IGenericAttributeService genericAttributeService,
            IGiftCardService giftCardService,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IOrderProcessingService orderProcessingService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IProductAttributeFormatter productAttributeFormatter,
            IProductService productService,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            ITaxService taxService,
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            OrderSettings orderSettings,
            RewardPointsSettings rewardPointsSettings,
            ShippingSettings shippingSettings,
            ShoppingCartSettings shoppingCartSettings,
            TaxSettings taxSettings,
            VendorSettings vendorSettings,
            ILogger logger) :
            base(
                addressSettings,
                captchaSettings,
                catalogSettings,
                commonSettings,
                customerSettings,
                addressModelFactory,
                checkoutAttributeFormatter,
                checkoutAttributeParser,
                checkoutAttributeService,
                countryService,
                currencyService,
                customerService,
                dateTimeHelper,
                discountService,
                downloadService,
                genericAttributeService,
                giftCardService,
                httpContextAccessor,
                localizationService,
                orderProcessingService,
                orderTotalCalculationService,
                paymentPluginManager,
                paymentService,
                permissionService,
                pictureService,
                priceFormatter,
                productAttributeFormatter,
                productService,
                shippingService,
                shoppingCartService,
                stateProvinceService,
                staticCacheManager,
                storeContext,
                storeMappingService,
                taxService,
                urlRecordService,
                vendorService,
                webHelper,
                workContext,
                mediaSettings,
                orderSettings,
                rewardPointsSettings,
                shippingSettings,
                shoppingCartSettings,
                taxSettings,
                vendorSettings,
                logger)
        {
            _priceFormatter = priceFormatter;
        }
        
        protected override async Task<ShoppingCartModel.ShoppingCartItemModel> PrepareShoppingCartItemModelAsync(
            IList<ShoppingCartItem> cart, ShoppingCartItem sci)
        {
            var model = await base.PrepareShoppingCartItemModelAsync(cart, sci);

            // ABCTODO: consider adding the mattress/delivery option functionality
            // here as well

            if (model.Discount == null) { return model; }
            var discountValue = Decimal.Parse(
                model.Discount,
                NumberStyles.AllowCurrencySymbol |
                NumberStyles.AllowDecimalPoint |
                NumberStyles.AllowThousands, new CultureInfo("en-US")) / model.Quantity;
            var unitPriceValue = Decimal.Parse(
                model.UnitPrice,
                NumberStyles.AllowCurrencySymbol |
                NumberStyles.AllowDecimalPoint |
                NumberStyles.AllowThousands, new CultureInfo("en-US"));

            model.UnitPrice = await _priceFormatter.FormatPriceAsync(unitPriceValue + discountValue);

            return model;
        }
    }
}
