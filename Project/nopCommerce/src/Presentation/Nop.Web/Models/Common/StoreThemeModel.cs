using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common;

public partial record StoreThemeModel : BaseNopModel
{
    public string Name { get; set; }
    public string Title { get; set; }
}