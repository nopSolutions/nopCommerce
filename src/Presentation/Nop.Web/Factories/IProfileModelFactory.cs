using Nop.Core.Domain.Customers;
using Nop.Web.Models.Profile;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the interface of the profile model factory
/// </summary>
public partial interface IProfileModelFactory
{
    /// <summary>
    /// Prepare the profile index model
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="page">Number of posts page; pass null to disable paging</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the profile index model
    /// </returns>
    Task<ProfileIndexModel> PrepareProfileIndexModelAsync(Customer customer, int? page);

    /// <summary>
    /// Prepare the profile info model
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the profile info model
    /// </returns>
    Task<ProfileInfoModel> PrepareProfileInfoModelAsync(Customer customer);

    /// <summary>
    /// Prepare the profile posts model
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="page">Number of posts page</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the profile posts model  
    /// </returns>
    Task<ProfilePostsModel> PrepareProfilePostsModelAsync(Customer customer, int page);
}