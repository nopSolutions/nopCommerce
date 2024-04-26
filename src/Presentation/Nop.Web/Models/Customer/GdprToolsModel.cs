using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Customer;

public partial record GdprToolsModel : BaseNopModel
{
    public string Result { get; set; }
}