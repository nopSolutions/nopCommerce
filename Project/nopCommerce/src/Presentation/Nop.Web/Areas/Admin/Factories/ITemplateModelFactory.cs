using Nop.Web.Areas.Admin.Models.Templates;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the template model factory
/// </summary>
public partial interface ITemplateModelFactory
{
    /// <summary>
    /// Prepare templates model
    /// </summary>
    /// <param name="model">Templates model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the mplates model
    /// </returns>
    Task<TemplatesModel> PrepareTemplatesModelAsync(TemplatesModel model);

    /// <summary>
    /// Prepare paged category template list model
    /// </summary>
    /// <param name="searchModel">Category template search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category template list model
    /// </returns>
    Task<CategoryTemplateListModel> PrepareCategoryTemplateListModelAsync(CategoryTemplateSearchModel searchModel);

    /// <summary>
    /// Prepare paged manufacturer template list model
    /// </summary>
    /// <param name="searchModel">Manufacturer template search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturer template list model
    /// </returns>
    Task<ManufacturerTemplateListModel> PrepareManufacturerTemplateListModelAsync(ManufacturerTemplateSearchModel searchModel);

    /// <summary>
    /// Prepare paged product template list model
    /// </summary>
    /// <param name="searchModel">Product template search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product template list model
    /// </returns>
    Task<ProductTemplateListModel> PrepareProductTemplateListModelAsync(ProductTemplateSearchModel searchModel);

    /// <summary>
    /// Prepare paged topic template list model
    /// </summary>
    /// <param name="searchModel">Topic template search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the topic template list model
    /// </returns>
    Task<TopicTemplateListModel> PrepareTopicTemplateListModelAsync(TopicTemplateSearchModel searchModel);

    /// <summary>
    /// Prepare category template search model
    /// </summary>
    /// <param name="searchModel">Category template search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category template search model
    /// </returns>
    Task<CategoryTemplateSearchModel> PrepareCategoryTemplateSearchModelAsync(CategoryTemplateSearchModel searchModel);

    /// <summary>
    /// Prepare manufacturer template search model
    /// </summary>
    /// <param name="searchModel">Manufacturer template search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturer template search model
    /// </returns>
    Task<ManufacturerTemplateSearchModel> PrepareManufacturerTemplateSearchModelAsync(ManufacturerTemplateSearchModel searchModel);

    /// <summary>
    /// Prepare product template search model
    /// </summary>
    /// <param name="searchModel">Product template search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product template search model
    /// </returns>
    Task<ProductTemplateSearchModel> PrepareProductTemplateSearchModelAsync(ProductTemplateSearchModel searchModel);

    /// <summary>
    /// Prepare topic template search model
    /// </summary>
    /// <param name="searchModel">Topic template search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the topic template search model
    /// </returns>
    Task<TopicTemplateSearchModel> PrepareTopicTemplateSearchModelAsync(TopicTemplateSearchModel searchModel);
}