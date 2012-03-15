using System;
using Nop.Core.Infrastructure;
using Nop.Services.Tasks;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Represents a task for deleting guest customers
    /// </summary>
    public partial class DeleteGuestsTask : ITask
    {
        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            var customerService = EngineContext.Current.Resolve<ICustomerService>();
            //60*24 = 1 day
            var olderThanMinutes = 1440; //TODO move to settings
            customerService.DeleteGuestCustomers(null, DateTime.UtcNow.AddMinutes(-olderThanMinutes), true);
        }
    }
}
