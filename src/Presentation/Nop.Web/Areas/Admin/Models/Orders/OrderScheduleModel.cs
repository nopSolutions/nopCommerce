using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    public partial class OrderScheduleModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Orders.Fields.ScheduleDate1")]
        public string ScheduleDate1 { get; set; }

        [NopResourceDisplayName("Admin.Orders.Fields.ScheduleDate2")]
        public string ScheduleDate2 { get; set; }

        [NopResourceDisplayName("Admin.Orders.Fields.ScheduleDate3")]
        public string ScheduleDate3 { get; set; }
    }
}