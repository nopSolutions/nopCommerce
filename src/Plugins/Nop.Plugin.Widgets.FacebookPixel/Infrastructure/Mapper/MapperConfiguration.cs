using Mapster;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Widgets.FacebookPixel.Domain;
using Nop.Plugin.Widgets.FacebookPixel.Models;

namespace Nop.Plugin.Widgets.FacebookPixel.Infrastructure.Mapper;

/// <summary>
/// Represents Mapster configuration for plugin models
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
        config.NewConfig<FacebookPixelConfiguration, FacebookPixelModel>()
              .Ignore(model => model.AvailableStores)
              .Ignore(model => model.CustomEventSearchModel)
              .Ignore(model => model.CustomProperties)
              .Ignore(model => model.HideCustomEventsSearch)
              .Ignore(model => model.HideStoresList)
              .Ignore(model => model.StoreName);
        config.NewConfig<FacebookPixelModel, FacebookPixelConfiguration>()
              .Ignore(entity => entity.CustomEvents);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Order of this mapper implementation
    /// </summary>
    public int Order => 1;

    #endregion
}