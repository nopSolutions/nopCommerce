using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Category service interface
    /// </summary>
    public partial interface ICategoryService
    {
        /// <summary>
        /// Delete category
        /// </summary>
        /// <param name="category">Category</param>
        void DeleteCategory(Category category);

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        IList<Category> GetAllCategories(bool showHidden = false);

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        IPagedList<Category> GetAllCategories(int pageIndex, int pageSize, bool showHidden = false);

        /// <summary>
        /// Gets all categories filtered by parent category identifier
        /// </summary>
        /// <param name="parentCategoryId">Parent category identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Category collection</returns>
        IList<Category> GetAllCategoriesByParentCategoryId(int parentCategoryId,
            bool showHidden = false);
        
        /// <summary>
        /// Gets all categories displayed on the home page
        /// </summary>
        /// <returns>Categories</returns>
        IList<Category> GetAllCategoriesDisplayedOnHomePage();
                
        /// <summary>
        /// Gets a category
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Category</returns>
        Category GetCategoryById(int categoryId);

        /// <summary>
        /// Inserts category
        /// </summary>
        /// <param name="category">Category</param>
        void InsertCategory(Category category);

        /// <summary>
        /// Updates the category
        /// </summary>
        /// <param name="category">Category</param>
        void UpdateCategory(Category category);

        /// <summary>
        /// Deletes a product category mapping
        /// </summary>
        /// <param name="productCategory">Product category</param>
        void DeleteProductCategory(ProductCategory productCategory);

        /// <summary>
        /// Gets product category mapping collection
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product a category mapping collection</returns>
        IList<ProductCategory> GetProductCategoriesByCategoryId(int categoryId, bool showHidden = false);

        /// <summary>
        /// Gets a product category mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product category mapping collection</returns>
        IList<ProductCategory> GetProductCategoriesByProductId(int productId, bool showHidden = false);

        /// <summary>
        /// Get a total number of featured products by category identifer
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Number of featured products</returns>
        int GetTotalNumberOfFeaturedProducts(int categoryId);

        /// <summary>
        /// Gets a product category mapping 
        /// </summary>
        /// <param name="productCategoryId">Product category mapping identifier</param>
        /// <returns>Product category mapping</returns>
        ProductCategory GetProductCategoryById(int productCategoryId);

        /// <summary>
        /// Inserts a product category mapping
        /// </summary>
        /// <param name="productCategory">>Product category mapping</param>
        void InsertProductCategory(ProductCategory productCategory);

        /// <summary>
        /// Updates the product category mapping 
        /// </summary>
        /// <param name="productCategory">>Product category mapping</param>
        void UpdateProductCategory(ProductCategory productCategory);
    }
}
