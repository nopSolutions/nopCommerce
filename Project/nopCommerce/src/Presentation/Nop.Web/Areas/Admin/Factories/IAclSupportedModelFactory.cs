using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the model factory which supports access control list (ACL)
/// </summary>
public partial interface IAclSupportedModelFactory
{
    /// <summary>
    /// Prepare selected and all available customer roles for the passed model
    /// </summary>
    /// <typeparam name="TModel">ACL supported model type</typeparam>
    /// <param name="model">Model</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task PrepareModelCustomerRolesAsync<TModel>(TModel model) where TModel : IAclSupportedModel;

    /// <summary>
    /// Prepare selected and all available customer roles for the passed model by ACL mappings
    /// </summary>
    /// <typeparam name="TModel">ACL supported model type</typeparam>
    /// <param name="model">Model</param>
    /// <param name="entityName">Entity name</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task PrepareModelCustomerRolesAsync<TModel>(TModel model, string entityName)
        where TModel : BaseNopEntityModel, IAclSupportedModel;
}