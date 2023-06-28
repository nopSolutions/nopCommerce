using Nop.Core;
using Nop.Data;
using System;
using System.Linq;

namespace Nop.Plugin.Misc.AbcCore.Domain
{
    public class ProductAbcFinance : BaseEntity
    {
        public virtual string AbcItemNumber { get; set; }
        public virtual string Sku { get; set; }
        public virtual string Description { get; set; }
        public virtual int Months { get; set; }
        public virtual string TransPromo { get; set; }
        public virtual bool IsMonthlyPricing { get; set; }
        public virtual bool IsDeferredPricing { get; set; }
        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
    }
}