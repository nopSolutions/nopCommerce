using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Widgets.AccessiBe.Models;

namespace Nop.Plugin.Widgets.AccessiBe.Infrastructure;

/// <summary>
/// Represents AutoMapper configuration for widget models
/// </summary>
public class MapperConfiguration : Profile, IOrderedMapperProfile
{
    #region Ctor

    public MapperConfiguration()
    {
        CreateMap<AccessiBeMobileSettings, AccessiBeTriggerMobileModel>();
        CreateMap<AccessiBeTriggerMobileModel, AccessiBeMobileSettings>();

        CreateMap<AccessiBeSettings, AccessiBeTriggerModel>()
            .ForMember(model => model.ShowMobile, options => options.MapFrom(src => !src.HideMobile)); //invert
        CreateMap<AccessiBeTriggerModel, AccessiBeSettings>()
            .ForMember(setting => setting.HideMobile, options => options.MapFrom(src => !src.ShowMobile)); //invert
    }

    #endregion

    #region Properties

    /// <summary>
    /// Order of this mapper implementation
    /// </summary>
    public int Order => 1;

    #endregion
}
