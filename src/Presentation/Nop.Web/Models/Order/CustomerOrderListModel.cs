using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Common;

namespace Nop.Web.Models.Order;

public partial record CustomerOrderListModel : BaseNopModel
{
    public List<CustomerOrderModel> Orders { get; set; } = new();
    public IList<SelectListItem> AvailableLimits { get; set; }
    public PagerModel PagerModel { get; set; }
}