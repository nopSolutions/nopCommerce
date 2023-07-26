using Nop.Core.Domain.Gdpr;
using Nop.Services.Customers;
using Nop.Services.ScheduleTasks;

namespace Nop.Services.Gdpr
{
    /// <summary>
    /// Represents a task for deleting inactive customers
    /// </summary>
    public partial class DeleteInactiveCustomersTask : IScheduleTask
    {
        #region Fields

        protected readonly ICustomerService _customerService;
        protected readonly IGdprService _gdprService;
        protected readonly GdprSettings _gdprSettings;

        #endregion

        #region Ctor

        public DeleteInactiveCustomersTask(ICustomerService customerService,
            IGdprService gdprService,
            GdprSettings gdprSettings)
        {
            _customerService = customerService;
            _gdprService = gdprService;
            _gdprSettings = gdprSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        public async Task ExecuteAsync()
        {
            if (!_gdprSettings.GdprEnabled)
                return;

            var lastActivityToUtc = DateTime.UtcNow.AddMonths(-_gdprSettings.DeleteInactiveCustomersAfterMonths);
            var inactiveCustomers = await _customerService.GetAllCustomersAsync(lastActivityToUtc: lastActivityToUtc);

            foreach (var customer in inactiveCustomers.Where(c => !c.IsSystemAccount))
                await _gdprService.PermanentDeleteCustomerAsync(customer);
        }

        #endregion
    }
}
