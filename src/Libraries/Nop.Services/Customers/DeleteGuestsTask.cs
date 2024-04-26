using Nop.Core.Domain.Customers;
using Nop.Services.ScheduleTasks;

namespace Nop.Services.Customers;

/// <summary>
/// Represents a task for deleting guest customers
/// </summary>
public partial class DeleteGuestsTask : IScheduleTask
{
    #region Fields

    protected readonly CustomerSettings _customerSettings;
    protected readonly ICustomerService _customerService;

    #endregion

    #region Ctor

    public DeleteGuestsTask(CustomerSettings customerSettings,
        ICustomerService customerService)
    {
        _customerSettings = customerSettings;
        _customerService = customerService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes a task
    /// </summary>
    public async System.Threading.Tasks.Task ExecuteAsync()
    {
        var olderThanMinutes = _customerSettings.DeleteGuestTaskOlderThanMinutes;
        // Default value in case 0 is returned.  0 would effectively disable this service and harm performance.
        olderThanMinutes = olderThanMinutes == 0 ? 1440 : olderThanMinutes;

        await _customerService.DeleteGuestCustomersAsync(null, DateTime.UtcNow.AddMinutes(-olderThanMinutes), true);
    }

    #endregion
}