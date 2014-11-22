using System;
using Nop.Services.Tasks;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Represents a task for deleting guest customers
    /// </summary>
    public partial class DeleteGuestsTask : ITask
    {
        private readonly ICustomerService _customerService;

        public DeleteGuestsTask(ICustomerService customerService)
        {
            this._customerService = customerService;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            //60*24 = 1 day
            var olderThanMinutes = 1440; //TODO move to settings
            //Do not delete more than 1000 records per time. This way the system is not slowed down
            _customerService.DeleteGuestCustomers(null, DateTime.UtcNow.AddMinutes(-olderThanMinutes), true);
        }
    }
}
