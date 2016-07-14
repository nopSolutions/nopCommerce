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
                if (!this.IsActive)
                    return null;

                var historyCollection = this.RecurringPaymentHistory;
                if (historyCollection.Count >= this.TotalCycles)
                {
                    return null;
                }

                //result
                DateTime? result = null;

                //set another value to change calculation method
                //bool useLatestPayment = false;
                //if (useLatestPayment)
                //{
                //    //get latest payment
                //    RecurringPaymentHistory latestPayment = null;
                //    foreach (var historyRecord in historyCollection)
                //    {
                //        if (latestPayment != null)
                //        {
                //            if (historyRecord.CreatedOnUtc >= latestPayment.CreatedOnUtc)
                //            {
                //                latestPayment = historyRecord;
                //            }
                //        }
                //        else
                //        {
                //            latestPayment = historyRecord;
                //        }
                //    }


                //    //calculate next payment date
                //    if (latestPayment != null)
                //    {
                //        switch (this.CyclePeriod)
                //        {
                //            case RecurringProductCyclePeriod.Days:
                //                result = latestPayment.CreatedOnUtc.AddDays((double)this.CycleLength);
                //                break;
                //            case RecurringProductCyclePeriod.Weeks:
                //                result = latestPayment.CreatedOnUtc.AddDays((double)(7 * this.CycleLength));
                //                break;
                //            case RecurringProductCyclePeriod.Months:
                //                result = latestPayment.CreatedOnUtc.AddMonths(this.CycleLength);
                //                break;
                //            case RecurringProductCyclePeriod.Years:
                //                result = latestPayment.CreatedOnUtc.AddYears(this.CycleLength);
                //                break;
                //            default:
                //                throw new NopException("Not supported cycle period");
                //        }
                //    }
                //    else
                //    {
                //        if (this.TotalCycles > 0)
                //            result = this.StartDateUtc;
                //    }
                //}
                //else
                //{
                    if (historyCollection.Any())
                    {
                        switch (this.CyclePeriod)
                        {
                            case RecurringProductCyclePeriod.Days:
                                result = this.StartDateUtc.AddDays((double)this.CycleLength * historyCollection.Count);
                                break;
                            case RecurringProductCyclePeriod.Weeks:
                                result = this.StartDateUtc.AddDays((double)(7 * this.CycleLength) * historyCollection.Count);
                                break;
                            case RecurringProductCyclePeriod.Months:
                                result = this.StartDateUtc.AddMonths(this.CycleLength * historyCollection.Count);
                                break;
                            case RecurringProductCyclePeriod.Years:
                                result = this.StartDateUtc.AddYears(this.CycleLength * historyCollection.Count);
                                break;
                            default:
                                throw new NopException("Not supported cycle period");
                        }
                    }
                    else
                    {
                        if (this.TotalCycles > 0)
                            result = this.StartDateUtc;
                    }
                //}

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
                var historyCollection = this.RecurringPaymentHistory;
                int result = this.TotalCycles - historyCollection.Count;
                if (result < 0)
                    result = 0;

                return result;
            }
        }

        /// <summary>
        /// Gets or sets the payment status
        /// </summary>
        public RecurringProductCyclePeriod CyclePeriod
        {
            get
            {
                return (RecurringProductCyclePeriod)this.CyclePeriodId;
            }
            set
            {
                this.CyclePeriodId = (int)value;
            }
        }
        



        /// <summary>
        /// Gets or sets the recurring payment history
        /// </summary>
        public virtual ICollection<RecurringPaymentHistory> RecurringPaymentHistory
        {
            get { return _recurringPaymentHistory ?? (_recurringPaymentHistory = new List<RecurringPaymentHistory>()); }
            protected set { _recurringPaymentHistory = value; }
        }        

        /// <summary>
        /// Gets the initial order
        /// </summary>
        public virtual Order InitialOrder { get; set; }
    }
}
