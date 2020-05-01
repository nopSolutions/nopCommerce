using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Factories;

namespace Nop.Plugin.Misc.PolyCommerce.Factories
{
    public class PolyCommerceProductModelFactory : ProductModelFactory
    {
        public PolyCommerceProductModelFactory(CatalogSettings catalogSettings,
            CurrencySettings currencySettings,
            IAclSupportedModelFactory aclSupportedModelFactory,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICategoryService categoryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IDiscountService discountService,
            IDiscountSupportedModelFactory discountSupportedModelFactory,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IManufacturerService manufacturerService,
            IMeasureService measureService,
            IOrderService orderService,
            IPictureService pictureService,
            IProductAttributeFormatter productAttributeFormatter,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IProductTagService productTagService,
            IProductTemplateService productTemplateService,
            ISettingModelFactory settingModelFactory,
            IShipmentService shipmentService,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            ISpecificationAttributeService specificationAttributeService,
            IStaticCacheManager cacheManager,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IStoreService storeService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            MeasureSettings measureSettings,
            TaxSettings taxSettings,
            VendorSettings vendorSettings) : base(catalogSettings,
             currencySettings,
             aclSupportedModelFactory,
             baseAdminModelFactory,
             categoryService,
             currencyService,
             customerService,
             dateTimeHelper,
             discountService,
             discountSupportedModelFactory,
             localizationService,
             localizedModelFactory,
             manufacturerService,
             measureService,
             orderService,
             pictureService,
             productAttributeFormatter,
             productAttributeParser,
             productAttributeService,
             productService,
             productTagService,
             productTemplateService,
             settingModelFactory,
             shipmentService,
             shippingService,
             shoppingCartService,
             specificationAttributeService,
             cacheManager,
             storeMappingSupportedModelFactory,
             storeService,
             urlRecordService,
            workContext,
            measureSettings,
            taxSettings,
            vendorSettings)
        {

        }

        public override ProductModel PrepareProductModel(ProductModel model, Product product, bool excludeProperties = false)
        {
            var productModel = base.PrepareProductModel(model, product, excludeProperties);

            if (product == null)
            {
                productModel.StockQuantity = 1;
                productModel.ManageInventoryMethodId = (int)ManageInventoryMethod.ManageStock;
                productModel.MinStockQuantity = 1;
                productModel.LowStockActivityId = (int)LowStockActivity.Unpublish;
                productModel.DisplayStockQuantity = true;
            }
            
            return productModel;
        }
    }
}
