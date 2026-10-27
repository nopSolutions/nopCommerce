using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Widgets.FacebookPixel.Domain;
using Nop.Plugin.Widgets.FacebookPixel.Models;

namespace Nop.Plugin.Widgets.FacebookPixel.Infrastructure.Mapper;

/// <summary>
/// Represents mapper configuration for plugin models
/// </summary>
public class MapperConfiguration : BaseMapperProfile
{
    #region Ctor

    public MapperConfiguration()
    {
        CreateMap<FacebookPixelConfiguration, FacebookPixelModel>()
            .ForMember(model => model.AvailableStores, options => options.Ignore())
            .ForMember(model => model.CustomEventSearchModel, options => options.Ignore())
            .ForMember(model => model.CustomProperties, options => options.Ignore())
            .ForMember(model => model.HideCustomEventsSearch, options => options.Ignore())
            .ForMember(model => model.HideStoresList, options => options.Ignore())
            .ForMember(model => model.StoreName, options => options.Ignore());
        CreateMap<FacebookPixelModel, FacebookPixelConfiguration>()
            .ForMember(entity => entity.CustomEvents, options => options.Ignore());
    }

    #endregion
}