using Nop.Web.Areas.Admin.Models.Templates;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the template model factory
    /// </summary>
    public partial interface ITemplateModelFactory
    {
        /// <summary>
        /// Prepare templates model
        /// </summary>
        /// <param name="model">Templates model</param>
        /// <returns>Templates model</returns>
        TemplatesModel PrepareTemplatesModel(TemplatesModel model);

        /// <summary>
        /// Prepare category template search model
        /// </summary>
        /// <param name="searchModel">Category template search model</param>
        /// <returns>Category template search model</returns>
        CategoryTemplateSearchModel PrepareCategoryTemplateSearchModel(CategoryTemplateSearchModel searchModel);

        /// <summary>
        /// Prepare paged category template list model
        /// </summary>
        /// <param name="searchModel">Category template search model</param>
        /// <returns>Category template list model</returns>
        CategoryTemplateListModel PrepareCategoryTemplateListModel(CategoryTemplateSearchModel searchModel);

        /// <summary>
        /// Prepare manufacturer template search model
        /// </summary>
        /// <param name="searchModel">Manufacturer template search model</param>
        /// <returns>Manufacturer template search model</returns>
        ManufacturerTemplateSearchModel PrepareManufacturerTemplateSearchModel(ManufacturerTemplateSearchModel searchModel);

        /// <summary>
        /// Prepare paged manufacturer template list model
        /// </summary>
        /// <param name="searchModel">Manufacturer template search model</param>
        /// <returns>Manufacturer template list model</returns>
        ManufacturerTemplateListModel PrepareManufacturerTemplateListModel(ManufacturerTemplateSearchModel searchModel);

        /// <summary>
        /// Prepare product template search model
        /// </summary>
        /// <param name="searchModel">Product template search model</param>
        /// <returns>Product template search model</returns>
        ProductTemplateSearchModel PrepareProductTemplateSearchModel(ProductTemplateSearchModel searchModel);

        /// <summary>
        /// Prepare paged product template list model
        /// </summary>
        /// <param name="searchModel">Product template search model</param>
        /// <returns>Product template list model</returns>
        ProductTemplateListModel PrepareProductTemplateListModel(ProductTemplateSearchModel searchModel);

        /// <summary>
        /// Prepare topic template search model
        /// </summary>
        /// <param name="searchModel">Topic template search model</param>
        /// <returns>Topic template search model</returns>
        TopicTemplateSearchModel PrepareTopicTemplateSearchModel(TopicTemplateSearchModel searchModel);

        /// <summary>
        /// Prepare paged topic template list model
        /// </summary>
        /// <param name="searchModel">Topic template search model</param>
        /// <returns>Topic template list model</returns>
        TopicTemplateListModel PrepareTopicTemplateListModel(TopicTemplateSearchModel searchModel);
    }
}