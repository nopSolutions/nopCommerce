using Nop.Core.Domain.Catalog;
using Nop.Web.Areas.Admin.Models.Catalog;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the category model factory
/// </summary>
public partial interface ICategoryModelFactory
{
    /// <summary>
    /// Prepare category search model
    /// </summary>
    /// <param name="searchModel">Category search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category search model
    /// </returns>
    Task<CategorySearchModel> PrepareCategorySearchModelAsync(CategorySearchModel searchModel);

    /// <summary>
    /// Prepare paged category list model
    /// </summary>
    /// <param name="searchModel">Category search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category list model
    /// </returns>
    Task<CategoryListModel> PrepareCategoryListModelAsync(CategorySearchModel searchModel);

    /// <summary>
    /// Prepare category model
    /// </summary>
    /// <param name="model">Category model</param>
    /// <param name="category">Category</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category model
    /// </returns>
    Task<CategoryModel> PrepareCategoryModelAsync(CategoryModel model, Category category, bool excludeProperties = false);

    /// <summary>
    /// Prepare paged category product list model
    /// </summary>
    /// <param name="searchModel">Category product search model</param>
    /// <param name="category">Category</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category product list model
    /// </returns>
    Task<CategoryProductListModel> PrepareCategoryProductListModelAsync(CategoryProductSearchModel searchModel, Category category);

    /// <summary>
    /// Prepare product search model to add to the category
    /// </summary>
    /// <param name="searchModel">Product search model to add to the category</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product search model to add to the category
    /// </returns>
    Task<AddProductToCategorySearchModel> PrepareAddProductToCategorySearchModelAsync(AddProductToCategorySearchModel searchModel);

    /// <summary>
    /// Prepare paged product list model to add to the category
    /// </summary>
    /// <param name="searchModel">Product search model to add to the category</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product list model to add to the category
    /// </returns>
    Task<AddProductToCategoryListModel> PrepareAddProductToCategoryListModelAsync(AddProductToCategorySearchModel searchModel);
}