using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a product 3D object
/// </summary>
public partial record Product3dObjectModel : BaseNopEntityModel
{
    #region Properties

    [NopResourceDisplayName("Admin.Catalog.Products.Multimedia.Object3d.Fields.FileName")]
    public string FileName { get; set; }
    public string FileUrl { get; set; }
    public long FileSize { get; set; }
    public int UploadLimit { get; set; }

    [UIHint("Picture")]
    [NopResourceDisplayName("Admin.Catalog.Products.Multimedia.Object3d.Fields.Preview")]
    public int PreviewPictureId { get; set; }
    public string PictureUrl { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.Multimedia.Object3d.Fields.AltAttribute")]
    public string AltAttribute { get; set; }

    #endregion
}
