using System.Collections.Generic;

namespace Nop.Services.Payments
{
    /// <summary>
    /// Represents a CancelRecurringPaymentResult
    /// </summary>
    public partial class CancelRecurringPaymentResult
    {
        public IList<string> Errors { get; set; }

        public CancelRecurringPaymentResult() 
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
    }
}
