using Nop.Core.Domain.Catalog;
using Nop.Web.Areas.Admin.Models.Catalog;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the specification attribute model factory
/// </summary>
public partial interface ISpecificationAttributeModelFactory
{
    /// <summary>
    /// Prepare specification attribute group search model
    /// </summary>
    /// <param name="searchModel">Specification attribute group search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute group search model
    /// </returns>
    Task<SpecificationAttributeGroupSearchModel> PrepareSpecificationAttributeGroupSearchModelAsync(SpecificationAttributeGroupSearchModel searchModel);

    /// <summary>
    /// Prepare paged specification attribute group list model
    /// </summary>
    /// <param name="searchModel">Specification attribute group search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute group list model
    /// </returns>
    Task<SpecificationAttributeGroupListModel> PrepareSpecificationAttributeGroupListModelAsync(SpecificationAttributeGroupSearchModel searchModel);

    /// <summary>
    /// Prepare specification attribute group model
    /// </summary>
    /// <param name="model">Specification attribute group model</param>
    /// <param name="specificationAttributeGroup">Specification attribute group</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute group model
    /// </returns>
    Task<SpecificationAttributeGroupModel> PrepareSpecificationAttributeGroupModelAsync(SpecificationAttributeGroupModel model,
        SpecificationAttributeGroup specificationAttributeGroup, bool excludeProperties = false);

    /// <summary>
    /// Prepare paged specification attribute list model
    /// </summary>
    /// <param name="searchModel">Specification attribute search model</param>
    /// <param name="group">Specification attribute group</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute list model
    /// </returns>
    Task<SpecificationAttributeListModel> PrepareSpecificationAttributeListModelAsync(SpecificationAttributeSearchModel searchModel, SpecificationAttributeGroup group);

    /// <summary>
    /// Prepare specification attribute model
    /// </summary>
    /// <param name="model">Specification attribute model</param>
    /// <param name="specificationAttribute">Specification attribute</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute model
    /// </returns>
    Task<SpecificationAttributeModel> PrepareSpecificationAttributeModelAsync(SpecificationAttributeModel model,
        SpecificationAttribute specificationAttribute, bool excludeProperties = false);

    /// <summary>
    /// Prepare paged specification attribute option list model
    /// </summary>
    /// <param name="searchModel">Specification attribute option search model</param>
    /// <param name="specificationAttribute">Specification attribute</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute option list model
    /// </returns>
    Task<SpecificationAttributeOptionListModel> PrepareSpecificationAttributeOptionListModelAsync(
        SpecificationAttributeOptionSearchModel searchModel, SpecificationAttribute specificationAttribute);

    /// <summary>
    /// Prepare specification attribute option model
    /// </summary>
    /// <param name="model">Specification attribute option model</param>
    /// <param name="specificationAttribute">Specification attribute</param>
    /// <param name="specificationAttributeOption">Specification attribute option</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute option model
    /// </returns>
    Task<SpecificationAttributeOptionModel> PrepareSpecificationAttributeOptionModelAsync(SpecificationAttributeOptionModel model,
        SpecificationAttribute specificationAttribute, SpecificationAttributeOption specificationAttributeOption,
        bool excludeProperties = false);

    /// <summary>
    /// Prepare paged list model of products that use the specification attribute
    /// </summary>
    /// <param name="searchModel">Search model of products that use the specification attribute</param>
    /// <param name="specificationAttribute">Specification attribute</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list model of products that use the specification attribute
    /// </returns>
    Task<SpecificationAttributeProductListModel> PrepareSpecificationAttributeProductListModelAsync(
        SpecificationAttributeProductSearchModel searchModel, SpecificationAttribute specificationAttribute);
}