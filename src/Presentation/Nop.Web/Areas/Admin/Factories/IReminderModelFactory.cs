using Nop.Web.Areas.Admin.Models.Reminders;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the reminder model factory
/// </summary>
public partial interface IReminderModelFactory
{
    /// <summary>
    /// Prepare reminders model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the reminders model
    /// </returns>
    Task<RemindersModel> PrepareRemindersModelAsync();
}
