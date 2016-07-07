using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Payments;

namespace Nop.Services.Payments
{
    /// <summary>
    /// Refund payment result
    /// </summary>
    public partial class RefundPaymentResult
    {
        private PaymentStatus _newPaymentStatus = PaymentStatus.Pending;

        /// <summary>
        /// Ctor
        /// </summary>
        public RefundPaymentResult() 
        {
            this.Errors = new List<string>();
        }

        /// <summary>
        /// Gets a value indicating whether request has been completed successfully
        /// </summary>
        public bool Success
        {
            get { return (!this.Errors.Any()); }
        }

        /// <summary>
        /// Add error
        /// </summary>
        /// <param name="error">Error</param>
        public void AddError(string error) 
        {
            this.Errors.Add(error);
        }

        /// <summary>
        /// Errors
        /// </summary>
        public IList<string> Errors { get; set; }

        /// <summary>
        /// Gets or sets a payment status after processing
        /// </summary>
        public PaymentStatus NewPaymentStatus
        {
            get
            {
                return _newPaymentStatus;
            }
            set
            {
                _newPaymentStatus = value;
            }
        }
    }
}
