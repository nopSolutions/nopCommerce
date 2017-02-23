
using Nop.Services.Logging;
using Nop.Services.Tasks;
using System;

namespace Nop.Services.Orders
{
    public partial class AssignInvoiceIdentTask : ITask
    {
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ILogger _logger;
        public AssignInvoiceIdentTask(IOrderProcessingService orderProcessingService,
           ILogger logger)
        {
            this._orderProcessingService = orderProcessingService;
            this._logger = logger;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            try
            {
                _orderProcessingService.AssignInvoiceIdentToOrders();
            }
            catch (Exception exc)
            {
                _logger.Error(string.Format("Error assigning invoice ident. {0}", exc.Message), exc);
            }
        }
    }
}
