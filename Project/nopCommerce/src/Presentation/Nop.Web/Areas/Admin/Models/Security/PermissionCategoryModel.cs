using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Security;

public partial record PermissionCategoryModel : BaseNopModel
{
    public string Name { get; set; }

    public string Text { get; set; }
}