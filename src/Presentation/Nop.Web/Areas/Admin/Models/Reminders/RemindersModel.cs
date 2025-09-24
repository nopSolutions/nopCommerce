using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Reminders;

/// <summary>
/// Represents reminders model
/// </summary>
public partial record RemindersModel : BaseNopModel, ISettingsModel
{
    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Admin.Promotions.Reminder.AbandonedCartEnabled")]
    public bool AbandonedCartEnabled { get; set; }
    public bool AbandonedCartEnabled_OverrideForStore { get; set; }
    public List<FollowUpModel> AbandonedCartFollowUps { get; set; } = new();

    [NopResourceDisplayName("Admin.Promotions.Reminder.PendingOrdersEnabled")]
    public bool PendingOrdersEnabled { get; set; }
    public bool PendingOrdersEnabled_OverrideForStore { get; set; }
    public List<FollowUpModel> PendingOrdersFollowUps { get; set; } = new();

    [NopResourceDisplayName("Admin.Promotions.Reminder.IncompleteRegistrationEnabled")]
    public bool IncompleteRegistrationEnabled { get; set; }
    public bool IncompleteRegistrationEnabled_OverrideForStore { get; set; }
    public List<FollowUpModel> IncompleteRegistrationFollowUps { get; set; } = new();
}