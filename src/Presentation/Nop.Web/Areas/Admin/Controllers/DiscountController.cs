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

        protected CatalogSettings CatalogSettings { get; }
        protected ICategoryService CategoryService { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected IDiscountModelFactory DiscountModelFactory { get; }
        protected IDiscountPluginManager DiscountPluginManager { get; }
        protected IDiscountService DiscountService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IManufacturerService ManufacturerService { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }
        protected IProductService ProductService { get; }

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
            CatalogSettings = catalogSettings;
            CategoryService = categoryService;
            CustomerActivityService = customerActivityService;
            DiscountModelFactory = discountModelFactory;
            DiscountPluginManager = discountPluginManager;
            DiscountService = discountService;
            LocalizationService = localizationService;
            ManufacturerService = manufacturerService;
            NotificationService = notificationService;
            PermissionService = permissionService;
            ProductService = productService;
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
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //whether discounts are ignored
            if (CatalogSettings.IgnoreDiscounts)
                NotificationService.WarningNotification(await LocalizationService.GetResourceAsync("Admin.Promotions.Discounts.IgnoreDiscounts.Warning"));

            //prepare model
            var model = await DiscountModelFactory.PrepareDiscountSearchModelAsync(new DiscountSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(DiscountSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await DiscountModelFactory.PrepareDiscountListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //prepare model
            var model = await DiscountModelFactory.PrepareDiscountModelAsync(new DiscountModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(DiscountModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var discount = model.ToEntity<Discount>();
                await DiscountService.InsertDiscountAsync(discount);

                //activity log
                await CustomerActivityService.InsertActivityAsync("AddNewDiscount",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewDiscount"), discount.Name), discount);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Promotions.Discounts.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = discount.Id });
            }

            //prepare model
            model = await DiscountModelFactory.PrepareDiscountModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //try to get a discount with the specified id
            var discount = await DiscountService.GetDiscountByIdAsync(id);
            if (discount == null)
                return RedirectToAction("List");

            //prepare model
            var model = await DiscountModelFactory.PrepareDiscountModelAsync(null, discount);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(DiscountModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //try to get a discount with the specified id
            var discount = await DiscountService.GetDiscountByIdAsync(model.Id);
            if (discount == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var prevDiscountType = discount.DiscountType;
                discount = model.ToEntity(discount);
                await DiscountService.UpdateDiscountAsync(discount);

                //clean up old references (if changed) 
                if (prevDiscountType != discount.DiscountType)
                {
                    switch (prevDiscountType)
                    {
                        case DiscountType.AssignedToSkus:
                            await ProductService.ClearDiscountProductMappingAsync(discount);
                            break;
                        case DiscountType.AssignedToCategories:
                            await CategoryService.ClearDiscountCategoryMappingAsync(discount);
                            break;
                        case DiscountType.AssignedToManufacturers:
                            await ManufacturerService.ClearDiscountManufacturerMappingAsync(discount);
                            break;
                        default:
                            break;
                    }
                }

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditDiscount",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditDiscount"), discount.Name), discount);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Promotions.Discounts.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = discount.Id });
            }

            //prepare model
            model = await DiscountModelFactory.PrepareDiscountModelAsync(model, discount, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //try to get a discount with the specified id
            var discount = await DiscountService.GetDiscountByIdAsync(id);
            if (discount == null)
                return RedirectToAction("List");

            //applied to products
            var products = await ProductService.GetProductsWithAppliedDiscountAsync(discount.Id, true);

            await DiscountService.DeleteDiscountAsync(discount);

            //update "HasDiscountsApplied" properties
            foreach (var p in products)
                await ProductService.UpdateHasDiscountsAppliedAsync(p);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteDiscount",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteDiscount"), discount.Name), discount);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Promotions.Discounts.Deleted"));

            return RedirectToAction("List");
        }

        #endregion

        #region Discount requirements

        public virtual async Task<IActionResult> GetDiscountRequirementConfigurationUrl(string systemName, int discountId, int? discountRequirementId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            if (string.IsNullOrEmpty(systemName))
                throw new ArgumentNullException(nameof(systemName));

            var discountRequirementRule = await DiscountPluginManager.LoadPluginBySystemNameAsync(systemName)
                ?? throw new ArgumentException("Discount requirement rule could not be loaded");

            var discount = await DiscountService.GetDiscountByIdAsync(discountId)
                ?? throw new ArgumentException("Discount could not be loaded");

            var url = discountRequirementRule.GetConfigurationUrl(discount.Id, discountRequirementId);

            return Json(new { url });
        }

        public virtual async Task<IActionResult> GetDiscountRequirements(int discountId, int discountRequirementId,
            int? parentId, int? interactionTypeId, bool deleteRequirement)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var requirements = new List<DiscountRequirementRuleModel>();

            var discount = await DiscountService.GetDiscountByIdAsync(discountId);
            if (discount == null)
                return Json(requirements);

            var discountRequirement = await DiscountService.GetDiscountRequirementByIdAsync(discountRequirementId);
            if (discountRequirement != null)
            {
                //delete
                if (deleteRequirement)
                {
                    await DiscountService.DeleteDiscountRequirementAsync(discountRequirement, true);

                    var discountRequirements = await DiscountService.GetAllDiscountRequirementsAsync(discount.Id);

                    //delete default group if there are no any requirements
                    if (!discountRequirements.Any(requirement => requirement.ParentId.HasValue))
                    {
                        foreach (var dr in discountRequirements)
                            await DiscountService.DeleteDiscountRequirementAsync(dr, true);
                    }
                }
                //or update the requirement
                else
                {
                    var defaultGroupId = (await DiscountService.GetAllDiscountRequirementsAsync(discount.Id, true)).FirstOrDefault(requirement => requirement.IsGroup)?.Id ?? 0;
                    if (defaultGroupId == 0)
                    {
                        //add default requirement group
                        var defaultGroup = new DiscountRequirement
                        {
                            IsGroup = true,
                            DiscountId = discount.Id,
                            InteractionType = RequirementGroupInteractionType.And,
                            DiscountRequirementRuleSystemName = await LocalizationService
                                .GetResourceAsync("Admin.Promotions.Discounts.Requirements.DefaultRequirementGroup")
                        };

                        await DiscountService.InsertDiscountRequirementAsync(defaultGroup);

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

                    await DiscountService.UpdateDiscountRequirementAsync(discountRequirement);
                }
            }

            //get current requirements
            var topLevelRequirements = (await DiscountService.GetAllDiscountRequirementsAsync(discount.Id, true)).Where(requirement => requirement.IsGroup).ToList();

            //get interaction type of top-level group
            var interactionType = topLevelRequirements.FirstOrDefault()?.InteractionType;

            if (interactionType.HasValue)
            {
                requirements = (await DiscountModelFactory
                    .PrepareDiscountRequirementRuleModelsAsync(topLevelRequirements, discount, interactionType.Value)).ToList();
            }

            //get available groups
            var requirementGroups = (await DiscountService.GetAllDiscountRequirementsAsync(discount.Id)).Where(requirement => requirement.IsGroup);

            var availableRequirementGroups = requirementGroups.Select(requirement =>
                new SelectListItem { Value = requirement.Id.ToString(), Text = requirement.DiscountRequirementRuleSystemName }).ToList();

            return Json(new { Requirements = requirements, AvailableGroups = availableRequirementGroups });
        }

        public virtual async Task<IActionResult> AddNewGroup(int discountId, string name)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = await DiscountService.GetDiscountByIdAsync(discountId);
            if (discount == null)
                throw new ArgumentException("Discount could not be loaded");

            var defaultGroup = (await DiscountService.GetAllDiscountRequirementsAsync(discount.Id, true)).FirstOrDefault(requirement => requirement.IsGroup);
            if (defaultGroup == null)
            {
                //add default requirement group
                await DiscountService.InsertDiscountRequirementAsync(new DiscountRequirement
                {
                    DiscountId = discount.Id,
                    IsGroup = true,
                    InteractionType = RequirementGroupInteractionType.And,
                    DiscountRequirementRuleSystemName = await LocalizationService
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

            await DiscountService.InsertDiscountRequirementAsync(discountRequirementGroup);

            if (!string.IsNullOrEmpty(name))
                return Json(new { Result = true, NewRequirementId = discountRequirementGroup.Id });

            //set identifier as group name (if not specified)
            discountRequirementGroup.DiscountRequirementRuleSystemName = $"#{discountRequirementGroup.Id}";
            await DiscountService.UpdateDiscountRequirementAsync(discountRequirementGroup);

            await DiscountService.UpdateDiscountAsync(discount);

            return Json(new { Result = true, NewRequirementId = discountRequirementGroup.Id });
        }

        #endregion

        #region Applied to products

        [HttpPost]
        public virtual async Task<IActionResult> ProductList(DiscountProductSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return await AccessDeniedDataTablesJson();

            //try to get a discount with the specified id
            var discount = await DiscountService.GetDiscountByIdAsync(searchModel.DiscountId)
                ?? throw new ArgumentException("No discount found with the specified id");

            //prepare model
            var model = await DiscountModelFactory.PrepareDiscountProductListModelAsync(searchModel, discount);

            return Json(model);
        }

        public virtual async Task<IActionResult> ProductDelete(int discountId, int productId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //try to get a discount with the specified id
            var discount = await DiscountService.GetDiscountByIdAsync(discountId)
                ?? throw new ArgumentException("No discount found with the specified id", nameof(discountId));

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(productId)
                ?? throw new ArgumentException("No product found with the specified id", nameof(productId));

            //remove discount
            if (await ProductService.GetDiscountAppliedToProductAsync(product.Id, discount.Id) is DiscountProductMapping discountProductMapping)
                await ProductService.DeleteDiscountProductMappingAsync(discountProductMapping);

            await ProductService.UpdateProductAsync(product);
            await ProductService.UpdateHasDiscountsAppliedAsync(product);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> ProductAddPopup(int discountId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //prepare model
            var model = await DiscountModelFactory.PrepareAddProductToDiscountSearchModelAsync(new AddProductToDiscountSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductAddPopupList(AddProductToDiscountSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await DiscountModelFactory.PrepareAddProductToDiscountListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> ProductAddPopup(AddProductToDiscountModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //try to get a discount with the specified id
            var discount = await DiscountService.GetDiscountByIdAsync(model.DiscountId)
                ?? throw new ArgumentException("No discount found with the specified id");

            var selectedProducts = await ProductService.GetProductsByIdsAsync(model.SelectedProductIds.ToArray());
            if (selectedProducts.Any())
            {
                foreach (var product in selectedProducts)
                {
                    if (await ProductService.GetDiscountAppliedToProductAsync(product.Id, discount.Id) is null)
                        await ProductService.InsertDiscountProductMappingAsync(new DiscountProductMapping { EntityId = product.Id, DiscountId = discount.Id });

                    await ProductService.UpdateProductAsync(product);
                    await ProductService.UpdateHasDiscountsAppliedAsync(product);
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
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return await AccessDeniedDataTablesJson();

            //try to get a discount with the specified id
            var discount = await DiscountService.GetDiscountByIdAsync(searchModel.DiscountId)
                ?? throw new ArgumentException("No discount found with the specified id");

            //prepare model
            var model = await DiscountModelFactory.PrepareDiscountCategoryListModelAsync(searchModel, discount);

            return Json(model);
        }

        public virtual async Task<IActionResult> CategoryDelete(int discountId, int categoryId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //try to get a discount with the specified id
            var discount = await DiscountService.GetDiscountByIdAsync(discountId)
                ?? throw new ArgumentException("No discount found with the specified id", nameof(discountId));

            //try to get a category with the specified id
            var category = await CategoryService.GetCategoryByIdAsync(categoryId)
                ?? throw new ArgumentException("No category found with the specified id", nameof(categoryId));

            //remove discount
            if (await CategoryService.GetDiscountAppliedToCategoryAsync(category.Id, discount.Id) is DiscountCategoryMapping mapping)
                await CategoryService.DeleteDiscountCategoryMappingAsync(mapping);

            await CategoryService.UpdateCategoryAsync(category);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> CategoryAddPopup(int discountId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //prepare model
            var model = await DiscountModelFactory.PrepareAddCategoryToDiscountSearchModelAsync(new AddCategoryToDiscountSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CategoryAddPopupList(AddCategoryToDiscountSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await DiscountModelFactory.PrepareAddCategoryToDiscountListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> CategoryAddPopup(AddCategoryToDiscountModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //try to get a discount with the specified id
            var discount = await DiscountService.GetDiscountByIdAsync(model.DiscountId)
                ?? throw new ArgumentException("No discount found with the specified id");

            foreach (var id in model.SelectedCategoryIds)
            {
                var category = await CategoryService.GetCategoryByIdAsync(id);
                if (category == null)
                    continue;

                if (await CategoryService.GetDiscountAppliedToCategoryAsync(category.Id, discount.Id) is null)
                    await CategoryService.InsertDiscountCategoryMappingAsync(new DiscountCategoryMapping { DiscountId = discount.Id, EntityId = category.Id });

                await CategoryService.UpdateCategoryAsync(category);
            }

            ViewBag.RefreshPage = true;

            return View(new AddCategoryToDiscountSearchModel());
        }

        #endregion

        #region Applied to manufacturers

        [HttpPost]
        public virtual async Task<IActionResult> ManufacturerList(DiscountManufacturerSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return await AccessDeniedDataTablesJson();

            //try to get a discount with the specified id
            var discount = await DiscountService.GetDiscountByIdAsync(searchModel.DiscountId)
                ?? throw new ArgumentException("No discount found with the specified id");

            //prepare model
            var model = await DiscountModelFactory.PrepareDiscountManufacturerListModelAsync(searchModel, discount);

            return Json(model);
        }

        public virtual async Task<IActionResult> ManufacturerDelete(int discountId, int manufacturerId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //try to get a discount with the specified id
            var discount = await DiscountService.GetDiscountByIdAsync(discountId)
                ?? throw new ArgumentException("No discount found with the specified id", nameof(discountId));

            //try to get a manufacturer with the specified id
            var manufacturer = await ManufacturerService.GetManufacturerByIdAsync(manufacturerId)
                ?? throw new ArgumentException("No manufacturer found with the specified id", nameof(manufacturerId));

            //remove discount
            if (await ManufacturerService.GetDiscountAppliedToManufacturerAsync(manufacturer.Id, discount.Id) is DiscountManufacturerMapping discountManufacturerMapping)
                await ManufacturerService.DeleteDiscountManufacturerMappingAsync(discountManufacturerMapping);

            await ManufacturerService.UpdateManufacturerAsync(manufacturer);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> ManufacturerAddPopup(int discountId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //prepare model
            var model = await DiscountModelFactory.PrepareAddManufacturerToDiscountSearchModelAsync(new AddManufacturerToDiscountSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ManufacturerAddPopupList(AddManufacturerToDiscountSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await DiscountModelFactory.PrepareAddManufacturerToDiscountListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> ManufacturerAddPopup(AddManufacturerToDiscountModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //try to get a discount with the specified id
            var discount = await DiscountService.GetDiscountByIdAsync(model.DiscountId)
                ?? throw new ArgumentException("No discount found with the specified id");

            foreach (var id in model.SelectedManufacturerIds)
            {
                var manufacturer = await ManufacturerService.GetManufacturerByIdAsync(id);
                if (manufacturer == null)
                    continue;

                if (await ManufacturerService.GetDiscountAppliedToManufacturerAsync(manufacturer.Id, discount.Id) is null)
                    await ManufacturerService.InsertDiscountManufacturerMappingAsync(new DiscountManufacturerMapping { EntityId = manufacturer.Id, DiscountId = discount.Id });

                await ManufacturerService.UpdateManufacturerAsync(manufacturer);
            }

            ViewBag.RefreshPage = true;

            return View(new AddManufacturerToDiscountSearchModel());
        }

        #endregion

        #region Discount usage history

        [HttpPost]
        public virtual async Task<IActionResult> UsageHistoryList(DiscountUsageHistorySearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return await AccessDeniedDataTablesJson();

            //try to get a discount with the specified id
            var discount = await DiscountService.GetDiscountByIdAsync(searchModel.DiscountId)
                ?? throw new ArgumentException("No discount found with the specified id");

            //prepare model
            var model = await DiscountModelFactory.PrepareDiscountUsageHistoryListModelAsync(searchModel, discount);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> UsageHistoryDelete(int discountId, int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //try to get a discount with the specified id
            _ = await DiscountService.GetDiscountByIdAsync(discountId)
                ?? throw new ArgumentException("No discount found with the specified id", nameof(discountId));

            //try to get a discount usage history entry with the specified id
            var discountUsageHistoryEntry = await DiscountService.GetDiscountUsageHistoryByIdAsync(id)
                ?? throw new ArgumentException("No discount usage history entry found with the specified id", nameof(id));

            await DiscountService.DeleteDiscountUsageHistoryAsync(discountUsageHistoryEntry);

            return new NullJsonResult();
        }

        #endregion

        #endregion
    }
}