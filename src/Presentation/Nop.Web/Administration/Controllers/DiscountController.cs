using System;
using System.Linq;
using System.Web.Mvc;
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
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public partial class DiscountController : BaseNopController
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
        protected string GetRequirementUrlInternal(IDiscountRequirementRule discountRequirementRule, Discount discount, int? discountRequirementId)
        {   
            if (discountRequirementRule == null)
                throw new ArgumentNullException("discountRequirementRule");

            if (discount == null)
                throw new ArgumentNullException("discount");

            string url = string.Format("{0}{1}", _webHelper.GetStoreLocation(), discountRequirementRule.GetConfigurationUrl(discount.Id, discountRequirementId));
            return url;
        }
        
        [NonAction]
        protected void PrepareDiscountModel(DiscountModel model, Discount discount)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            model.AvailableDiscountRequirementRules.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Promotions.Discounts.Requirements.DiscountRequirementType.Select"), Value = "" });
            var discountRules = _discountService.LoadAllDiscountRequirementRules();
            foreach (var discountRule in discountRules)
                model.AvailableDiscountRequirementRules.Add(new SelectListItem() { Text = discountRule.PluginDescriptor.FriendlyName, Value = discountRule.PluginDescriptor.SystemName });

            if (discount != null)
            {
                //applied to categories
                foreach (var category in discount.AppliedToCategories)
                {
                    if (category != null && !category.Deleted)
                    {
                        model.AppliedToCategoryModels.Add(new DiscountModel.AppliedToCategoryModel()
                        {
                            CategoryId = category.Id,
                            Name = category.Name
                        });
                    }
                }

                //applied to product variants
                foreach (var pv in discount.AppliedToProductVariants)
                {
                    if (pv != null && !pv.Deleted)
                    {
                        var appliedToProductVariantModel = new DiscountModel.AppliedToProductVariantModel()
                        {
                            ProductVariantId = pv.Id,
                        };
                        //full product name
                        if (!String.IsNullOrEmpty(pv.Name))
                            appliedToProductVariantModel.FullProductName = string.Format("{0} ({1})", pv.Product.Name, pv.Name);
                        else
                            appliedToProductVariantModel.FullProductName = pv.Product.Name;

                        model.AppliedToProductVariantModels.Add(appliedToProductVariantModel);
                    }
                }

                //requirements
                foreach (var dr in discount.DiscountRequirements.OrderBy(dr=>dr.Id))
                {
                    var drr = _discountService.LoadDiscountRequirementRuleBySystemName(dr.DiscountRequirementRuleSystemName);
                    if (drr != null)
                    {
                        model.DiscountRequirementMetaInfos.Add(new DiscountModel.DiscountRequirementMetaInfo()
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

            var gridModel = new GridModel<DiscountModel>();
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discounts = _discountService.GetAllDiscounts(null, null, true);
            var gridModel = new GridModel<DiscountModel>
            {
                Data = discounts.PagedForCommand(command).Select(x => x.ToModel()),
                Total = discounts.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
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

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
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

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
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
                    //applied to product variants
                    var productVariants = discount.AppliedToProductVariants.ToList();
                    discount.AppliedToProductVariants.Clear();
                    _discountService.UpdateDiscount(discount);
                    //update "HasDiscountsApplied" property
                    foreach (var pv in productVariants)
                        _productService.UpdateHasDiscountsApplied(pv);
                }

                //activity log
                _customerActivityService.InsertActivity("EditDiscount", _localizationService.GetResource("ActivityLog.EditDiscount"), discount.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Promotions.Discounts.Updated"));
                return continueEditing ? RedirectToAction("Edit", discount.Id) : RedirectToAction("List");
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
            //applied to product variants
            var productVariants = discount.AppliedToProductVariants.ToList();

            _discountService.DeleteDiscount(discount);
            
            //update "HasDiscountsApplied" properties
            foreach (var category in categories)
                _categoryService.UpdateHasDiscountsApplied(category);
            foreach (var pv in productVariants)
                _productService.UpdateHasDiscountsApplied(pv);

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

            var discountRequirement = discount.DiscountRequirements.Where(dr => dr.Id == discountRequirementId).FirstOrDefault();
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

            var discountRequirement = discount.DiscountRequirements.Where(dr => dr.Id == discountRequirementId).FirstOrDefault();
            if (discountRequirement == null)
                throw new ArgumentException("Discount requirement could not be loaded");

            _discountService.DeleteDiscountRequirement(discountRequirement);

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Discount usage history
        
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult UsageHistoryList(int discountId, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new ArgumentException("No discount found with the specified id");

            var duh = _discountService.GetAllDiscountUsageHistory(discount.Id, null, command.Page - 1, command.PageSize);
            
            var model = new GridModel<DiscountModel.DiscountUsageHistoryModel>
            {
                Data = duh.Select(x =>
                {
                    return new DiscountModel.DiscountUsageHistoryModel()
                    {
                        Id = x.Id,
                        DiscountId = x.DiscountId,
                        OrderId = x.OrderId,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc)
                    };
                }),
                Total = duh.TotalCount
            };
            return new JsonResult
            {
                Data = model
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult UsageHistoryDelete(int discountId, int id, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new ArgumentException("No discount found with the specified id");
            
            var duh = _discountService.GetDiscountUsageHistoryById(id);
            if (duh != null)
                _discountService.DeleteDiscountUsageHistory(duh);
            return UsageHistoryList(discountId, command);
        }

        #endregion
    }
}
