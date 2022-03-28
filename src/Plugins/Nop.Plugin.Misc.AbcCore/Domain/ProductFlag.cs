using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Plugin.Misc.AbcCore.Domain
{
    public partial class ProductFlag : BaseEntity
    {
        public virtual int ProductId { get; set; }
        public virtual string PriceBucketImageUrl { get; set; }
        public virtual string StockMessage { get; set; }
        public virtual string NewModelMessage { get; set; }

        public bool IsOpenBox()
        {
            return IsOpenBox10Percent() || IsOpenBox15Percent() || IsOpenBox20Percent();
        }

        public bool IsOpenBox10Percent()
        {
            return PriceBucketImageUrl == "OpenBox10Percent";
        }

        public bool IsOpenBox15Percent()
        {
            return PriceBucketImageUrl == "OpenBox15Percent";
        }

        public bool IsOpenBox20Percent()
        {
            return PriceBucketImageUrl == "OpenBox20Percent";
        }

        public bool IsSpecialOrder()
        {
            return StockMessage == "Normally ships in 2-3 weeks.";
        }

        public bool IsInStoreOnly()
        {
            return PriceBucketImageUrl.Contains("InStore");
        }

        public decimal CalculateOpenBoxPrice(decimal price)
        {
            if (IsOpenBox10Percent()) return Math.Floor(price - (price * 0.10M));
            if (IsOpenBox15Percent()) return Math.Floor(price - (price * 0.15M));
            if (IsOpenBox20Percent()) return Math.Floor(price - (price * 0.20M));

            return price;
        }
    }
}