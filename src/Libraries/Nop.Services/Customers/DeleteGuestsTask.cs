using System;
using Nop.Services.Configuration;
using Nop.Services.Tasks;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Represents a task for deleting guest customers
    /// </summary>
    public partial class DeleteGuestsTask : ITask
    {
        private readonly ICustomerService _customerService;
        private readonly ISettingService _settingService;

        public DeleteGuestsTask(ICustomerService customerService, ISettingService settingService)
        {
            this._customerService = customerService;
            this._settingService = settingService;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            var olderThanMinutes = _settingService.GetSettingByKey<int>("Tasks.DeleteGuestTask.OlderThanMinutes"); 
            // Default value in case 0 is returned.  0 would effectively disable this service and harm performance.
            olderThanMinutes = olderThanMinutes == 0 ? 1440 : olderThanMinutes;
    
            //Do not delete more than 1000 records per time. This way the system is not slowed down
            _customerService.DeleteGuestCustomers(null, DateTime.UtcNow.AddMinutes(-olderThanMinutes), true);
        }
    }
}
