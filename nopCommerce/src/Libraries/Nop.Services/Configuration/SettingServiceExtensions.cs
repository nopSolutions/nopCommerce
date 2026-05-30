using System.Globalization;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Common;

namespace Nop.Services.Configuration;

public static class SettingServiceExtensions
{
    public static async Task<(bool Exists, LicenseTermsInfo CurrentVersion)> TryGetLicenseAsync(this ISettingService settingService,
        bool accepted = false, DateTime? checkDate = null)
    {
        var jsonSerializerSettings = new JsonSerializerSettings
        {
            DateFormatString = @"yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK",
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            DateParseHandling = DateParseHandling.DateTime,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            Culture = CultureInfo.InvariantCulture
        };

        try
        {
            var needToSave = checkDate.HasValue;
            var value = await settingService.GetSettingByKeyAsync<string>($"{nameof(AdminAreaSettings)}.{nameof(AdminAreaSettings.LicenseTerms)}");
            var licenseTerms = JsonConvert.DeserializeObject<LicenseTerms>(value ?? string.Empty, jsonSerializerSettings) ?? new();

            var currentVersion = licenseTerms.FirstOrDefault(info => string.Equals(info.Version, NopVersion.CURRENT_VERSION));
            if (currentVersion is null)
            {
                currentVersion = new LicenseTermsInfo { Version = NopVersion.CURRENT_VERSION, InstallationDate = DateTime.UtcNow };
                licenseTerms.Add(currentVersion);
                needToSave = true;
            }

            if (needToSave || !currentVersion.AcceptedLicenseTerms && accepted)
            {
                currentVersion.AcceptedLicenseTerms |= accepted;
                currentVersion.LastCheckDate = checkDate ?? currentVersion.LastCheckDate;
                value = JsonConvert.SerializeObject(licenseTerms, jsonSerializerSettings);
                await settingService.SetSettingAsync($"{nameof(AdminAreaSettings)}.{nameof(AdminAreaSettings.LicenseTerms)}", value);
            }

            return (true, currentVersion);
        }
        catch
        {
            return default;
        }
    }
}