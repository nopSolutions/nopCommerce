using System.Collections.Generic;
using System.Threading.Tasks;
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the discount search model
        /// </returns>
        Task<DiscountSearchModel> PrepareDiscountSearchModelAsync(DiscountSearchModel searchModel);

        /// <summary>
        /// Prepare paged discount list model
        /// </summary>
        /// <param name="searchModel">Discount search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the discount list model
        /// </returns>
        Task<DiscountListModel> PrepareDiscountListModelAsync(DiscountSearchModel searchModel);

        /// <summary>
        /// Prepare discount model
        /// </summary>
        /// <param name="model">Discount model</param>
        /// <param name="discount">Discount</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the discount model
        /// </returns>
        Task<DiscountModel> PrepareDiscountModelAsync(DiscountModel model, Discount discount, bool excludeProperties = false);

        /// <summary>
        /// Prepare discount requirement rule models
        /// </summary>
        /// <param name="requirements">Collection of discount requirements</param>
        /// <param name="discount">Discount</param>
        /// <param name="groupInteractionType">Interaction type within the group of requirements</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of discount requirement rule models
        /// </returns>
        Task<IList<DiscountRequirementRuleModel>> PrepareDiscountRequirementRuleModelsAsync(ICollection<DiscountRequirement> requirements,
            Discount discount, RequirementGroupInteractionType groupInteractionType);

        /// <summary>
        /// Prepare paged discount usage history list model
        /// </summary>
        /// <param name="searchModel">Discount usage history search model</param>
        /// <param name="discount">Discount</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the discount usage history list model
        /// </returns>
        Task<DiscountUsageHistoryListModel> PrepareDiscountUsageHistoryListModelAsync(DiscountUsageHistorySearchModel searchModel,
            Discount discount);

        /// <summary>
        /// Prepare paged discount product list model
        /// </summary>
        /// <param name="searchModel">Discount product search model</param>
        /// <param name="discount">Discount</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the discount product list model
        /// </returns>
        Task<DiscountProductListModel> PrepareDiscountProductListModelAsync(DiscountProductSearchModel searchModel, Discount discount);

        /// <summary>
        /// Prepare product search model to add to the discount
        /// </summary>
        /// <param name="searchModel">Product search model to add to the discount</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product search model to add to the discount
        /// </returns>
        Task<AddProductToDiscountSearchModel> PrepareAddProductToDiscountSearchModelAsync(AddProductToDiscountSearchModel searchModel);

        /// <summary>
        /// Prepare paged product list model to add to the discount
        /// </summary>
        /// <param name="searchModel">Product search model to add to the discount</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product list model to add to the discount
        /// </returns>
        Task<AddProductToDiscountListModel> PrepareAddProductToDiscountListModelAsync(AddProductToDiscountSearchModel searchModel);

        /// <summary>
        /// Prepare paged discount category list model
        /// </summary>
        /// <param name="searchModel">Discount category search model</param>
        /// <param name="discount">Discount</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the discount category list model
        /// </returns>
        Task<DiscountCategoryListModel> PrepareDiscountCategoryListModelAsync(DiscountCategorySearchModel searchModel, Discount discount);

        /// <summary>
        /// Prepare category search model to add to the discount
        /// </summary>
        /// <param name="searchModel">Category search model to add to the discount</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category search model to add to the discount
        /// </returns>
        Task<AddCategoryToDiscountSearchModel> PrepareAddCategoryToDiscountSearchModelAsync(AddCategoryToDiscountSearchModel searchModel);

        /// <summary>
        /// Prepare paged category list model to add to the discount
        /// </summary>
        /// <param name="searchModel">Category search model to add to the discount</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category list model to add to the discount
        /// </returns>
        Task<AddCategoryToDiscountListModel> PrepareAddCategoryToDiscountListModelAsync(AddCategoryToDiscountSearchModel searchModel);

        /// <summary>
        /// Prepare paged discount manufacturer list model
        /// </summary>
        /// <param name="searchModel">Discount manufacturer search model</param>
        /// <param name="discount">Discount</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the discount manufacturer list model
        /// </returns>
        Task<DiscountManufacturerListModel> PrepareDiscountManufacturerListModelAsync(DiscountManufacturerSearchModel searchModel,
            Discount discount);

        /// <summary>
        /// Prepare manufacturer search model to add to the discount
        /// </summary>
        /// <param name="searchModel">Manufacturer search model to add to the discount</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the manufacturer search model to add to the discount
        /// </returns>
        Task<AddManufacturerToDiscountSearchModel> PrepareAddManufacturerToDiscountSearchModelAsync(AddManufacturerToDiscountSearchModel searchModel);

        /// <summary>
        /// Prepare paged manufacturer list model to add to the discount
        /// </summary>
        /// <param name="searchModel">Manufacturer search model to add to the discount</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the manufacturer list model to add to the discount
        /// </returns>
        Task<AddManufacturerToDiscountListModel> PrepareAddManufacturerToDiscountListModelAsync(AddManufacturerToDiscountSearchModel searchModel);
    }
}