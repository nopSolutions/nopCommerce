using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Media;

public partial record Product3dObjectModel : BaseNopEntityModel
{
    public string ObjectUrl { get; set; }
    public string AlternateText { get; set; }
    public string PosterImageUrl { get; set; }
    public string ThumbImageUrl { get; set; }

    public bool ZoomEnabled { get; set; }
    public bool AutoRotateEnabled { get; set; }
    public bool CameraControlEnabled { get; set; }
    public bool LazyLoadEnabled { get; set; }
}