using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Nop.Services.Payments;

namespace Nop.Web.Framework.Controllers
{
    /// <summary>
    /// Base controller for payment plugins
    /// </summary>
    public abstract class BasePaymentController : BasePluginController
    {
        /// <summary>
        /// Validate payment form
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>List of validating errors</returns>
        public abstract IList<string> ValidatePaymentForm(FormCollection form);

        /// <summary>
        /// Get payment information
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>Payment info holder</returns>
        public abstract ProcessPaymentRequest GetPaymentInfo(FormCollection form);
    }
}
