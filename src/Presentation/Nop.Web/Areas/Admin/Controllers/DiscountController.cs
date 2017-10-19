using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Helpers;
using Nop.Web.Areas.Admin.Models.Discounts;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class DiscountController : BaseAdminController
    {
        #region Fields

        private readonly IDiscountService _discountService;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICurrencyService _currencyService;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly CurrencySettings _currencySettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly IManufacturerService _manufacturerService;
        private readonly IStoreService _storeService;
        private readonly IVendorService _vendorService;
        private readonly IOrderService _orderService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IStaticCacheManager _cacheManager;

        #endregion

        #region Ctor

        public DiscountController(IDiscountService discountService,
            ILocalizationService localizationService,
            ICurrencyService currencyService,
            ICategoryService categoryService,
            IProductService productService,
            IWebHelper webHelper,
            IDateTimeHelper dateTimeHelper,
            ICustomerActivityService customerActivityService,
            CurrencySettings currencySettings,
            CatalogSettings catalogSettings,
            IPermissionService permissionService,
            IWorkContext workContext,
            IManufacturerService manufacturerService,
            IStoreService storeService,
            IVendorService vendorService,
            IOrderService orderService,
            IPriceFormatter priceFormatter,
            IStaticCacheManager cacheManager)
        {
            this._discountService = discountService;
            this._localizationService = localizationService;
            this._currencyService = currencyService;
            this._categoryService = categoryService;
            this._productService = productService;
            this._webHelper = webHelper;
            this._dateTimeHelper = dateTimeHelper;
            this._customerActivityService = customerActivityService;
            this._currencySettings = currencySettings;
            this._catalogSettings = catalogSettings;
            this._permissionService = permissionService;
            this._workContext = workContext;
            this._manufacturerService = manufacturerService;
            this._storeService = storeService;
            this._vendorService = vendorService;
            this._orderService = orderService;
            this._priceFormatter = priceFormatter;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Utilities
        
        protected virtual string GetRequirementUrlInternal(IDiscountRequirementRule discountRequirementRule, Discount discount, int? discountRequirementId)
        {
            if (discountRequirementRule == null)
                throw new ArgumentNullException(nameof(discountRequirementRule));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            var url = $"{_webHelper.GetStoreLocation()}{discountRequirementRule.GetConfigurationUrl(discount.Id, discountRequirementId)}";
            return url;
        }
        
        protected virtual void PrepareDiscountModel(DiscountModel model, Discount discount)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (discount == null)
                return;

            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            model.AvailableDiscountRequirementRules.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Promotions.Discounts.Requirements.DiscountRequirementType.Select"), Value = "" });

            //add item "Add group" in the list
            model.AvailableDiscountRequirementRules.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Promotions.Discounts.Requirements.DiscountRequirementType.AddGroup"), Value = "AddGroup" });

            var discountRules = _discountService.LoadAllDiscountRequirementRules();
            foreach (var discountRule in discountRules)
                model.AvailableDiscountRequirementRules.Add(new SelectListItem { Text = discountRule.PluginDescriptor.FriendlyName, Value = discountRule.PluginDescriptor.SystemName });

            //get available requirement groups
            var requirementGroups = discount.DiscountRequirements.Where(requirement => requirement.IsGroup);
            model.AvailableRequirementGroups = requirementGroups.Select(requirement => 
                new SelectListItem { Value = requirement.Id.ToString(), Text = requirement.DiscountRequirementRuleSystemName }).ToList();
        }
        
        protected IList<DiscountModel.DiscountRequirementMetaInfo> GetReqirements(IEnumerable<DiscountRequirement> requirements,
            RequirementGroupInteractionType groupInteractionType, Discount discount)
        {
            var lastRequirement = requirements.LastOrDefault();

            return requirements.Select(requirement =>
            {
                //set common properties
                var requirementModel = new DiscountModel.DiscountRequirementMetaInfo
                {
                    DiscountRequirementId = requirement.Id,
                    ParentId = requirement.ParentId,
                    IsGroup = requirement.IsGroup,
                    RuleName = requirement.DiscountRequirementRuleSystemName,
                    IsLastInGroup = lastRequirement == null || lastRequirement.Id == requirement.Id,
                    InteractionTypeId = (int)groupInteractionType,
                };

                var interactionType = requirement.InteractionType.HasValue
                    ? requirement.InteractionType.Value : RequirementGroupInteractionType.And;
                requirementModel.AvailableInteractionTypes = interactionType.ToSelectList(true);

                if (requirement.IsGroup)
                {
                    //get child requirements for the group
                    requirementModel.ChildRequirements = GetReqirements(requirement.ChildRequirements, interactionType, discount);
                    return requirementModel;
                }

                //or try to get name and configuration URL for the requirement
                var requirementRule = _discountService.LoadDiscountRequirementRuleBySystemName(requirement.DiscountRequirementRuleSystemName);
                if (requirementRule == null)
                    return null;

                requirementModel.RuleName = requirementRule.PluginDescriptor.FriendlyName;
                requirementModel.ConfigurationUrl = GetRequirementUrlInternal(requirementRule, discount, requirement.Id);

                return requirementModel;
            }).ToList();
        }
        
        protected void DeleteRequirement(ICollection<DiscountRequirement> requirements)
        {
            //recursively delete child requirements
            var tmpRequirements = requirements.ToList();
            for (var i = 0; i < tmpRequirements.Count; i++)
            {
                if (tmpRequirements[i].ChildRequirements.Any())
                    DeleteRequirement(tmpRequirements[i].ChildRequirements);
                _discountService.DeleteDiscountRequirement(tmpRequirements[i]);
            }
        }

        #endregion

        #region Methods

        #region Discounts

        //list
        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var model = new DiscountListModel
            {
                AvailableDiscountTypes = DiscountType.AssignedToOrderTotal.ToSelectList(false).ToList()
            };
            model.AvailableDiscountTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            if (_catalogSettings.IgnoreDiscounts)
                WarningNotification(_localizationService.GetResource("Admin.Promotions.Discounts.IgnoreDiscounts.Warning"));

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(DiscountListModel model, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedKendoGridJson();

            DiscountType? discountType = null;
            if (model.SearchDiscountTypeId > 0)
                discountType = (DiscountType)model.SearchDiscountTypeId;
            var discounts = _discountService.GetAllDiscounts(discountType,
                model.SearchDiscountCouponCode,
                model.SearchDiscountName,
                true);

            var gridModel = new DataSourceResult
            {
                Data = discounts.PagedForCommand(command).Select(x =>
                {
                    var discountModel = x.ToModel();
                    discountModel.DiscountTypeName = x.DiscountType.GetLocalizedEnum(_localizationService, _workContext);
                    discountModel.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
                    discountModel.TimesUsed = _discountService.GetAllDiscountUsageHistory(x.Id, pageSize: 1).TotalCount;
                    return discountModel;
                }),
                Total = discounts.Count
            };

            return Json(gridModel);
        }

        //create
        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var model = new DiscountModel();
            PrepareDiscountModel(model, null);
            //default values
            model.LimitationTimes = 1;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(DiscountModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var discount = model.ToEntity();
                _discountService.InsertDiscount(discount);

                //activity log
                _customerActivityService.InsertActivity("AddNewDiscount", _localizationService.GetResource("ActivityLog.AddNewDiscount"), discount.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Promotions.Discounts.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = discount.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareDiscountModel(model, null);
            return View(model);
        }

        //edit
        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(id);
            if (discount == null)
                //No discount found with the specified id
                return RedirectToAction("List");

            var model = discount.ToModel();
            PrepareDiscountModel(model, discount);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(DiscountModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(model.Id);
            if (discount == null)
                //No discount found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var prevDiscountType = discount.DiscountType;
                discount = model.ToEntity(discount);
                _discountService.UpdateDiscount(discount);

                //clean up old references (if changed) and update "HasDiscountsApplied" properties
                if (prevDiscountType == DiscountType.AssignedToCategories
                    && discount.DiscountType != DiscountType.AssignedToCategories)
                {
                    //applied to categories
                    discount.AppliedToCategories.Clear();
                    _discountService.UpdateDiscount(discount);
                }
                if (prevDiscountType == DiscountType.AssignedToManufacturers
                    && discount.DiscountType != DiscountType.AssignedToManufacturers)
                {
                    //applied to manufacturers
                    discount.AppliedToManufacturers.Clear();
                    _discountService.UpdateDiscount(discount);
                }
                if (prevDiscountType == DiscountType.AssignedToSkus
                    && discount.DiscountType != DiscountType.AssignedToSkus)
                {
                    //applied to products
                    var products = discount.AppliedToProducts.ToList();
                    discount.AppliedToProducts.Clear();
                    _discountService.UpdateDiscount(discount);
                    //update "HasDiscountsApplied" property
                    foreach (var p in products)
                        _productService.UpdateHasDiscountsApplied(p);
                }

                //activity log
                _customerActivityService.InsertActivity("EditDiscount", _localizationService.GetResource("ActivityLog.EditDiscount"), discount.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Promotions.Discounts.Updated"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = discount.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareDiscountModel(model, discount);
            return View(model);
        }

        //delete
        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(id);
            if (discount == null)
                //No discount found with the specified id
                return RedirectToAction("List");

            //applied to products
            var products = discount.AppliedToProducts.ToList();

            _discountService.DeleteDiscount(discount);

            //update "HasDiscountsApplied" properties
            foreach (var p in products)
                _productService.UpdateHasDiscountsApplied(p);

            //activity log
            _customerActivityService.InsertActivity("DeleteDiscount", _localizationService.GetResource("ActivityLog.DeleteDiscount"), discount.Name);

            SuccessNotification(_localizationService.GetResource("Admin.Promotions.Discounts.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Discount requirements

        public virtual IActionResult GetDiscountRequirementConfigurationUrl(string systemName, int discountId, int? discountRequirementId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            if (string.IsNullOrEmpty(systemName))
                throw new ArgumentNullException(nameof(systemName));

            var discountRequirementRule = _discountService.LoadDiscountRequirementRuleBySystemName(systemName);
            if (discountRequirementRule == null)
                throw new ArgumentException("Discount requirement rule could not be loaded");

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new ArgumentException("Discount could not be loaded");

            var url = GetRequirementUrlInternal(discountRequirementRule, discount, discountRequirementId);
            return Json(new { url = url });
        }

        public virtual IActionResult GetDiscountRequirements(int discountId, int discountRequirementId, 
            int? parentId, int? interactionTypeId, bool deleteRequirement)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var requirements = new List<DiscountModel.DiscountRequirementMetaInfo>();

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                return Json(requirements);

            var discountRequirement = discount.DiscountRequirements.FirstOrDefault(requirement => requirement.Id == discountRequirementId);
            if (discountRequirement != null)
            {
                //delete
                if (deleteRequirement)
                {
                    DeleteRequirement(new List<DiscountRequirement> { discountRequirement });

                    //delete default group if there are no any requirements
                    if (!discount.DiscountRequirements.Any(requirement => requirement.ParentId.HasValue))
                        DeleteRequirement(discount.DiscountRequirements);
                }
                //or update the requirement
                else
                {
                    var defaultGroupId = discount.DiscountRequirements.FirstOrDefault(requirement => 
                        !requirement.ParentId.HasValue && requirement.IsGroup)?.Id ?? 0;
                    if (defaultGroupId == 0)
                    {
                        //add default requirement group
                        var defaultGroup = new DiscountRequirement
                        {
                            IsGroup = true,
                            InteractionType = RequirementGroupInteractionType.And,
                            DiscountRequirementRuleSystemName = _localizationService.GetResource("Admin.Promotions.Discounts.Requirements.DefaultRequirementGroup")
                        };
                        discount.DiscountRequirements.Add(defaultGroup);
                        _discountService.UpdateDiscount(discount);
                        defaultGroupId = defaultGroup.Id;
                    }

                    //set parent identifier if specified
                    if (parentId.HasValue)
                        discountRequirement.ParentId = parentId.Value;
                    else
                    {
                        //or default group identifier
                        if (defaultGroupId != discountRequirement.Id)
                            discountRequirement.ParentId = defaultGroupId;
                    }

                    //set interaction type
                    if (interactionTypeId.HasValue)
                        discountRequirement.InteractionTypeId = interactionTypeId;

                    _discountService.UpdateDiscount(discount);
                }
            }

            //get current requirements
            var topLevelRequirements = discount.DiscountRequirements.Where(requirement => !requirement.ParentId.HasValue && requirement.IsGroup).ToList();

            //get interaction type of top-level group
            var interactionType = topLevelRequirements.FirstOrDefault()?.InteractionType;

            if (interactionType.HasValue)
                requirements = GetReqirements(topLevelRequirements, interactionType.Value, discount).ToList();

            //get available groups
            var requirementGroups = discount.DiscountRequirements.Where(requirement => requirement.IsGroup);
            var availableRequirementGroups = requirementGroups.Select(requirement =>
                new SelectListItem { Value = requirement.Id.ToString(), Text = requirement.DiscountRequirementRuleSystemName }).ToList();

            return Json(new { Requirements = requirements, AvailableGroups = availableRequirementGroups });
        }

        public virtual IActionResult AddNewGroup(int discountId, string name)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new ArgumentException("Discount could not be loaded");

            var defaultGroup = discount.DiscountRequirements.FirstOrDefault(requirement => !requirement.ParentId.HasValue && requirement.IsGroup);
            if (defaultGroup == null)
            {
                //add default requirement group
                discount.DiscountRequirements.Add(new DiscountRequirement
                {
                    IsGroup = true,
                    InteractionType = RequirementGroupInteractionType.And,
                    DiscountRequirementRuleSystemName = _localizationService.GetResource("Admin.Promotions.Discounts.Requirements.DefaultRequirementGroup")
                });
            }

            //save new requirement group
            var discountRequirementGroup = new DiscountRequirement
            {
                IsGroup = true,
                DiscountRequirementRuleSystemName = name,
                InteractionType = RequirementGroupInteractionType.And
            };
            discount.DiscountRequirements.Add(discountRequirementGroup);
            _discountService.UpdateDiscount(discount);

            //set identifier as group name (if not specified)
            if (string.IsNullOrEmpty(name))
            {
                discountRequirementGroup.DiscountRequirementRuleSystemName = $"#{discountRequirementGroup.Id}";
                _discountService.UpdateDiscount(discount);
            }

            return Json(new { Result = true, NewRequirementId = discountRequirementGroup.Id });
        }

        #endregion

        #region Applied to products

        [HttpPost]
        public virtual IActionResult ProductList(DataSourceRequest command, int discountId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedKendoGridJson();

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new Exception("No discount found with the specified id");

            var products = discount
                .AppliedToProducts
                .Where(x => !x.Deleted)
                .ToList();
            var gridModel = new DataSourceResult
            {
                Data = products.Select(x => new DiscountModel.AppliedToProductModel
                {
                    ProductId = x.Id,
                    ProductName = x.Name
                }),
                Total = products.Count
            };

            return Json(gridModel);
        }

        public virtual IActionResult ProductDelete(int discountId, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new Exception("No discount found with the specified id");

            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new Exception("No product found with the specified id");

            //remove discount
            if (product.AppliedDiscounts.Count(d => d.Id == discount.Id) > 0)
                product.AppliedDiscounts.Remove(discount);

            _productService.UpdateProduct(product);
            _productService.UpdateHasDiscountsApplied(product);

            return new NullJsonResult();
        }

        public virtual IActionResult ProductAddPopup(int discountId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var model = new DiscountModel.AddProductToDiscountModel();
            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var manufacturers = SelectListHelper.GetManufacturerList(_manufacturerService, _cacheManager, true);
            foreach (var m in manufacturers)
                model.AvailableManufacturers.Add(m);

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var vendors = SelectListHelper.GetVendorList(_vendorService, _cacheManager, true);
            foreach (var v in vendors)
                model.AvailableVendors.Add(v);

            //product types
            model.AvailableProductTypes = ProductType.SimpleProduct.ToSelectList(false).ToList();
            model.AvailableProductTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ProductAddPopupList(DataSourceRequest command, DiscountModel.AddProductToDiscountModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedKendoGridJson();

            var gridModel = new DataSourceResult();
            var products = _productService.SearchProducts(
                categoryIds: new List<int> { model.SearchCategoryId },
                manufacturerId: model.SearchManufacturerId,
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                keywords: model.SearchProductName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true
                );
            gridModel.Data = products.Select(x => x.ToModel());
            gridModel.Total = products.TotalCount;

            return Json(gridModel);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult ProductAddPopup(DiscountModel.AddProductToDiscountModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(model.DiscountId);
            if (discount == null)
                throw new Exception("No discount found with the specified id");

            if (model.SelectedProductIds != null)
            {
                foreach (var id in model.SelectedProductIds)
                {
                    var product = _productService.GetProductById(id);
                    if (product != null)
                    {
                        if (product.AppliedDiscounts.Count(d => d.Id == discount.Id) == 0)
                            product.AppliedDiscounts.Add(discount);

                        _productService.UpdateProduct(product);
                        _productService.UpdateHasDiscountsApplied(product);
                    }
                }
            }

            ViewBag.RefreshPage = true;
            return View(model);
        }

        #endregion

        #region Applied to categories

        [HttpPost]
        public virtual IActionResult CategoryList(DataSourceRequest command, int discountId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedKendoGridJson();

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new Exception("No discount found with the specified id");

            var categories = discount
                .AppliedToCategories
                .Where(x => !x.Deleted)
                .ToList();
            var gridModel = new DataSourceResult
            {
                Data = categories.Select(x => new DiscountModel.AppliedToCategoryModel
                {
                    CategoryId = x.Id,
                    CategoryName = x.GetFormattedBreadCrumb(_categoryService)
                }),
                Total = categories.Count
            };

            return Json(gridModel);
        }

        public virtual IActionResult CategoryDelete(int discountId, int categoryId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new Exception("No discount found with the specified id");

            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null)
                throw new Exception("No category found with the specified id");

            //remove discount
            if (category.AppliedDiscounts.Count(d => d.Id == discount.Id) > 0)
                category.AppliedDiscounts.Remove(discount);

            _categoryService.UpdateCategory(category);

            return new NullJsonResult();
        }

        public virtual IActionResult CategoryAddPopup(int discountId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var model = new DiscountModel.AddCategoryToDiscountModel();
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult CategoryAddPopupList(DataSourceRequest command, DiscountModel.AddCategoryToDiscountModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedKendoGridJson();

            var categories = _categoryService.GetAllCategories(model.SearchCategoryName,
                0, command.Page - 1, command.PageSize, true);
            var gridModel = new DataSourceResult
            {
                Data = categories.Select(x =>
                {
                    var categoryModel = x.ToModel();
                    categoryModel.Breadcrumb = x.GetFormattedBreadCrumb(_categoryService);
                    return categoryModel;
                }),
                Total = categories.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult CategoryAddPopup(DiscountModel.AddCategoryToDiscountModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(model.DiscountId);
            if (discount == null)
                throw new Exception("No discount found with the specified id");

            if (model.SelectedCategoryIds != null)
            {
                foreach (var id in model.SelectedCategoryIds)
                {
                    var category = _categoryService.GetCategoryById(id);
                    if (category != null)
                    {
                        if (category.AppliedDiscounts.Count(d => d.Id == discount.Id) == 0)
                            category.AppliedDiscounts.Add(discount);

                        _categoryService.UpdateCategory(category);
                    }
                }
            }

            ViewBag.RefreshPage = true;
            return View(model);
        }

        #endregion

        #region Applied to manufacturers

        [HttpPost]
        public virtual IActionResult ManufacturerList(DataSourceRequest command, int discountId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedKendoGridJson();

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new Exception("No discount found with the specified id");

            var manufacturers = discount
                .AppliedToManufacturers
                .Where(x => !x.Deleted)
                .ToList();
            var gridModel = new DataSourceResult
            {
                Data = manufacturers.Select(x => new DiscountModel.AppliedToManufacturerModel
                {
                    ManufacturerId = x.Id,
                    ManufacturerName = x.Name
                }),
                Total = manufacturers.Count
            };

            return Json(gridModel);
        }

        public virtual IActionResult ManufacturerDelete(int discountId, int manufacturerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new Exception("No discount found with the specified id");

            var manufacturer = _manufacturerService.GetManufacturerById(manufacturerId);
            if (manufacturer == null)
                throw new Exception("No manufacturer found with the specified id");

            //remove discount
            if (manufacturer.AppliedDiscounts.Count(d => d.Id == discount.Id) > 0)
                manufacturer.AppliedDiscounts.Remove(discount);

            _manufacturerService.UpdateManufacturer(manufacturer);

            return new NullJsonResult();
        }

        public virtual IActionResult ManufacturerAddPopup(int discountId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var model = new DiscountModel.AddManufacturerToDiscountModel();
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ManufacturerAddPopupList(DataSourceRequest command, DiscountModel.AddManufacturerToDiscountModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedKendoGridJson();

            var manufacturers = _manufacturerService.GetAllManufacturers(model.SearchManufacturerName,
                0, command.Page - 1, command.PageSize, true);
            var gridModel = new DataSourceResult
            {
                Data = manufacturers.Select(x => x.ToModel()),
                Total = manufacturers.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult ManufacturerAddPopup(DiscountModel.AddManufacturerToDiscountModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(model.DiscountId);
            if (discount == null)
                throw new Exception("No discount found with the specified id");

            if (model.SelectedManufacturerIds != null)
            {
                foreach (var id in model.SelectedManufacturerIds)
                {
                    var manufacturer = _manufacturerService.GetManufacturerById(id);
                    if (manufacturer != null)
                    {
                        if (manufacturer.AppliedDiscounts.Count(d => d.Id == discount.Id) == 0)
                            manufacturer.AppliedDiscounts.Add(discount);

                        _manufacturerService.UpdateManufacturer(manufacturer);
                    }
                }
            }

            ViewBag.RefreshPage = true;
            return View(model);
        }

        #endregion

        #region Discount usage history

        [HttpPost]
        public virtual IActionResult UsageHistoryList(int discountId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedKendoGridJson();

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new ArgumentException("No discount found with the specified id");

            var duh = _discountService.GetAllDiscountUsageHistory(discount.Id, null, null, command.Page - 1, command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = duh.Select(x =>
                {
                    var order = _orderService.GetOrderById(x.OrderId);
                    var duhModel = new DiscountModel.DiscountUsageHistoryModel
                    {
                        Id = x.Id,
                        DiscountId = x.DiscountId,
                        OrderId = x.OrderId,
                        OrderTotal = order != null ? _priceFormatter.FormatPrice(order.OrderTotal, true, false) : "",
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc),
                        CustomOrderNumber = x.Order.CustomOrderNumber
                    };
                    return duhModel;
                }),
                Total = duh.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult UsageHistoryDelete(int discountId, int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new ArgumentException("No discount found with the specified id");

            var duh = _discountService.GetDiscountUsageHistoryById(id);
            if (duh != null)
                _discountService.DeleteDiscountUsageHistory(duh);

            return new NullJsonResult();
        }

        #endregion

        #endregion
    }
}