using System;
using System.Xml;
using Nop.Core.Infrastructure;
using Nop.Core.Tasks;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Represents a task for deleting guest customers
    /// </summary>
    public partial class DeleteGuestsTask : ITask
    {
        private int _olderThanMinutes = 1440; //60*24 = 1 day

        private readonly ICustomerService _customerService = EngineContext.Current.Resolve<ICustomerService>();

        /// <summary>
        /// Executes a task
        /// </summary>
        /// <param name="node">Xml node that represents a task description</param>
        public void Execute(XmlNode node)
        {
            var olderThanMinutesAttribute = node.Attributes["olderThanMinutes"];
            if (olderThanMinutesAttribute != null && !string.IsNullOrWhiteSpace(olderThanMinutesAttribute.Value))
            {
                this._olderThanMinutes = int.Parse(olderThanMinutesAttribute.Value);
            }

            _customerService.DeleteGuestCustomers(null, DateTime.UtcNow.AddMinutes(-_olderThanMinutes), true);
        }
    }
}
