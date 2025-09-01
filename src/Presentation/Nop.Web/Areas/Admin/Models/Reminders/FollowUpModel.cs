using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Reminders;

/// <summary>
/// Represents a follow up model
/// </summary>
public partial record FollowUpModel : BaseNopModel
{
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Promotions.Reminder.FollowUp.Enabled")]
    public bool Enabled { get; set; }

    [NopResourceDisplayName("Admin.Promotions.Reminder.FollowUp.DelayBeforeSend")]
    public int DelayBeforeSend { get; set; }

    public int DelayPeriodId { get; set; }
}
