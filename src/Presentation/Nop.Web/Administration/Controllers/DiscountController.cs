using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Models.Discounts;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Controllers
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
        private readonly IPermissionService _permissionService;

        #endregion

        #region Constructors

        public DiscountController(IDiscountService discountService, 
            ILocalizationService localizationService, ICurrencyService currencyService,
            ICategoryService categoryService, IProductService productService,
            IWebHelper webHelper, IDateTimeHelper dateTimeHelper,
            ICustomerActivityService customerActivityService, CurrencySettings currencySettings,
            IPermissionService permissionService)
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
            this._permissionService = permissionService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual string GetRequirementUrlInternal(IDiscountRequirementRule discountRequirementRule, Discount discount, int? discountRequirementId)
        {   
            if (discountRequirementRule == null)
                throw new ArgumentNullException("discountRequirementRule");

            if (discount == null)
                throw new ArgumentNullException("discount");

            string url = string.Format("{0}{1}", _webHelper.GetStoreLocation(), discountRequirementRule.GetConfigurationUrl(discount.Id, discountRequirementId));
            return url;
        }
        
        [NonAction]
        protected virtual void PrepareDiscountModel(DiscountModel model, Discount discount)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            model.AvailableDiscountRequirementRules.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Promotions.Discounts.Requirements.DiscountRequirementType.Select"), Value = "" });
            var discountRules = _discountService.LoadAllDiscountRequirementRules();
            foreach (var discountRule in discountRules)
                model.AvailableDiscountRequirementRules.Add(new SelectListItem { Text = discountRule.PluginDescriptor.FriendlyName, Value = discountRule.PluginDescriptor.SystemName });

            if (discount != null)
            {
                //applied to categories
                foreach (var category in discount.AppliedToCategories)
                {
                    if (category != null && !category.Deleted)
                    {
                        model.AppliedToCategoryModels.Add(new DiscountModel.AppliedToCategoryModel
                        {
                            CategoryId = category.Id,
                            Name = category.Name
                        });
                    }
                }

                //applied to products
                foreach (var product in discount.AppliedToProducts)
                {
                    if (product != null && !product.Deleted)
                    {
                        var appliedToProductModel = new DiscountModel.AppliedToProductModel
                        {
                            ProductId = product.Id,
                            ProductName = product.Name
                        };
                        model.AppliedToProductModels.Add(appliedToProductModel);
                    }
                }

                //requirements
                foreach (var dr in discount.DiscountRequirements.OrderBy(dr=>dr.Id))
                {
                    var drr = _discountService.LoadDiscountRequirementRuleBySystemName(dr.DiscountRequirementRuleSystemName);
                    if (drr != null)
                    {
                        model.DiscountRequirementMetaInfos.Add(new DiscountModel.DiscountRequirementMetaInfo
                        {
                            DiscountRequirementId = dr.Id,
                            RuleName = drr.PluginDescriptor.FriendlyName,
                            ConfigurationUrl = GetRequirementUrlInternal(drr, discount, dr.Id)
                        });
                    }
                }
            }
        }

        #endregion

        #region Discounts

        //list
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discounts = _discountService.GetAllDiscounts(null, null, true);
            var gridModel = new DataSourceResult
            {
                Data = discounts.PagedForCommand(command).Select(x =>
                {
                    var discountModel = x.ToModel();
                    discountModel.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
                    return discountModel;
                }),
                Total = discounts.Count
            };

            return Json(gridModel);
        }
        
        //create
        public ActionResult Create()
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
        public ActionResult Create(DiscountModel model, bool continueEditing)
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
                return continueEditing ? RedirectToAction("Edit", new { id = discount.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareDiscountModel(model, null);
            return View(model);
        }

        //edit
        public ActionResult Edit(int id)
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
        public ActionResult Edit(DiscountModel model, bool continueEditing)
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
                    var categories = discount.AppliedToCategories.ToList();
                    discount.AppliedToCategories.Clear();
                    _discountService.UpdateDiscount(discount);
                    //update "HasDiscountsApplied" property
                    foreach (var category in categories)
                        _categoryService.UpdateHasDiscountsApplied(category);
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
                    SaveSelectedTabIndex();

                    return RedirectToAction("Edit", discount.Id);
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareDiscountModel(model, discount);
            return View(model);
        }

        //delete
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(id);
            if (discount == null)
                //No discount found with the specified id
                return RedirectToAction("List");

            //applied to categories
            var categories = discount.AppliedToCategories.ToList();
            //applied to products
            var products = discount.AppliedToProducts.ToList();

            _discountService.DeleteDiscount(discount);
            
            //update "HasDiscountsApplied" properties
            foreach (var category in categories)
                _categoryService.UpdateHasDiscountsApplied(category);
            foreach (var p in products)
                _productService.UpdateHasDiscountsApplied(p);

            //activity log
            _customerActivityService.InsertActivity("DeleteDiscount", _localizationService.GetResource("ActivityLog.DeleteDiscount"), discount.Name);

            SuccessNotification(_localizationService.GetResource("Admin.Promotions.Discounts.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Discount requirements

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetDiscountRequirementConfigurationUrl(string systemName, int discountId, int? discountRequirementId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            if (String.IsNullOrEmpty(systemName))
                throw new ArgumentNullException("systemName");
            
            var discountRequirementRule = _discountService.LoadDiscountRequirementRuleBySystemName(systemName);
            if (discountRequirementRule == null)
                throw new ArgumentException("Discount requirement rule could not be loaded");

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new ArgumentException("Discount could not be loaded");

            string url = GetRequirementUrlInternal(discountRequirementRule, discount, discountRequirementId);
            return Json(new { url = url }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDiscountRequirementMetaInfo(int discountRequirementId, int discountId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new ArgumentException("Discount could not be loaded");

            var discountRequirement = discount.DiscountRequirements.FirstOrDefault(dr => dr.Id == discountRequirementId);
            if (discountRequirement == null)
                throw new ArgumentException("Discount requirement could not be loaded");

            var discountRequirementRule = _discountService.LoadDiscountRequirementRuleBySystemName(discountRequirement.DiscountRequirementRuleSystemName);
            if (discountRequirementRule == null)
                throw new ArgumentException("Discount requirement rule could not be loaded");

            string url = GetRequirementUrlInternal(discountRequirementRule, discount, discountRequirementId);
            string ruleName = discountRequirementRule.PluginDescriptor.FriendlyName;
            return Json(new { url = url, ruleName = ruleName }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteDiscountRequirement(int discountRequirementId, int discountId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new ArgumentException("Discount could not be loaded");

            var discountRequirement = discount.DiscountRequirements.FirstOrDefault(dr => dr.Id == discountRequirementId);
            if (discountRequirement == null)
                throw new ArgumentException("Discount requirement could not be loaded");

            _discountService.DeleteDiscountRequirement(discountRequirement);

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Discount usage history
        
        [HttpPost]
        public ActionResult UsageHistoryList(int discountId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new ArgumentException("No discount found with the specified id");

            var duh = _discountService.GetAllDiscountUsageHistory(discount.Id, null, null, command.Page - 1, command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = duh.Select(x => new DiscountModel.DiscountUsageHistoryModel
                {
                    Id = x.Id,
                    DiscountId = x.DiscountId,
                    OrderId = x.OrderId,
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc)
                }),
                Total = duh.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult UsageHistoryDelete(int discountId, int id)
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
    }
}
