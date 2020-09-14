using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Services.Catalog;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Discounts;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class DiscountController : BaseAdminController
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly ICategoryService _categoryService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IDiscountModelFactory _discountModelFactory;
        private readonly IDiscountPluginManager _discountPluginManager;
        private readonly IDiscountService _discountService;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IProductService _productService;

        #endregion

        #region Ctor

        public DiscountController(CatalogSettings catalogSettings,
            ICategoryService categoryService,
            ICustomerActivityService customerActivityService,
            IDiscountModelFactory discountModelFactory,
            IDiscountPluginManager discountPluginManager,
            IDiscountService discountService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IProductService productService)
        {
            _catalogSettings = catalogSettings;
            _categoryService = categoryService;
            _customerActivityService = customerActivityService;
            _discountModelFactory = discountModelFactory;
            _discountPluginManager = discountPluginManager;
            _discountService = discountService;
            _localizationService = localizationService;
            _manufacturerService = manufacturerService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _productService = productService;
        }

        #endregion

        #region Methods

        #region Discounts

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //whether discounts are ignored
            if (_catalogSettings.IgnoreDiscounts)
                _notificationService.WarningNotification(await _localizationService.GetResource("Admin.Promotions.Discounts.IgnoreDiscounts.Warning"));

            //prepare model
            var model = await _discountModelFactory.PrepareDiscountSearchModel(new DiscountSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(DiscountSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = await _discountModelFactory.PrepareDiscountListModel(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //prepare model
            var model = await _discountModelFactory.PrepareDiscountModel(new DiscountModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(DiscountModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var discount = model.ToEntity<Discount>();
                await _discountService.InsertDiscount(discount);

                //activity log
                await _customerActivityService.InsertActivity("AddNewDiscount",
                    string.Format(await _localizationService.GetResource("ActivityLog.AddNewDiscount"), discount.Name), discount);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Promotions.Discounts.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = discount.Id });
            }

            //prepare model
            model = await _discountModelFactory.PrepareDiscountModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //try to get a discount with the specified id
            var discount = await _discountService.GetDiscountById(id);
            if (discount == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _discountModelFactory.PrepareDiscountModel(null, discount);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(DiscountModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //try to get a discount with the specified id
            var discount = await _discountService.GetDiscountById(model.Id);
            if (discount == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var prevDiscountType = discount.DiscountType;
                discount = model.ToEntity(discount);
                await _discountService.UpdateDiscount(discount);

                //clean up old references (if changed) 
                if (prevDiscountType != discount.DiscountType)
                {
                    switch (prevDiscountType)
                    {
                        case DiscountType.AssignedToSkus:
                            await _productService.ClearDiscountProductMapping(discount);
                            break;
                        case DiscountType.AssignedToCategories:
                            await _categoryService.ClearDiscountCategoryMapping(discount);
                            break;
                        case DiscountType.AssignedToManufacturers:
                            await _manufacturerService.ClearDiscountManufacturerMapping(discount);
                            break;
                        default:
                            break;
                    }
                }

                //activity log
                await _customerActivityService.InsertActivity("EditDiscount",
                    string.Format(await _localizationService.GetResource("ActivityLog.EditDiscount"), discount.Name), discount);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Promotions.Discounts.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = discount.Id });
            }

            //prepare model
            model = await _discountModelFactory.PrepareDiscountModel(model, discount, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //try to get a discount with the specified id
            var discount = await _discountService.GetDiscountById(id);
            if (discount == null)
                return RedirectToAction("List");

            //applied to products
            var products = await _productService.GetProductsWithAppliedDiscount(discount.Id, true);

            await _discountService.DeleteDiscount(discount);

            //update "HasDiscountsApplied" properties
            foreach (var p in products)
                await _productService.UpdateHasDiscountsApplied(p);

            //activity log
            await _customerActivityService.InsertActivity("DeleteDiscount",
                string.Format(await _localizationService.GetResource("ActivityLog.DeleteDiscount"), discount.Name), discount);

            _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Promotions.Discounts.Deleted"));

            return RedirectToAction("List");
        }

        #endregion

        #region Discount requirements

        public virtual async Task<IActionResult> GetDiscountRequirementConfigurationUrl(string systemName, int discountId, int? discountRequirementId)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            if (string.IsNullOrEmpty(systemName))
                throw new ArgumentNullException(nameof(systemName));

            var discountRequirementRule = _discountPluginManager.LoadPluginBySystemName(systemName)
                ?? throw new ArgumentException("Discount requirement rule could not be loaded");

            var discount = await _discountService.GetDiscountById(discountId)
                ?? throw new ArgumentException("Discount could not be loaded");

            var url = discountRequirementRule.GetConfigurationUrl(discount.Id, discountRequirementId);

            return Json(new { url });
        }

        public virtual async Task<IActionResult> GetDiscountRequirements(int discountId, int discountRequirementId,
            int? parentId, int? interactionTypeId, bool deleteRequirement)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var requirements = new List<DiscountRequirementRuleModel>();

            var discount = await _discountService.GetDiscountById(discountId);
            if (discount == null)
                return Json(requirements);

            var discountRequirement = await _discountService.GetDiscountRequirementById(discountRequirementId);
            if (discountRequirement != null)
            {
                //delete
                if (deleteRequirement)
                {
                    await _discountService.DeleteDiscountRequirement(discountRequirement, true);

                    var discountRequirements = await _discountService.GetAllDiscountRequirements(discount.Id);

                    //delete default group if there are no any requirements
                    if (!discountRequirements.Any(requirement => requirement.ParentId.HasValue))
                    {
                        foreach (var dr in discountRequirements)
                            await _discountService.DeleteDiscountRequirement(dr, true);
                    }
                }
                //or update the requirement
                else
                {
                    var defaultGroupId = (await _discountService.GetAllDiscountRequirements(discount.Id, true)).FirstOrDefault(requirement => requirement.IsGroup)?.Id ?? 0;
                    if (defaultGroupId == 0)
                    {
                        //add default requirement group
                        var defaultGroup = new DiscountRequirement
                        {
                            IsGroup = true,
                            DiscountId = discount.Id,
                            InteractionType = RequirementGroupInteractionType.And,
                            DiscountRequirementRuleSystemName = await _localizationService
                                .GetResource("Admin.Promotions.Discounts.Requirements.DefaultRequirementGroup")
                        };

                        await _discountService.InsertDiscountRequirement(defaultGroup);

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

                    await _discountService.UpdateDiscountRequirement(discountRequirement);
                }
            }

            //get current requirements
            var topLevelRequirements = (await _discountService.GetAllDiscountRequirements(discount.Id, true)).Where(requirement => requirement.IsGroup).ToList();

            //get interaction type of top-level group
            var interactionType = topLevelRequirements.FirstOrDefault()?.InteractionType;

            if (interactionType.HasValue)
            {
                requirements = (await _discountModelFactory
                    .PrepareDiscountRequirementRuleModels(topLevelRequirements, discount, interactionType.Value)).ToList();
            }

            //get available groups
            var requirementGroups = (await _discountService.GetAllDiscountRequirements(discount.Id)).Where(requirement => requirement.IsGroup);

            var availableRequirementGroups = requirementGroups.Select(requirement =>
                new SelectListItem { Value = requirement.Id.ToString(), Text = requirement.DiscountRequirementRuleSystemName }).ToList();

            return Json(new { Requirements = requirements, AvailableGroups = availableRequirementGroups });
        }

        public virtual async Task<IActionResult> AddNewGroup(int discountId, string name)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = await _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new ArgumentException("Discount could not be loaded");

            var defaultGroup = (await _discountService.GetAllDiscountRequirements(discount.Id, true)).FirstOrDefault(requirement => requirement.IsGroup);
            if (defaultGroup == null)
            {
                //add default requirement group
                await _discountService.InsertDiscountRequirement(new DiscountRequirement
                {
                    DiscountId = discount.Id,
                    IsGroup = true,
                    InteractionType = RequirementGroupInteractionType.And,
                    DiscountRequirementRuleSystemName = await _localizationService
                        .GetResource("Admin.Promotions.Discounts.Requirements.DefaultRequirementGroup")
                });
            }

            //save new requirement group
            var discountRequirementGroup = new DiscountRequirement
            {
                DiscountId = discount.Id,
                IsGroup = true,
                DiscountRequirementRuleSystemName = name,
                InteractionType = RequirementGroupInteractionType.And
            };

            await _discountService.InsertDiscountRequirement(discountRequirementGroup);

            if (!string.IsNullOrEmpty(name))
                return Json(new { Result = true, NewRequirementId = discountRequirementGroup.Id });

            //set identifier as group name (if not specified)
            discountRequirementGroup.DiscountRequirementRuleSystemName = $"#{discountRequirementGroup.Id}";
            await _discountService.UpdateDiscountRequirement(discountRequirementGroup);

            await _discountService.UpdateDiscount(discount);

            return Json(new { Result = true, NewRequirementId = discountRequirementGroup.Id });
        }

        #endregion

        #region Applied to products

        [HttpPost]
        public virtual async Task<IActionResult> ProductList(DiscountProductSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedDataTablesJson();

            //try to get a discount with the specified id
            var discount = await _discountService.GetDiscountById(searchModel.DiscountId)
                ?? throw new ArgumentException("No discount found with the specified id");

            //prepare model
            var model = await _discountModelFactory.PrepareDiscountProductListModel(searchModel, discount);

            return Json(model);
        }

        public virtual async Task<IActionResult> ProductDelete(int discountId, int productId)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //try to get a discount with the specified id
            var discount = await _discountService.GetDiscountById(discountId)
                ?? throw new ArgumentException("No discount found with the specified id", nameof(discountId));

            //try to get a product with the specified id
            var product = await _productService.GetProductById(productId)
                ?? throw new ArgumentException("No product found with the specified id", nameof(productId));

            //remove discount
            if (await _productService.GetDiscountAppliedToProduct(product.Id, discount.Id) is DiscountProductMapping discountProductMapping)
                await _productService.DeleteDiscountProductMapping(discountProductMapping);

            await _productService.UpdateProduct(product);
            await _productService.UpdateHasDiscountsApplied(product);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> ProductAddPopup(int discountId)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //prepare model
            var model = await _discountModelFactory.PrepareAddProductToDiscountSearchModel(new AddProductToDiscountSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductAddPopupList(AddProductToDiscountSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = await _discountModelFactory.PrepareAddProductToDiscountListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> ProductAddPopup(AddProductToDiscountModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //try to get a discount with the specified id
            var discount = await _discountService.GetDiscountById(model.DiscountId)
                ?? throw new ArgumentException("No discount found with the specified id");

            var selectedProducts = await _productService.GetProductsByIds(model.SelectedProductIds.ToArray());
            if (selectedProducts.Any())
            {
                foreach (var product in selectedProducts)
                {
                    if (await _productService.GetDiscountAppliedToProduct(product.Id, discount.Id) is null)
                        await _productService.InsertDiscountProductMapping(new DiscountProductMapping { EntityId = product.Id, DiscountId = discount.Id });

                    await _productService.UpdateProduct(product);
                    await _productService.UpdateHasDiscountsApplied(product);
                }
            }

            ViewBag.RefreshPage = true;

            return View(new AddProductToDiscountSearchModel());
        }

        #endregion

        #region Applied to categories

        [HttpPost]
        public virtual async Task<IActionResult> CategoryList(DiscountCategorySearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedDataTablesJson();

            //try to get a discount with the specified id
            var discount = await _discountService.GetDiscountById(searchModel.DiscountId)
                ?? throw new ArgumentException("No discount found with the specified id");

            //prepare model
            var model = await _discountModelFactory.PrepareDiscountCategoryListModel(searchModel, discount);

            return Json(model);
        }

        public virtual async Task<IActionResult> CategoryDelete(int discountId, int categoryId)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //try to get a discount with the specified id
            var discount = await _discountService.GetDiscountById(discountId)
                ?? throw new ArgumentException("No discount found with the specified id", nameof(discountId));

            //try to get a category with the specified id
            var category = await _categoryService.GetCategoryById(categoryId)
                ?? throw new ArgumentException("No category found with the specified id", nameof(categoryId));

            //remove discount
            if (await _categoryService.GetDiscountAppliedToCategory(category.Id, discount.Id) is DiscountCategoryMapping mapping)
                await _categoryService.DeleteDiscountCategoryMapping(mapping);

            await _categoryService.UpdateCategory(category);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> CategoryAddPopup(int discountId)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //prepare model
            var model = await _discountModelFactory.PrepareAddCategoryToDiscountSearchModel(new AddCategoryToDiscountSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CategoryAddPopupList(AddCategoryToDiscountSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = await _discountModelFactory.PrepareAddCategoryToDiscountListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> CategoryAddPopup(AddCategoryToDiscountModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //try to get a discount with the specified id
            var discount = await _discountService.GetDiscountById(model.DiscountId)
                ?? throw new ArgumentException("No discount found with the specified id");

            foreach (var id in model.SelectedCategoryIds)
            {
                var category = await _categoryService.GetCategoryById(id);
                if (category == null)
                    continue;

                if (await _categoryService.GetDiscountAppliedToCategory(category.Id, discount.Id) is null)
                    await _categoryService.InsertDiscountCategoryMapping(new DiscountCategoryMapping { DiscountId = discount.Id, EntityId = category.Id });

                await _categoryService.UpdateCategory(category);
            }

            ViewBag.RefreshPage = true;

            return View(new AddCategoryToDiscountSearchModel());
        }

        #endregion

        #region Applied to manufacturers

        [HttpPost]
        public virtual async Task<IActionResult> ManufacturerList(DiscountManufacturerSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedDataTablesJson();

            //try to get a discount with the specified id
            var discount = await _discountService.GetDiscountById(searchModel.DiscountId)
                ?? throw new ArgumentException("No discount found with the specified id");

            //prepare model
            var model = await _discountModelFactory.PrepareDiscountManufacturerListModel(searchModel, discount);

            return Json(model);
        }

        public virtual async Task<IActionResult> ManufacturerDelete(int discountId, int manufacturerId)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //try to get a discount with the specified id
            var discount = await _discountService.GetDiscountById(discountId)
                ?? throw new ArgumentException("No discount found with the specified id", nameof(discountId));

            //try to get a manufacturer with the specified id
            var manufacturer = await _manufacturerService.GetManufacturerById(manufacturerId)
                ?? throw new ArgumentException("No manufacturer found with the specified id", nameof(manufacturerId));

            //remove discount
            if (await _manufacturerService.GetDiscountAppliedToManufacturer(manufacturer.Id, discount.Id) is DiscountManufacturerMapping discountManufacturerMapping)
                await _manufacturerService.DeleteDiscountManufacturerMapping(discountManufacturerMapping);

            await _manufacturerService.UpdateManufacturer(manufacturer);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> ManufacturerAddPopup(int discountId)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //prepare model
            var model = await _discountModelFactory.PrepareAddManufacturerToDiscountSearchModel(new AddManufacturerToDiscountSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ManufacturerAddPopupList(AddManufacturerToDiscountSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = await _discountModelFactory.PrepareAddManufacturerToDiscountListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> ManufacturerAddPopup(AddManufacturerToDiscountModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //try to get a discount with the specified id
            var discount = await _discountService.GetDiscountById(model.DiscountId)
                ?? throw new ArgumentException("No discount found with the specified id");

            foreach (var id in model.SelectedManufacturerIds)
            {
                var manufacturer = await _manufacturerService.GetManufacturerById(id);
                if (manufacturer == null)
                    continue;

                if (await _manufacturerService.GetDiscountAppliedToManufacturer(manufacturer.Id, discount.Id) is null)
                    await _manufacturerService.InsertDiscountManufacturerMapping(new DiscountManufacturerMapping { EntityId = manufacturer.Id, DiscountId = discount.Id });

                await _manufacturerService.UpdateManufacturer(manufacturer);
            }

            ViewBag.RefreshPage = true;

            return View(new AddManufacturerToDiscountSearchModel());
        }

        #endregion

        #region Discount usage history

        [HttpPost]
        public virtual async Task<IActionResult> UsageHistoryList(DiscountUsageHistorySearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedDataTablesJson();

            //try to get a discount with the specified id
            var discount = await _discountService.GetDiscountById(searchModel.DiscountId)
                ?? throw new ArgumentException("No discount found with the specified id");

            //prepare model
            var model = await _discountModelFactory.PrepareDiscountUsageHistoryListModel(searchModel, discount);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> UsageHistoryDelete(int discountId, int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //try to get a discount with the specified id
            _ = await _discountService.GetDiscountById(discountId)
                ?? throw new ArgumentException("No discount found with the specified id", nameof(discountId));

            //try to get a discount usage history entry with the specified id
            var discountUsageHistoryEntry = await _discountService.GetDiscountUsageHistoryById(id)
                ?? throw new ArgumentException("No discount usage history entry found with the specified id", nameof(id));

            await _discountService.DeleteDiscountUsageHistory(discountUsageHistoryEntry);

            return new NullJsonResult();
        }

        #endregion

        #endregion
    }
}