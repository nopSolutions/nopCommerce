using Nop.Web.Areas.Admin.Models.Sms;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the sms model factory
/// </summary>
public partial interface ISmsModelFactory
{
    /// <summary>
    /// Prepare sms provider search model
    /// </summary>
    /// <param name="searchModel">Sms provider search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sms provider search model
    /// </returns>
    Task<SmsProviderSearchModel> PrepareSmsProviderSearchModelAsync(SmsProviderSearchModel searchModel);

    /// <summary>
    /// Prepare paged sms provider list model
    /// </summary>
    /// <param name="searchModel">Sms provider search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sms provider list model
    /// </returns>
    Task<SmsProviderListModel> PrepareSmsProviderListModelAsync(SmsProviderSearchModel searchModel);
}