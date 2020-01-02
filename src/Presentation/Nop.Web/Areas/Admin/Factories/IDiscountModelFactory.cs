using System.Collections.Generic;
using Nop.Core.Domain.Discounts;
using Nop.Web.Areas.Admin.Models.Discounts;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the discount model factory
    /// </summary>
    public partial interface IDiscountModelFactory
    {
        /// <summary>
        /// Prepare discount search model
        /// </summary>
        /// <param name="searchModel">Discount search model</param>
        /// <returns>Discount search model</returns>
        DiscountSearchModel PrepareDiscountSearchModel(DiscountSearchModel searchModel);

        /// <summary>
        /// Prepare paged discount list model
        /// </summary>
        /// <param name="searchModel">Discount search model</param>
        /// <returns>Discount list model</returns>
        DiscountListModel PrepareDiscountListModel(DiscountSearchModel searchModel);

        /// <summary>
        /// Prepare discount model
        /// </summary>
        /// <param name="model">Discount model</param>
        /// <param name="discount">Discount</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Discount model</returns>
        DiscountModel PrepareDiscountModel(DiscountModel model, Discount discount, bool excludeProperties = false);

        /// <summary>
        /// Prepare discount requirement rule models
        /// </summary>
        /// <param name="requirements">Collection of discount requirements</param>
        /// <param name="discount">Discount</param>
        /// <param name="groupInteractionType">Interaction type within the group of requirements</param>
        /// <returns>List of discount requirement rule models</returns>
        IList<DiscountRequirementRuleModel> PrepareDiscountRequirementRuleModels(ICollection<DiscountRequirement> requirements,
            Discount discount, RequirementGroupInteractionType groupInteractionType);

        /// <summary>
        /// Prepare paged discount usage history list model
        /// </summary>
        /// <param name="searchModel">Discount usage history search model</param>
        /// <param name="discount">Discount</param>
        /// <returns>Discount usage history list model</returns>
        DiscountUsageHistoryListModel PrepareDiscountUsageHistoryListModel(DiscountUsageHistorySearchModel searchModel,
            Discount discount);

        /// <summary>
        /// Prepare paged discount product list model
        /// </summary>
        /// <param name="searchModel">Discount product search model</param>
        /// <param name="discount">Discount</param>
        /// <returns>Discount product list model</returns>
        DiscountProductListModel PrepareDiscountProductListModel(DiscountProductSearchModel searchModel, Discount discount);

        /// <summary>
        /// Prepare product search model to add to the discount
        /// </summary>
        /// <param name="searchModel">Product search model to add to the discount</param>
        /// <returns>Product search model to add to the discount</returns>
        AddProductToDiscountSearchModel PrepareAddProductToDiscountSearchModel(AddProductToDiscountSearchModel searchModel);

        /// <summary>
        /// Prepare paged product list model to add to the discount
        /// </summary>
        /// <param name="searchModel">Product search model to add to the discount</param>
        /// <returns>Product list model to add to the discount</returns>
        AddProductToDiscountListModel PrepareAddProductToDiscountListModel(AddProductToDiscountSearchModel searchModel);

        /// <summary>
        /// Prepare paged discount category list model
        /// </summary>
        /// <param name="searchModel">Discount category search model</param>
        /// <param name="discount">Discount</param>
        /// <returns>Discount category list model</returns>
        DiscountCategoryListModel PrepareDiscountCategoryListModel(DiscountCategorySearchModel searchModel, Discount discount);

        /// <summary>
        /// Prepare category search model to add to the discount
        /// </summary>
        /// <param name="searchModel">Category search model to add to the discount</param>
        /// <returns>Category search model to add to the discount</returns>
        AddCategoryToDiscountSearchModel PrepareAddCategoryToDiscountSearchModel(AddCategoryToDiscountSearchModel searchModel);

        /// <summary>
        /// Prepare paged category list model to add to the discount
        /// </summary>
        /// <param name="searchModel">Category search model to add to the discount</param>
        /// <returns>Category list model to add to the discount</returns>
        AddCategoryToDiscountListModel PrepareAddCategoryToDiscountListModel(AddCategoryToDiscountSearchModel searchModel);

        /// <summary>
        /// Prepare paged discount manufacturer list model
        /// </summary>
        /// <param name="searchModel">Discount manufacturer search model</param>
        /// <param name="discount">Discount</param>
        /// <returns>Discount manufacturer list model</returns>
        DiscountManufacturerListModel PrepareDiscountManufacturerListModel(DiscountManufacturerSearchModel searchModel,
            Discount discount);

        /// <summary>
        /// Prepare manufacturer search model to add to the discount
        /// </summary>
        /// <param name="searchModel">Manufacturer search model to add to the discount</param>
        /// <returns>Manufacturer search model to add to the discount</returns>
        AddManufacturerToDiscountSearchModel PrepareAddManufacturerToDiscountSearchModel(AddManufacturerToDiscountSearchModel searchModel);

        /// <summary>
        /// Prepare paged manufacturer list model to add to the discount
        /// </summary>
        /// <param name="searchModel">Manufacturer search model to add to the discount</param>
        /// <returns>Manufacturer list model to add to the discount</returns>
        AddManufacturerToDiscountListModel PrepareAddManufacturerToDiscountListModel(AddManufacturerToDiscountSearchModel searchModel);
    }
}