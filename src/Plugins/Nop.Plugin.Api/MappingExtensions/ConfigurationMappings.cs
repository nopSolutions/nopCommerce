using Nop.Plugin.Api.Areas.Admin.Models;
using Nop.Plugin.Api.AutoMapper;
using Nop.Plugin.Api.Domain;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class ConfigurationMappings
    {
        public static ConfigurationModel ToModel(this ApiSettings apiSettings)
        {
            return apiSettings.MapTo<ApiSettings, ConfigurationModel>();
        }

        public static ApiSettings ToEntity(this ConfigurationModel apiSettingsModel)
        {
            return apiSettingsModel.MapTo<ConfigurationModel, ApiSettings>();
        }
    }
}
