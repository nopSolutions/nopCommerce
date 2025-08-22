using Mapster;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Widgets.AccessiBe.Models;

namespace Nop.Plugin.Widgets.AccessiBe.Infrastructure;

/// <summary>
/// Represents Mapster configuration for widget models
/// </summary>
public class MapperConfiguration : IOrderedMapperProfile
{
    #region Methods

    /// <summary>
    /// Configure mappings for Mapster
    /// </summary>
    /// <param name="config">Type adapter configuration</param>
    public void Configure(TypeAdapterConfig config)
    {
        config.NewConfig<AccessiBeMobileSettings, AccessiBeTriggerMobileModel>();
        config.NewConfig<AccessiBeTriggerMobileModel, AccessiBeMobileSettings>();

        config.NewConfig<AccessiBeSettings, AccessiBeTriggerModel>()
              .Map(model => model.ShowMobile, src => !src.HideMobile); // invert
        config.NewConfig<AccessiBeTriggerModel, AccessiBeSettings>()
              .Map(setting => setting.HideMobile, src => !src.ShowMobile); // invert
    }

    #endregion

    #region Properties

    /// <summary>
    /// Order of this mapper implementation
    /// </summary>
    public int Order => 1;

    #endregion
}
