using Nop.Services.Catalog;

namespace Nop.Plugin.Widgets.GoogleAnalytics;

/// <summary>
/// Provides extension methods for the Google Analytics widget plugin
/// </summary>
public static class PluginExtensions
{
    /// <summary>
    /// Gets the category breadcrumb for a product
    /// </summary>
    /// <param name="categoryService">Category service</param>
    /// <param name="productId">Product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category breadcrumb for a product
    /// </returns>
    public static async Task<string> GetCategoryNameForProductAsync(this ICategoryService categoryService, int productId)
    {
        var category = await categoryService.GetCategoryByIdAsync((await categoryService.GetProductCategoriesByProductIdAsync(productId)).FirstOrDefault()?.CategoryId ?? 0);
        var categoryName = string.Empty;

        if (category != null)
            categoryName = await categoryService.GetFormattedBreadCrumbAsync(category, separator: ">");

        if (string.IsNullOrEmpty(categoryName))
            categoryName = "No category";

        return categoryName;
    }
}