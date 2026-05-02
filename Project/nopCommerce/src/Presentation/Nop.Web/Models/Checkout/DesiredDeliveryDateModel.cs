using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Checkout;

public partial record DesiredDeliveryDateModel : BaseNopModel
{
    public DesiredDeliveryDateModel()
    {
        AvailableDates = new List<SelectListItem>();
    }

    public bool Enabled { get; set; }
    public IList<SelectListItem> AvailableDates { get; set; }
    public string SelectedDate { get; set; }
}
