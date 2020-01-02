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
        /// <returns>Tax provider search model</returns>
        TaxProviderSearchModel PrepareTaxProviderSearchModel(TaxProviderSearchModel searchModel);

        /// <summary>
        /// Prepare paged tax provider list model
        /// </summary>
        /// <param name="searchModel">Tax provider search model</param>
        /// <returns>Tax provider list model</returns>
        TaxProviderListModel PrepareTaxProviderListModel(TaxProviderSearchModel searchModel);

        /// <summary>
        /// Prepare tax category search model
        /// </summary>
        /// <param name="searchModel">Tax category search model</param>
        /// <returns>Tax category search model</returns>
        TaxCategorySearchModel PrepareTaxCategorySearchModel(TaxCategorySearchModel searchModel);

        /// <summary>
        /// Prepare paged tax category list model
        /// </summary>
        /// <param name="searchModel">Tax category search model</param>
        /// <returns>Tax category list model</returns>
        TaxCategoryListModel PrepareTaxCategoryListModel(TaxCategorySearchModel searchModel);

        /// <summary>
        /// Prepare tax configuration model
        /// </summary>
        /// <param name="searchModel">Tax configuration model</param>
        /// <returns>Tax configuration model</returns>
        TaxConfigurationModel PrepareTaxConfigurationModel(TaxConfigurationModel searchModel);
    }
}