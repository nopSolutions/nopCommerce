using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.PeachPayments
{
    public enum StatusMode
    {
        /// <summary>
        /// Pending
        /// </summary>
        Enabled = 0,

        /// <summary>
        /// Authorize
        /// </summary>
        Disabled = 1
    }
}
