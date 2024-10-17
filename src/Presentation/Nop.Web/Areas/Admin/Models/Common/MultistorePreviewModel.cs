using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Common;

public partial record MultistorePreviewModel : BaseNopModel
{
    public string StoreName { get; set; }
    public string Url { get; set; }
}
