using Nop.Plugin.Widgets.FacebookPixel.Domain;
using Nop.Plugin.Widgets.FacebookPixel.Models;
using Riok.Mapperly.Abstractions;

namespace Nop.Plugin.Widgets.FacebookPixel.Infrastructure.Mapper;

/// <summary>
/// Facebook Pixel mapper
/// </summary>
[Mapper]
public partial class FacebookPixelMapper
{
    public partial FacebookPixelModel Map(FacebookPixelConfiguration source);
    public partial FacebookPixelConfiguration Map(FacebookPixelModel source);
}