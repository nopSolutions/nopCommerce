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
        private readonly ICustomerService _customerService = EngineContext.Current.Resolve<ICustomerService>();

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            //60*24 = 1 day
            var olderThanMinutes = 1440; //TODO move to settings
            _customerService.DeleteGuestCustomers(null, DateTime.UtcNow.AddMinutes(-olderThanMinutes), true);
        }
    }
}
