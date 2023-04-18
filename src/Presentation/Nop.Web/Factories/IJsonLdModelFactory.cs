using Nop.Web.Models.Catalog;
using Nop.Web.Models.JsonLD;

namespace Nop.Web.Factories
{
    public partial interface IJsonLdModelFactory
    {
        /// <summary>
        /// Prepare category breadcrumb JsonLD
        /// </summary>
        /// <param name="categoryBreadcrumb">List CategorySimpleModel</param>
        /// <returns>A task that represents the asynchronous operation
        /// The task result JsonLD breadbrumb list
        /// </returns>
        Task<JsonLdBreadcrumbListModel> PrepareJsonLdBreadcrumbCategoryAsync(IList<CategorySimpleModel> categoryBreadcrumb);

        /// <summary>
        /// Prepare product breadcrumb JsonLD
        /// </summary>
        /// <param name="breadcrumbModel">Product breadcrumb model</param>
        /// <returns>A task that represents the asynchronous operation
        /// The task result JsonLD breadcrumb list
        /// </returns>
        Task<JsonLdBreadcrumbListModel> PrepareJsonLdBreadcrumbProductAsync(ProductDetailsModel.ProductBreadcrumbModel breadcrumbModel);

        // <summary>
        /// Prepare JsonLD product
        /// </summary>
        /// <param name="model">Product details model</param>
        /// <returns>A task that represents the asynchronous operation
        /// The task result JsonLD product
        /// </returns>
        Task<JsonLdProductModel> PrepareJsonLdProductAsync(ProductDetailsModel model);
    }
}
