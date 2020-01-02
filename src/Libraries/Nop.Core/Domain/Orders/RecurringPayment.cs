using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Catalog;

namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents a recurring payment
    /// </summary>
    public partial class RecurringPayment : BaseEntity
    {
        private ICollection<RecurringPaymentHistory> _recurringPaymentHistory;

        /// <summary>
        /// Gets or sets the cycle length
        /// </summary>
        public int CycleLength { get; set; }

        /// <summary>
        /// Gets or sets the cycle period identifier
        /// </summary>
        public int CyclePeriodId { get; set; }

        /// <summary>
        /// Gets or sets the total cycles
        /// </summary>
        public int TotalCycles { get; set; }

        /// <summary>
        /// Gets or sets the start date
        /// </summary>
        public DateTime StartDateUtc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the payment is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the last payment failed
        /// </summary>
        public bool LastPaymentFailed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the initial order identifier
        /// </summary>
        public int InitialOrderId { get; set; }

        /// <summary>
        /// Gets or sets the date and time of payment creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
        
        /// <summary>
        /// Gets the next payment date
        /// </summary>
        public DateTime? NextPaymentDate
        {
            get
            {
                if (!IsActive)
                    return null;

                var historyCollection = RecurringPaymentHistory;
                if (historyCollection.Count >= TotalCycles)
                {
                    return null;
                }

                //result
                DateTime? result = null;

                //calculate next payment date
                if (historyCollection.Any())
                {
                    switch (CyclePeriod)
                    {
                        case RecurringProductCyclePeriod.Days:
                            result = StartDateUtc.AddDays((double)CycleLength * historyCollection.Count);
                            break;
                        case RecurringProductCyclePeriod.Weeks:
                            result = StartDateUtc.AddDays((double)(7 * CycleLength) * historyCollection.Count);
                            break;
                        case RecurringProductCyclePeriod.Months:
                            result = StartDateUtc.AddMonths(CycleLength * historyCollection.Count);
                            break;
                        case RecurringProductCyclePeriod.Years:
                            result = StartDateUtc.AddYears(CycleLength * historyCollection.Count);
                            break;
                        default:
                            throw new NopException("Not supported cycle period");
                    }
                }
                else
                {
                    if (TotalCycles > 0)
                        result = StartDateUtc;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the cycles remaining
        /// </summary>
        public int CyclesRemaining
        {
            get
            {
                //result
                var historyCollection = RecurringPaymentHistory;
                var result = TotalCycles - historyCollection.Count;
                if (result < 0)
                    result = 0;

                return result;
            }
        }

        /// <summary>
        /// Gets or sets the cycle period
        /// </summary>
        public RecurringProductCyclePeriod CyclePeriod
        {
            get => (RecurringProductCyclePeriod)CyclePeriodId;
            set => CyclePeriodId = (int)value;
        }

        /// <summary>
        /// Gets or sets the recurring payment history
        /// </summary>
        public virtual ICollection<RecurringPaymentHistory> RecurringPaymentHistory
        {
            get => _recurringPaymentHistory ?? (_recurringPaymentHistory = new List<RecurringPaymentHistory>());
            protected set => _recurringPaymentHistory = value;
        }        

        /// <summary>
        /// Gets the initial order
        /// </summary>
        public virtual Order InitialOrder { get; set; }
    }
}
