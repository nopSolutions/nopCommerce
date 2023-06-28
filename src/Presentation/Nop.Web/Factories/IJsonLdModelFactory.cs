using Nop.Web.Models.Catalog;
using Nop.Web.Models.JsonLD;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents JSON-LD model factory
    /// </summary>
    public partial interface IJsonLdModelFactory
    {
        /// <summary>
        /// Prepare JSON-LD category breadcrumb model
        /// </summary>
        /// <param name="categoryModels">List of category models</param>
        /// <returns>A task that represents the asynchronous operation
        /// The task result contains JSON-LD category breadcrumb model
        /// </returns>
        Task<JsonLdBreadcrumbListModel> PrepareJsonLdCategoryBreadcrumbAsync(IList<CategorySimpleModel> categoryModels);

        /// <summary>
        /// Prepare JSON-LD product breadcrumb model
        /// </summary>
        /// <param name="breadcrumbModel">Product breadcrumb model</param>
        /// <returns>A task that represents the asynchronous operation
        /// The task result contains JSON-LD product breadcrumb model
        /// </returns>
        Task<JsonLdBreadcrumbListModel> PrepareJsonLdProductBreadcrumbAsync(ProductDetailsModel.ProductBreadcrumbModel breadcrumbModel);

        /// <summary>
        /// Prepare JSON-LD product model
        /// </summary>
        /// <param name="model">Product details model</param>
        /// <param name="productUrl">Product URL</param>
        /// <returns>A task that represents the asynchronous operation
        /// The task result contains JSON-LD product model
        /// </returns>
        Task<JsonLdProductModel> PrepareJsonLdProductAsync(ProductDetailsModel model, string productUrl = null);
    }
}