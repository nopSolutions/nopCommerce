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

namespace Nop.Web.Areas.Admin.Controllers;

public partial class DiscountController : BaseAdminController
{
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly ICategoryService _categoryService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly IDiscountModelFactory _discountModelFactory;
    protected readonly IDiscountPluginManager _discountPluginManager;
    protected readonly IDiscountService _discountService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IManufacturerService _manufacturerService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IProductService _productService;

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

    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_VIEW)]
    public virtual async Task<IActionResult> List()
    {
        //whether discounts are ignored
        if (_catalogSettings.IgnoreDiscounts)
            _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Admin.Promotions.Discounts.IgnoreDiscounts.Warning"));

        //prepare model
        var model = await _discountModelFactory.PrepareDiscountSearchModelAsync(new DiscountSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_VIEW)]
    public virtual async Task<IActionResult> List(DiscountSearchModel searchModel)
    {
        //prepare model
        var model = await _discountModelFactory.PrepareDiscountListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create()
    {
        //prepare model
        var model = await _discountModelFactory.PrepareDiscountModelAsync(new DiscountModel(), null);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create(DiscountModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var discount = model.ToEntity<Discount>();
            await _discountService.InsertDiscountAsync(discount);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewDiscount",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewDiscount"), discount.Name), discount);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.Discounts.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = discount.Id });
        }

        //prepare model
        model = await _discountModelFactory.PrepareDiscountModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
        //try to get a discount with the specified id
        var discount = await _discountService.GetDiscountByIdAsync(id);
        if (discount == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _discountModelFactory.PrepareDiscountModelAsync(null, discount);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(DiscountModel model, bool continueEditing)
    {
        //try to get a discount with the specified id
        var discount = await _discountService.GetDiscountByIdAsync(model.Id);
        if (discount == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            var prevDiscountType = discount.DiscountType;
            discount = model.ToEntity(discount);
            await _discountService.UpdateDiscountAsync(discount);

            //clean up old references (if changed) 
            if (prevDiscountType != discount.DiscountType)
            {
                switch (prevDiscountType)
                {
                    case DiscountType.AssignedToSkus:
                        await _productService.ClearDiscountProductMappingAsync(discount);
                        break;
                    case DiscountType.AssignedToCategories:
                        await _categoryService.ClearDiscountCategoryMappingAsync(discount);
                        break;
                    case DiscountType.AssignedToManufacturers:
                        await _manufacturerService.ClearDiscountManufacturerMappingAsync(discount);
                        break;
                    default:
                        break;
                }
            }

            //activity log
            await _customerActivityService.InsertActivityAsync("EditDiscount",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditDiscount"), discount.Name), discount);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.Discounts.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = discount.Id });
        }

        //prepare model
        model = await _discountModelFactory.PrepareDiscountModelAsync(model, discount, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        //try to get a discount with the specified id
        var discount = await _discountService.GetDiscountByIdAsync(id);
        if (discount == null)
            return RedirectToAction("List");

        //applied to products
        var products = await _productService.GetProductsWithAppliedDiscountAsync(discount.Id, true);

        await _discountService.DeleteDiscountAsync(discount);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteDiscount",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteDiscount"), discount.Name), discount);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.Discounts.Deleted"));

        return RedirectToAction("List");
    }

    #endregion

    #region Discount requirements

    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_VIEW)]
    public virtual async Task<IActionResult> GetDiscountRequirementConfigurationUrl(string systemName, int discountId, int? discountRequirementId)
    {
        ArgumentException.ThrowIfNullOrEmpty(systemName);

        var discountRequirementRule = await _discountPluginManager.LoadPluginBySystemNameAsync(systemName)
            ?? throw new ArgumentException("Discount requirement rule could not be loaded");

        var discount = await _discountService.GetDiscountByIdAsync(discountId)
            ?? throw new ArgumentException("Discount could not be loaded");

        var url = discountRequirementRule.GetConfigurationUrl(discount.Id, discountRequirementId);

        return Json(new { url });
    }

    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_VIEW)]
    public virtual async Task<IActionResult> GetDiscountRequirements(int discountId, int discountRequirementId,
        int? parentId, int? interactionTypeId, bool deleteRequirement)
    {
        var requirements = new List<DiscountRequirementRuleModel>();

        var discount = await _discountService.GetDiscountByIdAsync(discountId);
        if (discount == null)
            return Json(requirements);

        var discountRequirement = await _discountService.GetDiscountRequirementByIdAsync(discountRequirementId);
        if (discountRequirement != null)
        {
            //delete
            if (deleteRequirement)
            {
                await _discountService.DeleteDiscountRequirementAsync(discountRequirement, true);

                var discountRequirements = await _discountService.GetAllDiscountRequirementsAsync(discount.Id);

                //delete default group if there are no any requirements
                if (!discountRequirements.Any(requirement => requirement.ParentId.HasValue))
                {
                    foreach (var dr in discountRequirements)
                        await _discountService.DeleteDiscountRequirementAsync(dr, true);
                }
            }
            //or update the requirement
            else
            {
                var defaultGroupId = (await _discountService.GetAllDiscountRequirementsAsync(discount.Id, true)).FirstOrDefault(requirement => requirement.IsGroup)?.Id ?? 0;
                if (defaultGroupId == 0)
                {
                    //add default requirement group
                    var defaultGroup = new DiscountRequirement
                    {
                        IsGroup = true,
                        DiscountId = discount.Id,
                        InteractionType = RequirementGroupInteractionType.And,
                        DiscountRequirementRuleSystemName = await _localizationService
                            .GetResourceAsync("Admin.Promotions.Discounts.Requirements.DefaultRequirementGroup")
                    };

                    await _discountService.InsertDiscountRequirementAsync(defaultGroup);

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

                await _discountService.UpdateDiscountRequirementAsync(discountRequirement);
            }
        }

        //get current requirements
        var topLevelRequirements = (await _discountService.GetAllDiscountRequirementsAsync(discount.Id, true)).Where(requirement => requirement.IsGroup).ToList();

        //get interaction type of top-level group
        var interactionType = topLevelRequirements.FirstOrDefault()?.InteractionType;

        if (interactionType.HasValue)
        {
            requirements = (await _discountModelFactory
                .PrepareDiscountRequirementRuleModelsAsync(topLevelRequirements, discount, interactionType.Value)).ToList();
        }

        //get available groups
        var requirementGroups = (await _discountService.GetAllDiscountRequirementsAsync(discount.Id)).Where(requirement => requirement.IsGroup);

        var availableRequirementGroups = requirementGroups.Select(requirement =>
            new SelectListItem { Value = requirement.Id.ToString(), Text = requirement.DiscountRequirementRuleSystemName }).ToList();

        return Json(new { Requirements = requirements, AvailableGroups = availableRequirementGroups });
    }

    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AddNewGroup(int discountId, string name)
    {
        var discount = await _discountService.GetDiscountByIdAsync(discountId) ?? throw new ArgumentException("Discount could not be loaded");

        var defaultGroup = (await _discountService.GetAllDiscountRequirementsAsync(discount.Id, true)).FirstOrDefault(requirement => requirement.IsGroup);
        if (defaultGroup == null)
        {
            //add default requirement group
            await _discountService.InsertDiscountRequirementAsync(new DiscountRequirement
            {
                DiscountId = discount.Id,
                IsGroup = true,
                InteractionType = RequirementGroupInteractionType.And,
                DiscountRequirementRuleSystemName = await _localizationService
                    .GetResourceAsync("Admin.Promotions.Discounts.Requirements.DefaultRequirementGroup")
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

        await _discountService.InsertDiscountRequirementAsync(discountRequirementGroup);

        if (!string.IsNullOrEmpty(name))
            return Json(new { Result = true, NewRequirementId = discountRequirementGroup.Id });

        //set identifier as group name (if not specified)
        discountRequirementGroup.DiscountRequirementRuleSystemName = $"#{discountRequirementGroup.Id}";
        await _discountService.UpdateDiscountRequirementAsync(discountRequirementGroup);

        await _discountService.UpdateDiscountAsync(discount);

        return Json(new { Result = true, NewRequirementId = discountRequirementGroup.Id });
    }

    //action displaying notification (warning) to a store owner that entered coupon code already exists
    public virtual async Task<IActionResult> CouponCodeReservedWarning(int discountId, string couponCode)
    {
        if (string.IsNullOrEmpty(couponCode))
            return Json(new { Result = string.Empty });

        //check whether discount with passed coupon code exists
        var discounts = (await _discountService.GetAllDiscountsAsync(couponCode: couponCode, showHidden: true))
            .Where(discount => discount.Id != discountId);
        if (!discounts.Any())
            return Json(new { Result = string.Empty });

        var message = string.Format(await _localizationService.GetResourceAsync("Admin.Promotions.Discounts.Fields.CouponCode.Reserved"), discounts.FirstOrDefault().Name);
        return Json(new { Result = message });
    }

    #endregion

    #region Applied to products

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_VIEW)]
    public virtual async Task<IActionResult> ProductList(DiscountProductSearchModel searchModel)
    {
        //try to get a discount with the specified id
        var discount = await _discountService.GetDiscountByIdAsync(searchModel.DiscountId)
            ?? throw new ArgumentException("No discount found with the specified id");

        //prepare model
        var model = await _discountModelFactory.PrepareDiscountProductListModelAsync(searchModel, discount);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductDelete(int discountId, int productId)
    {
        //try to get a discount with the specified id
        var discount = await _discountService.GetDiscountByIdAsync(discountId)
            ?? throw new ArgumentException("No discount found with the specified id", nameof(discountId));

        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(productId)
            ?? throw new ArgumentException("No product found with the specified id", nameof(productId));

        //remove discount
        if (await _productService.GetDiscountAppliedToProductAsync(product.Id, discount.Id) is DiscountProductMapping discountProductMapping)
            await _productService.DeleteDiscountProductMappingAsync(discountProductMapping);

        await _productService.UpdateProductAsync(product);

        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAddPopup(int discountId)
    {
        //prepare model
        var model = await _discountModelFactory.PrepareAddProductToDiscountSearchModelAsync(new AddProductToDiscountSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAddPopupList(AddProductToDiscountSearchModel searchModel)
    {
        //prepare model
        var model = await _discountModelFactory.PrepareAddProductToDiscountListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAddPopup(AddProductToDiscountModel model)
    {
        //try to get a discount with the specified id
        var discount = await _discountService.GetDiscountByIdAsync(model.DiscountId)
            ?? throw new ArgumentException("No discount found with the specified id");

        var selectedProducts = await _productService.GetProductsByIdsAsync(model.SelectedProductIds.ToArray());
        if (selectedProducts.Any())
        {
            foreach (var product in selectedProducts)
            {
                if (await _productService.GetDiscountAppliedToProductAsync(product.Id, discount.Id) is null)
                    await _productService.InsertDiscountProductMappingAsync(new DiscountProductMapping { EntityId = product.Id, DiscountId = discount.Id });

                await _productService.UpdateProductAsync(product);
            }
        }

        ViewBag.RefreshPage = true;

        return View(new AddProductToDiscountSearchModel());
    }

    #endregion

    #region Applied to categories

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_VIEW)]
    public virtual async Task<IActionResult> CategoryList(DiscountCategorySearchModel searchModel)
    {
        //try to get a discount with the specified id
        var discount = await _discountService.GetDiscountByIdAsync(searchModel.DiscountId)
            ?? throw new ArgumentException("No discount found with the specified id");

        //prepare model
        var model = await _discountModelFactory.PrepareDiscountCategoryListModelAsync(searchModel, discount);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CategoryDelete(int discountId, int categoryId)
    {
        //try to get a discount with the specified id
        var discount = await _discountService.GetDiscountByIdAsync(discountId)
            ?? throw new ArgumentException("No discount found with the specified id", nameof(discountId));

        //try to get a category with the specified id
        var category = await _categoryService.GetCategoryByIdAsync(categoryId)
            ?? throw new ArgumentException("No category found with the specified id", nameof(categoryId));

        //remove discount
        if (await _categoryService.GetDiscountAppliedToCategoryAsync(category.Id, discount.Id) is DiscountCategoryMapping mapping)
            await _categoryService.DeleteDiscountCategoryMappingAsync(mapping);

        await _categoryService.UpdateCategoryAsync(category);

        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CategoryAddPopup(int discountId)
    {
        //prepare model
        var model = await _discountModelFactory.PrepareAddCategoryToDiscountSearchModelAsync(new AddCategoryToDiscountSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CategoryAddPopupList(AddCategoryToDiscountSearchModel searchModel)
    {
        //prepare model
        var model = await _discountModelFactory.PrepareAddCategoryToDiscountListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CategoryAddPopup(AddCategoryToDiscountModel model)
    {
        //try to get a discount with the specified id
        var discount = await _discountService.GetDiscountByIdAsync(model.DiscountId)
            ?? throw new ArgumentException("No discount found with the specified id");

        foreach (var id in model.SelectedCategoryIds)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                continue;

            if (await _categoryService.GetDiscountAppliedToCategoryAsync(category.Id, discount.Id) is null)
                await _categoryService.InsertDiscountCategoryMappingAsync(new DiscountCategoryMapping { DiscountId = discount.Id, EntityId = category.Id });

            await _categoryService.UpdateCategoryAsync(category);
        }

        ViewBag.RefreshPage = true;

        return View(new AddCategoryToDiscountSearchModel());
    }

    #endregion

    #region Applied to manufacturers

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_VIEW)]
    public virtual async Task<IActionResult> ManufacturerList(DiscountManufacturerSearchModel searchModel)
    {
        //try to get a discount with the specified id
        var discount = await _discountService.GetDiscountByIdAsync(searchModel.DiscountId)
            ?? throw new ArgumentException("No discount found with the specified id");

        //prepare model
        var model = await _discountModelFactory.PrepareDiscountManufacturerListModelAsync(searchModel, discount);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ManufacturerDelete(int discountId, int manufacturerId)
    {
        //try to get a discount with the specified id
        var discount = await _discountService.GetDiscountByIdAsync(discountId)
            ?? throw new ArgumentException("No discount found with the specified id", nameof(discountId));

        //try to get a manufacturer with the specified id
        var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(manufacturerId)
            ?? throw new ArgumentException("No manufacturer found with the specified id", nameof(manufacturerId));

        //remove discount
        if (await _manufacturerService.GetDiscountAppliedToManufacturerAsync(manufacturer.Id, discount.Id) is DiscountManufacturerMapping discountManufacturerMapping)
            await _manufacturerService.DeleteDiscountManufacturerMappingAsync(discountManufacturerMapping);

        await _manufacturerService.UpdateManufacturerAsync(manufacturer);

        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ManufacturerAddPopup(int discountId)
    {
        //prepare model
        var model = await _discountModelFactory.PrepareAddManufacturerToDiscountSearchModelAsync(new AddManufacturerToDiscountSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ManufacturerAddPopupList(AddManufacturerToDiscountSearchModel searchModel)
    {
        //prepare model
        var model = await _discountModelFactory.PrepareAddManufacturerToDiscountListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ManufacturerAddPopup(AddManufacturerToDiscountModel model)
    {
        //try to get a discount with the specified id
        var discount = await _discountService.GetDiscountByIdAsync(model.DiscountId)
            ?? throw new ArgumentException("No discount found with the specified id");

        foreach (var id in model.SelectedManufacturerIds)
        {
            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(id);
            if (manufacturer == null)
                continue;

            if (await _manufacturerService.GetDiscountAppliedToManufacturerAsync(manufacturer.Id, discount.Id) is null)
                await _manufacturerService.InsertDiscountManufacturerMappingAsync(new DiscountManufacturerMapping { EntityId = manufacturer.Id, DiscountId = discount.Id });

            await _manufacturerService.UpdateManufacturerAsync(manufacturer);
        }

        ViewBag.RefreshPage = true;

        return View(new AddManufacturerToDiscountSearchModel());
    }

    #endregion

    #region Discount usage history

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_VIEW)]
    public virtual async Task<IActionResult> UsageHistoryList(DiscountUsageHistorySearchModel searchModel)
    {
        //try to get a discount with the specified id
        var discount = await _discountService.GetDiscountByIdAsync(searchModel.DiscountId)
            ?? throw new ArgumentException("No discount found with the specified id");

        //prepare model
        var model = await _discountModelFactory.PrepareDiscountUsageHistoryListModelAsync(searchModel, discount);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> UsageHistoryDelete(int discountId, int id)
    {
        //try to get a discount with the specified id
        _ = await _discountService.GetDiscountByIdAsync(discountId)
            ?? throw new ArgumentException("No discount found with the specified id", nameof(discountId));

        //try to get a discount usage history entry with the specified id
        var discountUsageHistoryEntry = await _discountService.GetDiscountUsageHistoryByIdAsync(id)
            ?? throw new ArgumentException("No discount usage history entry found with the specified id", nameof(id));

        await _discountService.DeleteDiscountUsageHistoryAsync(discountUsageHistoryEntry);

        return new NullJsonResult();
    }

    #endregion

    #endregion
}