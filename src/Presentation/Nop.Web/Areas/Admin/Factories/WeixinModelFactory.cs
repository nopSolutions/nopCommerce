using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Weixin;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
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
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Weixin;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the weixin model factory implementation
    /// </summary>
    public partial class WeixinModelFactory : IWeixinModelFactory
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly IAclSupportedModelFactory _aclSupportedModelFactory;
        private readonly IAddressService _addressService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICategoryService _categoryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IDiscountService _discountService;
        private readonly IDiscountSupportedModelFactory _discountSupportedModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IManufacturerService _manufacturerService;
        private readonly IMeasureService _measureService;
        private readonly IOrderService _orderService;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IProductTagService _productTagService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly ISettingModelFactory _settingModelFactory;
        private readonly IShipmentService _shipmentService;
        private readonly IShippingService _shippingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly MeasureSettings _measureSettings;
        private readonly TaxSettings _taxSettings;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public WeixinModelFactory(
            CurrencySettings currencySettings,
            IAclSupportedModelFactory aclSupportedModelFactory,
            IAddressService addressService,
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
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IStoreService storeService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            MeasureSettings measureSettings,
            TaxSettings taxSettings,
            VendorSettings vendorSettings)
        {
            _currencySettings = currencySettings;
            _aclSupportedModelFactory = aclSupportedModelFactory;
            _addressService = addressService;
            _baseAdminModelFactory = baseAdminModelFactory;
            _categoryService = categoryService;
            _currencyService = currencyService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _discountService = discountService;
            _discountSupportedModelFactory = discountSupportedModelFactory;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _manufacturerService = manufacturerService;
            _measureService = measureService;
            _measureSettings = measureSettings;
            _orderService = orderService;
            _pictureService = pictureService;
            _productAttributeFormatter = productAttributeFormatter;
            _productAttributeParser = productAttributeParser;
            _productAttributeService = productAttributeService;
            _productService = productService;
            _productTagService = productTagService;
            _productTemplateService = productTemplateService;
            _settingModelFactory = settingModelFactory;
            _shipmentService = shipmentService;
            _shippingService = shippingService;
            _shoppingCartService = shoppingCartService;
            _specificationAttributeService = specificationAttributeService;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            _storeService = storeService;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
            _taxSettings = taxSettings;
            _vendorSettings = vendorSettings;
        }

        #endregion

        #region Utilities

        #endregion

        #region Methods

        /// <summary>
        /// Prepare User model
        /// </summary>
        /// <param name="model">User model</param>
        /// <param name="product">User</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Product model</returns>
        public virtual UserModel PrepareUserModel(UserModel model, WUser user, bool excludeProperties = false)
        {
            return model;
        }

        /// <summary>
        /// Prepare User search model
        /// </summary>
        /// <param name="searchModel">User search model</param>
        /// <returns>User search model</returns>
        public virtual UserSearchModel PrepareUserSearchModel(UserSearchModel searchModel)
        {
            return searchModel;
        }

        /// <summary>
        /// Prepare paged User list model
        /// </summary>
        /// <param name="searchModel">User search model</param>
        /// <returns>User list model</returns>
        public virtual UserListModel PrepareUserListModel(UserSearchModel searchModel)
        {
            return null;
        }

        #endregion
    }
}