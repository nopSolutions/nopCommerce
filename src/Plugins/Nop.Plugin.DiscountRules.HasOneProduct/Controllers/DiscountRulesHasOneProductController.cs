using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Plugin.DiscountRules.HasOneProduct.Models;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.DiscountRules.HasOneProduct.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class DiscountRulesHasOneProductController : BasePluginController
    {
        #region Fields

        private readonly ICategoryService _categoryService;
        private readonly IDiscountService _discountService;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPermissionService _permissionService;
        private readonly IProductService _productService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public DiscountRulesHasOneProductController(ICategoryService categoryService,
            IDiscountService discountService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            IPermissionService permissionService,
            IProductService productService,
            ISettingService settingService,
            IStoreService storeService,
            IVendorService vendorService,
            IWorkContext workContext)
        {
            this._categoryService = categoryService;
            this._discountService = discountService;
            this._localizationService = localizationService;
            this._manufacturerService = manufacturerService;
            this._permissionService = permissionService;
            this._productService = productService;
            this._settingService = settingService;
            this._storeService = storeService;
            this._vendorService = vendorService;
            this._workContext = workContext;
        }

        #endregion

        #region Methods

        public IActionResult Configure(int discountId, int? discountRequirementId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return Content("Access denied");

            //load the discount
            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new ArgumentException("Discount could not be loaded");

            //check whether the discount requirement exists
            if (discountRequirementId.HasValue && !discount.DiscountRequirements.Any(requirement => requirement.Id == discountRequirementId.Value))
                return Content("Failed to load requirement.");

            //try to get previously saved restricted product identifiers
            var restrictedProductIds = _settingService.GetSettingByKey<string>(string.Format(DiscountRequirementDefaults.SettingsKey, discountRequirementId ?? 0));

            var model = new RequirementModel
            {
                RequirementId = discountRequirementId ?? 0,
                DiscountId = discountId,
                Products = restrictedProductIds
            };

            //set the HTML field prefix
            ViewData.TemplateInfo.HtmlFieldPrefix = string.Format(DiscountRequirementDefaults.HtmlFieldPrefix, discountRequirementId ?? 0);

            return View("~/Plugins/DiscountRules.HasOneProduct/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult Configure(int discountId, int? discountRequirementId, string productIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return Content("Access denied");

            //load the discount
            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new ArgumentException("Discount could not be loaded");

            //get the discount requirement
            var discountRequirement = discountRequirementId.HasValue
                ? discount.DiscountRequirements.FirstOrDefault(requirement => requirement.Id == discountRequirementId.Value) : null;

            //the discount requirement does not exist, so create a new one
            if (discountRequirement == null)
            {
                discountRequirement = new DiscountRequirement
                {
                    DiscountRequirementRuleSystemName = DiscountRequirementDefaults.SystemName
                };
                discount.DiscountRequirements.Add(discountRequirement);
                _discountService.UpdateDiscount(discount);
            }

            //save restricted product identifiers
            _settingService.SetSetting(string.Format(DiscountRequirementDefaults.SettingsKey, discountRequirement.Id), productIds);

            return Json(new { Result = true, NewRequirementId = discountRequirement.Id });
        }

        public IActionResult ProductAddPopup()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var model = new RequirementModel.AddProductModel();

            //a vendor should have access only to his products
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;

            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = _categoryService.GetAllCategories(showHidden: true);
            foreach (var category in categories)
                model.AvailableCategories.Add(new SelectListItem { Text = category.GetFormattedBreadCrumb(categories), Value = category.Id.ToString() });

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var manufacturer in _manufacturerService.GetAllManufacturers(showHidden: true))
                model.AvailableManufacturers.Add(new SelectListItem { Text = manufacturer.Name, Value = manufacturer.Id.ToString() });

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var store in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = store.Name, Value = store.Id.ToString() });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var vendor in _vendorService.GetAllVendors(showHidden: true))
                model.AvailableVendors.Add(new SelectListItem { Text = vendor.Name, Value = vendor.Id.ToString() });

            //product types
            model.AvailableProductTypes = ProductType.SimpleProduct.ToSelectList(false).ToList();
            model.AvailableProductTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            
            return View("~/Plugins/DiscountRules.HasOneProduct/Views/ProductAddPopup.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult ProductAddPopupList(DataSourceRequest command, RequirementModel.AddProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
                model.SearchVendorId = _workContext.CurrentVendor.Id;

            var products = _productService.SearchProducts(
                categoryIds: new List<int> { model.SearchCategoryId },
                manufacturerId: model.SearchManufacturerId,
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                keywords: model.SearchProductName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true);

            var gridModel = new DataSourceResult
            {
                Data = products.Select(product => new RequirementModel.ProductModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Published = product.Published
                }),
                Total = products.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult LoadProductFriendlyNames(string productIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return Json(new { Text = string.Empty });

            if (string.IsNullOrWhiteSpace(productIds))
                return Json(new { Text = string.Empty });

            var result = string.Empty;
            var ids = new List<int>();
            var rangeArray = productIds.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();

            //we support three ways of specifying products:
            //1. The comma-separated list of product identifiers (e.g. 77, 123, 156).
            //2. The comma-separated list of product identifiers with quantities.
            //      {Product ID}:{Quantity}. For example, 77:1, 123:2, 156:3
            //3. The comma-separated list of product identifiers with quantity range.
            //      {Product ID}:{Min quantity}-{Max quantity}. For example, 77:1-3, 123:2-5, 156:3-8
            foreach (string str1 in rangeArray)
            {
                var str2 = str1;
                //we do not display specified quantities and ranges
                //so let's parse only product names (before : sign)
                if (str2.Contains(":"))
                    str2 = str2.Substring(0, str2.IndexOf(":"));

                if (int.TryParse(str2, out int tmp1))
                    ids.Add(tmp1);
            }

            var products = _productService.GetProductsByIds(ids.ToArray());
            for (int i = 0; i <= products.Count - 1; i++)
            {
                result += products[i].Name;
                if (i != products.Count - 1)
                    result += ", ";
            }

            return Json(new { Text = result });
        }

        #endregion
    }
}