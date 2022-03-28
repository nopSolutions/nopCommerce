using Nop.Core.Domain.Catalog;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcCore.Services
{
    public interface IImportUtilities
    {
        /// <summary>
        ///		Create and insert an HTML widget
        ///		along with all the other needed entities.
        ///		It will have the provided name and HTML.
        ///		It will display in the provided widget zone
        ///		with the given display order.
        /// </summary>
        /// <returns>
        ///		The ID of the inserted HTML widget.
        /// </returns>
        Task<int> InsertHtmlWidgetAsync(
            string name, string html, string widgetZone, int displayOrder);

        /// <summary>
        ///		Add a product override onto the widget with the given ID
        ///		for the product with the given ID.
        ///		This will allow the widget to appear on the product's page.
        /// </summary>
        /// <param name="widgetId">
        ///		The ID of the widget to which you want to add a product.
        /// </param>
        /// <param name="productId">
        ///		The ID of product for which you want the widget to display.
        /// </param>
        Task AddProductToHtmlWidgetAsync(int widgetId, int productId);

        /// <summary>
        ///		Create and insert a quick tab
        ///		along with all the other needed entities.
        ///		It will have the provided system name, display name, and HTML.
        ///		It will display ONLY for a single product whose ID is given.
        /// </summary>
        Task InsertQuickTabForSpecificProductAsync(
            string systemName, string displayName, string html, int productId);

        /// <summary>
        /// returns the product corresponding to sku, will return deleted products
        /// </summary>
        /// <param name="sku"></param>
        Product GetExistingProductBySku(string sku);

        /// <summary>
        /// performs a partial clone of original covering only core fields changed during import. 
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        Product CoreClone(Product original);

        /// <summary>
        /// returns true if the core fields of the two products are equal
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        bool CoreEquals(Product p1, Product p2);

        Task<SpecificationAttribute> GetCategorySpecificationAttributeAsync();

        Task InsertProductAttributeMappingAsync(
            int productId,
            int attributeId,
            EntityManager<ProductAttributeMapping> attributeManager);

        Task<ProductAttribute> GetPickupAttributeAsync();

        Task<ProductAttribute> GetHomeDeliveryAttributeAsync();

        Task<PredefinedProductAttributeValue> GetHomeDeliveryAttributeValueAsync();
    }
}