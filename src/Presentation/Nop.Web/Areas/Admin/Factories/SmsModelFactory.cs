using Nop.Services.Messages;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Sms;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the sms model factory implementation
/// </summary>
public partial class SmsModelFactory : ISmsModelFactory
{
    #region Fields

    protected readonly ISmsPluginManager _smsPluginManager;

    #endregion

    #region Ctor

    public SmsModelFactory(

        ISmsPluginManager smsPluginManager)
    {
        _smsPluginManager = smsPluginManager;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare sms provider search model
    /// </summary>
    /// <param name="searchModel">Sms provider search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sms provider search model
    /// </returns>
    public virtual Task<SmsProviderSearchModel> PrepareSmsProviderSearchModelAsync(SmsProviderSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged sms provider list model
    /// </summary>
    /// <param name="searchModel">Sms provider search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sms provider list model
    /// </returns>
    public virtual async Task<SmsProviderListModel> PrepareSmsProviderListModelAsync(SmsProviderSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get sms providers
        var smsProviders = (await _smsPluginManager.LoadAllPluginsAsync()).ToPagedList(searchModel);

        //prepare grid model
        var model = new SmsProviderListModel().PrepareToGrid(searchModel, smsProviders, () =>
        {
            return smsProviders.Select(provider =>
            {
                //fill in model values from the entity
                var smsProviderModel = provider.ToPluginModel<SmsProviderModel>();

                //fill in additional values (not existing in the entity)
                smsProviderModel.ConfigurationUrl = provider.GetConfigurationPageUrl();
                smsProviderModel.IsPrimaryProvider = _smsPluginManager.IsPluginActive(provider);

                return smsProviderModel;
            });
        });

        return model;
    }

    #endregion
}
