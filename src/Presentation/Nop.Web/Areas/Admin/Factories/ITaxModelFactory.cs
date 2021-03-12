using System.Threading.Tasks;
using Nop.Web.Areas.Admin.Models.Tax;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the tax model factory
    /// </summary>
    public partial interface ITaxModelFactory
    {
        /// <summary>
        /// Prepare tax provider search model
        /// </summary>
        /// <param name="searchModel">Tax provider search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax provider search model
        /// </returns>
        Task<TaxProviderSearchModel> PrepareTaxProviderSearchModelAsync(TaxProviderSearchModel searchModel);

        /// <summary>
        /// Prepare paged tax provider list model
        /// </summary>
        /// <param name="searchModel">Tax provider search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax provider list model
        /// </returns>
        Task<TaxProviderListModel> PrepareTaxProviderListModelAsync(TaxProviderSearchModel searchModel);

        /// <summary>
        /// Prepare tax category search model
        /// </summary>
        /// <param name="searchModel">Tax category search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax category search model
        /// </returns>
        Task<TaxCategorySearchModel> PrepareTaxCategorySearchModelAsync(TaxCategorySearchModel searchModel);

        /// <summary>
        /// Prepare paged tax category list model
        /// </summary>
        /// <param name="searchModel">Tax category search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax category list model
        /// </returns>
        Task<TaxCategoryListModel> PrepareTaxCategoryListModelAsync(TaxCategorySearchModel searchModel);
    }
}