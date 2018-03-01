using Nop.Core.Domain.Catalog;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Kendoui;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the category model factory
    /// </summary>
    public partial interface ICategoryModelFactory
    {
        /// <summary>
        /// Prepare category list model
        /// </summary>
        /// <param name="model">Category list model</param>
        /// <returns>Category list model</returns>
        CategoryListModel PrepareCategoryListModel(CategoryListModel model);

        /// <summary>
        /// Prepare paged category list model for the grid
        /// </summary>
        /// <param name="listModel">Category list model</param>
        /// <param name="command">Pagination parameters</param>
        /// <returns>Grid model</returns>
        DataSourceResult PrepareCategoryListGridModel(CategoryListModel listModel, DataSourceRequest command);

        /// <summary>
        /// Prepare category model
        /// </summary>
        /// <param name="model">Category model</param>
        /// <param name="category">Category</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Category model</returns>
        CategoryModel PrepareCategoryModel(CategoryModel model, Category category, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged category product list model for the grid
        /// </summary>
        /// <param name="command">Pagination parameters</param>
        /// <param name="category">Category</param>
        /// <returns>Grid model</returns>
        DataSourceResult PrepareCategoryProductListGridModel(DataSourceRequest command, Category category);

        /// <summary>
        /// Prepare add category product list model
        /// </summary>
        /// <param name="model">Add category product list model</param>
        /// <returns>Add category product list model</returns>
        CategoryModel.AddCategoryProductModel PrepareAddCategoryProductListModel(CategoryModel.AddCategoryProductModel model);

        /// <summary>
        /// Prepare paged add category product list model for the grid
        /// </summary>
        /// <param name="model">Add category product list model</param>
        /// <param name="command">Pagination parameters</param>
        /// <returns>Grid model</returns>
        DataSourceResult PrepareAddCategoryProductListGridModel(CategoryModel.AddCategoryProductModel listModel,
            DataSourceRequest command);
    }
}