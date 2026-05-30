using Newtonsoft.Json;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework;
using Nop.Web.Framework.Events;

namespace Nop.Web.Areas.Admin.Infrastructure;

/// <summary>
/// Represents license terms event consumer
/// </summary>
public partial class LicenseTermsConsumer : IConsumer<PageRenderingEvent>
{
    #region Fields

    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly ISettingService _settingService;
    protected readonly NopHttpClient _nopHttpClient;

    #endregion

    #region Ctor

    public LicenseTermsConsumer(IHttpContextAccessor httpContextAccessor,
        ISettingService settingService,
        NopHttpClient nopHttpClient)
    {
        _httpContextAccessor = httpContextAccessor;
        _settingService = settingService;
        _nopHttpClient = nopHttpClient;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle page rendering event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(PageRenderingEvent eventMessage)
    {
        if (_httpContextAccessor.HttpContext.GetRouteValue("area") is not string area || area != AreaNames.ADMIN)
            return;

        var (exists, currentVersion) = await _settingService.TryGetLicenseAsync();
        if (!exists)
            return;

        if (currentVersion.AcceptedLicenseTerms)
            return;

        if ((currentVersion.InstallationDate ?? DateTime.UtcNow).AddMonths(1) > DateTime.UtcNow)
            return;

        if (currentVersion.LastCheckDate.HasValue && currentVersion.LastCheckDate.Value.AddHours(3) > DateTime.UtcNow)
            return;

        var licenseCheckModel = new LicenseCheckModel();
        try
        {
            currentVersion.LastCheckDate = DateTime.UtcNow;
            var result = await _nopHttpClient.CheckLicenseTermsAsync();
            if (!string.IsNullOrEmpty(result))
                licenseCheckModel = JsonConvert.DeserializeObject<LicenseCheckModel>(result);
        }
        catch { }

        await _settingService.TryGetLicenseAsync(licenseCheckModel.AcceptedLicenseTerms == true, currentVersion.LastCheckDate);

        if (licenseCheckModel.DisplayWarning == true && !string.IsNullOrEmpty(licenseCheckModel.WarningText))
            _httpContextAccessor.HttpContext.Items.TryAdd("licenseTerms", licenseCheckModel);
    }

    #endregion
}