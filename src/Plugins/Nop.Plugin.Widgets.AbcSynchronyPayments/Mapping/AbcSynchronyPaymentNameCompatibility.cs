using System;
using System.Collections.Generic;
using Nop.Data.Mapping;
using Nop.Plugin.Misc.AbcCore.Domain;

namespace Nop.Plugin.Widgets.AbcSynchronyPayments.Mapping
{
    // required since ProductAbcBundles is loaded by John
    public partial class AbcSynchronyPaymentsNameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string>
        {
            { (typeof(ProductAbcFinance), "IsMonthlyPricing"), "Fix_Pay" },
            { (typeof(ProductAbcFinance), "IsDeferredPricing"), "Min_Pay" },
            { (typeof(ProductAbcFinance), "StartDate"), "BegDate" },
            // sketchy here, we're using this so we can have an ID populated, but we'll never use it
            { (typeof(ProductAbcFinance), "Id"), "AbcItemNumber" },
        };
    }
}