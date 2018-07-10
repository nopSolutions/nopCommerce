using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Discounts;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the discount model factory implementation
    /// </summary>
    public partial class DiscountModelFactory : IDiscountModelFactory
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICategoryService _categoryService;
        private readonly ICurrencyService _currencyService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IDiscountService _discountService;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IOrderService _orderService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductService _productService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public DiscountModelFactory(CurrencySettings currencySettings,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICategoryService categoryService,
            ICurrencyService currencyService,
            IDateTimeHelper dateTimeHelper,
            IDiscountService discountService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            IOrderService orderService,
            IPriceFormatter priceFormatter,
            IProductService productService,
            IWebHelper webHelper)
        {
            this._currencySettings = currencySettings;
            this._baseAdminModelFactory = baseAdminModelFactory;
            this._categoryService = categoryService;
            this._currencyService = currencyService;
            this._dateTimeHelper = dateTimeHelper;
            this._discountService = discountService;
            this._localizationService = localizationService;
            this._manufacturerService = manufacturerService;
            this._orderService = orderService;
            this._priceFormatter = priceFormatter;
            this._productService = productService;
            this._webHelper = webHelper;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare discount usage history search model
        /// </summary>
        /// <param name="searchModel">Discount usage history search model</param>
        /// <param name="discount">Discount</param>
        /// <returns>Discount usage history search model</returns>
        protected virtual DiscountUsageHistorySearchModel PrepareDiscountUsageHistorySearchModel(DiscountUsageHistorySearchModel searchModel,
            Discount discount)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            searchModel.DiscountId = discount.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare discount product search model
        /// </summary>
        /// <param name="searchModel">Discount product search model</param>
        /// <param name="discount">Discount</param>
        /// <returns>Discount product search model</returns>
        protected virtual DiscountProductSearchModel PrepareDiscountProductSearchModel(DiscountProductSearchModel searchModel, Discount discount)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            searchModel.DiscountId = discount.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare discount category search model
        /// </summary>
        /// <param name="searchModel">Discount category search model</param>
        /// <param name="discount">Discount</param>
        /// <returns>Discount category search model</returns>
        protected virtual DiscountCategorySearchModel PrepareDiscountCategorySearchModel(DiscountCategorySearchModel searchModel, Discount discount)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            searchModel.DiscountId = discount.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare discount manufacturer search model
        /// </summary>
        /// <param name="searchModel">Discount manufacturer search model</param>
        /// <param name="discount">Discount</param>
        /// <returns>Discount manufacturer search model</returns>
        protected virtual DiscountManufacturerSearchModel PrepareDiscountManufacturerSearchModel(DiscountManufacturerSearchModel searchModel,
            Discount discount)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            searchModel.DiscountId = discount.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare discount search model
        /// </summary>
        /// <param name="searchModel">Discount search model</param>
        /// <returns>Discount search model</returns>
        public virtual DiscountSearchModel PrepareDiscountSearchModel(DiscountSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available discount types
            _baseAdminModelFactory.PrepareDiscountTypes(searchModel.AvailableDiscountTypes);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged discount list model
        /// </summary>
        /// <param name="searchModel">Discount search model</param>
        /// <returns>Discount list model</returns>
        public virtual DiscountListModel PrepareDiscountListModel(DiscountSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter discounts
            var discountType = searchModel.SearchDiscountTypeId > 0 ? (DiscountType?)searchModel.SearchDiscountTypeId : null;
            var startDateUtc = searchModel.SearchStartDate.HasValue ?
                (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchStartDate.Value, _dateTimeHelper.CurrentTimeZone) : null;
            var endDateUtc = searchModel.SearchEndDate.HasValue ?
                (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchEndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1) : null;

            //get discounts
            var discounts = _discountService.GetAllDiscounts(showHidden: true,
                discountType: discountType,
                couponCode: searchModel.SearchDiscountCouponCode,
                discountName: searchModel.SearchDiscountName,
                startDateUtc: startDateUtc,
                endDateUtc: endDateUtc);

            //prepare list model
            var model = new DiscountListModel
            {
                Data = discounts.PaginationByRequestModel(searchModel).Select(discount =>
                {
                    //fill in model values from the entity
                    var discountModel = discount.ToModel<DiscountModel>();

                    //fill in additional values (not existing in the entity)
                    discountModel.DiscountTypeName = _localizationService.GetLocalizedEnum(discount.DiscountType);
                    discountModel.PrimaryStoreCurrencyCode = _currencyService
                        .GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId)?.CurrencyCode;
                    discountModel.TimesUsed = _discountService.GetAllDiscountUsageHistory(discount.Id, pageSize: 1).TotalCount;

                    return discountModel;
                }),
                Total = discounts.Count
            };

            return model;
        }

        /// <summary>
        /// Prepare discount model
        /// </summary>
        /// <param name="model">Discount model</param>
        /// <param name="discount">Discount</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Discount model</returns>
        public virtual DiscountModel PrepareDiscountModel(DiscountModel model, Discount discount, bool excludeProperties = false)
        {
            if (discount != null)
            {
                //fill in model values from the entity
                model = model ?? discount.ToModel<DiscountModel>();

                //prepare available discount requirement rules
                var discountRules = _discountService.LoadAllDiscountRequirementRules();
                foreach (var discountRule in discountRules)
                {
                    model.AvailableDiscountRequirementRules.Add(new SelectListItem
                    {
                        Text = discountRule.PluginDescriptor.FriendlyName,
                        Value = discountRule.PluginDescriptor.SystemName
                    });
                }

                model.AvailableDiscountRequirementRules.Insert(0, new SelectListItem
                {
                    Text = _localizationService.GetResource("Admin.Promotions.Discounts.Requirements.DiscountRequirementType.AddGroup"),
                    Value = "AddGroup"
                });

                model.AvailableDiscountRequirementRules.Insert(0, new SelectListItem
                {
                    Text = _localizationService.GetResource("Admin.Promotions.Discounts.Requirements.DiscountRequirementType.Select"),
                    Value = string.Empty
                });

                //prepare available requirement groups
                var requirementGroups = discount.DiscountRequirements.Where(requirement => requirement.IsGroup);
                model.AvailableRequirementGroups = requirementGroups.Select(requirement =>
                    new SelectListItem { Value = requirement.Id.ToString(), Text = requirement.DiscountRequirementRuleSystemName }).ToList();

                //prepare nested search models
                PrepareDiscountUsageHistorySearchModel(model.DiscountUsageHistorySearchModel, discount);
                PrepareDiscountProductSearchModel(model.DiscountProductSearchModel, discount);
                PrepareDiscountCategorySearchModel(model.DiscountCategorySearchModel, discount);
                PrepareDiscountManufacturerSearchModel(model.DiscountManufacturerSearchModel, discount);
            }

            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;

            //get URL of discount with coupon code
            if (model.RequiresCouponCode && !string.IsNullOrEmpty(model.CouponCode))
            {
                model.DiscountUrl = QueryHelpers.AddQueryString(_webHelper.GetStoreLocation().TrimEnd('/'),
                    NopDiscountDefaults.DiscountCouponQueryParameter, model.CouponCode);
            }

            //set default values for the new model
            if (discount == null)
                model.LimitationTimes = 1;

            return model;
        }

        /// <summary>
        /// Prepare discount requirement rule models
        /// </summary>
        /// <param name="requirements">Collection of discount requirements</param>
        /// <param name="discount">Discount</param>
        /// <param name="groupInteractionType">Interaction type within the group of requirements</param>
        /// <returns>List of discount requirement rule models</returns>
        public virtual IList<DiscountRequirementRuleModel> PrepareDiscountRequirementRuleModels(ICollection<DiscountRequirement> requirements,
            Discount discount, RequirementGroupInteractionType groupInteractionType)
        {
            if (requirements == null)
                throw new ArgumentNullException(nameof(requirements));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            var lastRequirement = requirements.LastOrDefault();

            return requirements.Select(requirement =>
            {
                //set common properties
                var requirementModel = new DiscountRequirementRuleModel
                {
                    DiscountRequirementId = requirement.Id,
                    ParentId = requirement.ParentId,
                    IsGroup = requirement.IsGroup,
                    RuleName = requirement.DiscountRequirementRuleSystemName,
                    IsLastInGroup = lastRequirement == null || lastRequirement.Id == requirement.Id,
                    InteractionTypeId = (int)groupInteractionType
                };

                var interactionType = requirement.InteractionType ?? RequirementGroupInteractionType.And;
                requirementModel.AvailableInteractionTypes = interactionType.ToSelectList();

                if (requirement.IsGroup)
                {
                    //get child requirements for the group
                    requirementModel
                        .ChildRequirements = PrepareDiscountRequirementRuleModels(requirement.ChildRequirements, discount, interactionType);

                    return requirementModel;
                }

                //or try to get name and configuration URL for the requirement
                var requirementRule = _discountService.LoadDiscountRequirementRuleBySystemName(requirement.DiscountRequirementRuleSystemName);
                if (requirementRule == null)
                    return null;

                requirementModel.RuleName = requirementRule.PluginDescriptor.FriendlyName;
                requirementModel
                    .ConfigurationUrl = $"{_webHelper.GetStoreLocation()}{requirementRule.GetConfigurationUrl(discount.Id, requirement.Id)}";

                return requirementModel;
            }).ToList();
        }

        /// <summary>
        /// Prepare paged discount usage history list model
        /// </summary>
        /// <param name="searchModel">Discount usage history search model</param>
        /// <param name="discount">Discount</param>
        /// <returns>Discount usage history list model</returns>
        public virtual DiscountUsageHistoryListModel PrepareDiscountUsageHistoryListModel(DiscountUsageHistorySearchModel searchModel,
            Discount discount)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            //get discount usage history
            var history = _discountService.GetAllDiscountUsageHistory(discountId: discount.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new DiscountUsageHistoryListModel
            {
                Data = history.Select(historyEntry =>
                {
                    //fill in model values from the entity
                    var discountUsageHistoryModel = new DiscountUsageHistoryModel
                    {
                        Id = historyEntry.Id,
                        DiscountId = historyEntry.DiscountId,
                        OrderId = historyEntry.OrderId
                    };

                    //convert dates to the user time
                    discountUsageHistoryModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(historyEntry.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    var order = _orderService.GetOrderById(historyEntry.OrderId);
                    if (order != null)
                    {
                        discountUsageHistoryModel.OrderTotal = _priceFormatter.FormatPrice(order.OrderTotal, true, false);
                        discountUsageHistoryModel.CustomOrderNumber = order.CustomOrderNumber;
                    }

                    return discountUsageHistoryModel;
                }),
                Total = history.TotalCount
            };

            return model;
        }

        /// <summary>
        /// Prepare paged discount product list model
        /// </summary>
        /// <param name="searchModel">Discount product search model</param>
        /// <param name="discount">Discount</param>
        /// <returns>Discount product list model</returns>
        public virtual DiscountProductListModel PrepareDiscountProductListModel(DiscountProductSearchModel searchModel, Discount discount)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            //get products with applied discount
            var discountProducts = _discountService.GetProductsWithAppliedDiscount(discountId: discount.Id,
                showHidden: false,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new DiscountProductListModel
            {
                //fill in model values from the entity
                Data = discountProducts.Select(product => new DiscountProductModel
                {
                    ProductId = product.Id,
                    ProductName = product.Name
                }),
                Total = discountProducts.TotalCount
            };

            return model;
        }

        /// <summary>
        /// Prepare product search model to add to the discount
        /// </summary>
        /// <param name="searchModel">Product search model to add to the discount</param>
        /// <returns>Product search model to add to the discount</returns>
        public virtual AddProductToDiscountSearchModel PrepareAddProductToDiscountSearchModel(AddProductToDiscountSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available categories
            _baseAdminModelFactory.PrepareCategories(searchModel.AvailableCategories);

            //prepare available manufacturers
            _baseAdminModelFactory.PrepareManufacturers(searchModel.AvailableManufacturers);

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

            //prepare available vendors
            _baseAdminModelFactory.PrepareVendors(searchModel.AvailableVendors);

            //prepare available product types
            _baseAdminModelFactory.PrepareProductTypes(searchModel.AvailableProductTypes);

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged product list model to add to the discount
        /// </summary>
        /// <param name="searchModel">Product search model to add to the discount</param>
        /// <returns>Product list model to add to the discount</returns>
        public virtual AddProductToDiscountListModel PrepareAddProductToDiscountListModel(AddProductToDiscountSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get products
            var products = _productService.SearchProducts(showHidden: true,
                categoryIds: new List<int> { searchModel.SearchCategoryId },
                manufacturerId: searchModel.SearchManufacturerId,
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
                keywords: searchModel.SearchProductName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new AddProductToDiscountListModel
            {
                //fill in model values from the entity
                Data = products.Select(product => product.ToModel<ProductModel>()),
                Total = products.TotalCount
            };

            return model;
        }

        /// <summary>
        /// Prepare paged discount category list model
        /// </summary>
        /// <param name="searchModel">Discount category search model</param>
        /// <param name="discount">Discount</param>
        /// <returns>Discount category list model</returns>
        public virtual DiscountCategoryListModel PrepareDiscountCategoryListModel(DiscountCategorySearchModel searchModel, Discount discount)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            //get categories with applied discount
            var discountCategories = _discountService.GetCategoriesWithAppliedDiscount(discountId: discount.Id,
                showHidden: false,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new DiscountCategoryListModel
            {
                //fill in model values from the entity
                Data = discountCategories.Select(category => new DiscountCategoryModel
                {
                    CategoryId = category.Id,
                    CategoryName = _categoryService.GetFormattedBreadCrumb(category)
                }),
                Total = discountCategories.TotalCount
            };

            return model;
        }

        /// <summary>
        /// Prepare category search model to add to the discount
        /// </summary>
        /// <param name="searchModel">Category search model to add to the discount</param>
        /// <returns>Category search model to add to the discount</returns>
        public virtual AddCategoryToDiscountSearchModel PrepareAddCategoryToDiscountSearchModel(AddCategoryToDiscountSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged category list model to add to the discount
        /// </summary>
        /// <param name="searchModel">Category search model to add to the discount</param>
        /// <returns>Category list model to add to the discount</returns>
        public virtual AddCategoryToDiscountListModel PrepareAddCategoryToDiscountListModel(AddCategoryToDiscountSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get categories
            var categories = _categoryService.GetAllCategories(showHidden: true,
                categoryName: searchModel.SearchCategoryName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new AddCategoryToDiscountListModel
            {
                Data = categories.Select(category =>
                {
                    //fill in model values from the entity
                    var categoryModel = category.ToModel<CategoryModel>();

                    //fill in additional values (not existing in the entity)
                    categoryModel.Breadcrumb = _categoryService.GetFormattedBreadCrumb(category);

                    return categoryModel;
                }),
                Total = categories.TotalCount
            };

            return model;
        }

        /// <summary>
        /// Prepare paged discount manufacturer list model
        /// </summary>
        /// <param name="searchModel">Discount manufacturer search model</param>
        /// <param name="discount">Discount</param>
        /// <returns>Discount manufacturer list model</returns>
        public virtual DiscountManufacturerListModel PrepareDiscountManufacturerListModel(DiscountManufacturerSearchModel searchModel,
            Discount discount)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            //get manufacturers with applied discount
            var discountManufacturers = _discountService.GetManufacturersWithAppliedDiscount(discountId: discount.Id,
                showHidden: false,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new DiscountManufacturerListModel
            {
                //fill in model values from the entity
                Data = discountManufacturers.Select(manufacturer => new DiscountManufacturerModel
                {
                    ManufacturerId = manufacturer.Id,
                    ManufacturerName = manufacturer.Name
                }),
                Total = discountManufacturers.TotalCount
            };

            return model;
        }

        /// <summary>
        /// Prepare manufacturer search model to add to the discount
        /// </summary>
        /// <param name="searchModel">Manufacturer search model to add to the discount</param>
        /// <returns>Manufacturer search model to add to the discount</returns>
        public virtual AddManufacturerToDiscountSearchModel PrepareAddManufacturerToDiscountSearchModel(AddManufacturerToDiscountSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged manufacturer list model to add to the discount
        /// </summary>
        /// <param name="searchModel">Manufacturer search model to add to the discount</param>
        /// <returns>Manufacturer list model to add to the discount</returns>
        public virtual AddManufacturerToDiscountListModel PrepareAddManufacturerToDiscountListModel(AddManufacturerToDiscountSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get manufacturers
            var manufacturers = _manufacturerService.GetAllManufacturers(showHidden: true,
                manufacturerName: searchModel.SearchManufacturerName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new AddManufacturerToDiscountListModel
            {
                //fill in model values from the entity
                Data = manufacturers.Select(manufacturer => manufacturer.ToModel<ManufacturerModel>()),
                Total = manufacturers.TotalCount
            };

            return model;
        }

        #endregion
    }
}