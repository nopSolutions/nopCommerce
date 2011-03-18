using System.Collections.Generic;
using Nop.Core.Domain.Payments;

namespace Nop.Services.Payments
{
    /// <summary>
    /// Represents a CapturePaymentResult
    /// </summary>
    public partial class CapturePaymentResult
    {
        private PaymentStatus _newPaymentStatus = PaymentStatus.Pending;
        public IList<string> Errors { get; set; }

        public CapturePaymentResult() 
        {
            this.Errors = new List<string>();
        }

        public bool Success
        {
            get { return (this.Errors.Count == 0); }
        }

        public void AddError(string error)
        {
            this.Errors.Add(error);
        }
        
        /// <summary>
        /// Gets or sets the capture transaction identifier
        /// </summary>
        public string CaptureTransactionId { get; set; }

        /// <summary>
        /// Gets or sets the capture transaction result
        /// </summary>
        public string CaptureTransactionResult { get; set; }

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
