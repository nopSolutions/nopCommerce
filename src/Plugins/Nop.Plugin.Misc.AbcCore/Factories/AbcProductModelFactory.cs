using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Vendors;
using Nop.Services.Caching;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping.Date;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Nop.Plugin.Misc.AbcCore.Mattresses;
using Nop.Plugin.Misc.AbcCore.Services;
using Nop.Plugin.Misc.AbcCore.Delivery;

namespace Nop.Plugin.Misc.AbcCore.Factories
{
    public partial class AbcProductModelFactory : ProductModelFactory, IAbcProductModelFactory
    {
        private readonly IWebHelper _webHelper;
        private readonly IAbcMattressListingPriceService _abcMattressListingPriceService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAbcDescriptionService _productAbcDescriptionService;
        private readonly IProductAttributeParser _productAttributeParser;

        public AbcProductModelFactory(
            CaptchaSettings captchaSettings,
            CatalogSettings catalogSettings,
            CustomerSettings customerSettings,
            ICategoryService categoryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IDateRangeService dateRangeService,
            IDateTimeHelper dateTimeHelper,
            IDownloadService downloadService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IProductTagService productTagService,
            IProductTemplateService productTemplateService,
            IReviewTypeService reviewTypeService,
            ISpecificationAttributeService specificationAttributeService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IShoppingCartModelFactory shoppingCartModelFactory,
            ITaxService taxService,
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            OrderSettings orderSettings,
            SeoSettings seoSettings,
            ShippingSettings shippingSettings,
            VendorSettings vendorSettings,
            IAbcMattressListingPriceService abcMattressListingPriceService,
            IProductAbcDescriptionService productAbcDescriptionService)
            : base(captchaSettings, catalogSettings, customerSettings,
                categoryService, currencyService, customerService, dateRangeService,
                dateTimeHelper, downloadService, genericAttributeService, localizationService,
                manufacturerService, permissionService, pictureService, priceCalculationService,
                priceFormatter, productAttributeParser, productAttributeService, productService,
                productTagService, productTemplateService, reviewTypeService, specificationAttributeService,
                staticCacheManager, storeContext, shoppingCartModelFactory, taxService, urlRecordService,
                vendorService, webHelper, workContext, mediaSettings, orderSettings, seoSettings,
                shippingSettings, vendorSettings
            )
        {
            _webHelper = webHelper;
            _abcMattressListingPriceService = abcMattressListingPriceService;
            _priceFormatter = priceFormatter;
            _productAbcDescriptionService = productAbcDescriptionService;
            _productAttributeParser = productAttributeParser;
        }

        protected override async Task<ProductOverviewModel.ProductPriceModel>
            PrepareProductOverviewPriceModelAsync(
                Product product,
                bool forceRedirectionAfterAddingToCart = false
        )
        {
            var model = await base.PrepareProductOverviewPriceModelAsync(product, forceRedirectionAfterAddingToCart);
            model.Price = await AdjustMattressPriceAsync(product.Id) ?? model.Price;

            return model;
        }

        protected override async Task<ProductDetailsModel.ProductPriceModel>
            PrepareProductPriceModelAsync(Product product)
        {
            var model = await base.PrepareProductPriceModelAsync(product);
            model.Price = await AdjustMattressPriceAsync(product.Id) ?? model.Price;

            return model;
        }

        // Excludes product attributes that shouldn't appear by default
        protected override async Task<IList<ProductDetailsModel.ProductAttributeModel>> PrepareProductAttributeModelsAsync(
            Product product,
            ShoppingCartItem updatecartitem
        ) {
            var models = await base.PrepareProductAttributeModelsAsync(product, updatecartitem);

            return models.Where(m => !new string[]{
                "Home Delivery",
                "Warranty",
                "Delivery/Pickup Options",
                "Pickup",
                "Haul Away (Delivery)",
                "Haul Away (Delivery/Install)",
                "FedEx",
                AbcDeliveryConsts.DeliveryAccessoriesProductAttributeName,
                AbcDeliveryConsts.DeliveryInstallAccessoriesProductAttributeName,
                AbcDeliveryConsts.PickupAccessoriesProductAttributeName
            }.Contains(m.Name)).ToList();
        }

        // Allows for returning specific attributes
        public async Task<IList<ProductDetailsModel.ProductAttributeModel>> PrepareProductAttributeModelsAsync(
            Product product,
            ShoppingCartItem updatecartitem,
            string[] attributesToInclude
        ) {
            var models = await base.PrepareProductAttributeModelsAsync(product, updatecartitem);
            var filteredModels = models.Where(m => attributesToInclude.Contains(m.Name)).ToList();

            // Warranty needs to be pre-selected
            if (updatecartitem != null)
            {
                var warrantyModel = filteredModels.FirstOrDefault(m => m.Name == "Warranty");
                if (warrantyModel != null)
                {
                    var selectedValue = (await _productAttributeParser.ParseProductAttributeValuesAsync(updatecartitem.AttributesXml, warrantyModel.Id)).FirstOrDefault();
                    if (selectedValue != null)
                    {
                        selectedValue.IsPreSelected = true;
                    }
                }
            }
            

            return filteredModels;
        }

        private async Task<string> AdjustMattressPriceAsync(int productId)
        {
            var mattressPrice = await _abcMattressListingPriceService.GetListingPriceForMattressProductAsync(
                productId
            );
            if (mattressPrice.HasValue)
            {
                return (await _priceFormatter.FormatPriceAsync(mattressPrice.Value.Price)).Replace(".00", "");
            }

            return null;
        }
    }
}