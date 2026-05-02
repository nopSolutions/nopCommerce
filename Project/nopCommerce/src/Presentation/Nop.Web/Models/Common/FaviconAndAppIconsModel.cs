using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common;

public partial record FaviconAndAppIconsModel : BaseNopModel
{
    public string HeadCode { get; set; }
}