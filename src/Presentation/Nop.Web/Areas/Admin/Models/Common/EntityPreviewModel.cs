using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Common;

public partial record EntityPreviewModel : BaseNopEntityModel
{
    #region Ctor

    public EntityPreviewModel()
    {
        PreviewModels = new List<MultistorePreviewModel>();
    }

    #endregion

    #region Properties

    public Type ModelType { get; set; }

    public IList<MultistorePreviewModel> PreviewModels { get; set; }

    #endregion
}