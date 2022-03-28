using System;
using System.Collections.Generic;
using Nop.Data.Mapping;
using Nop.Plugin.Misc.AbcCore.Domain;
using Nop.Plugin.Widgets.AbcBonusBundle.Domain;

namespace Nop.Plugin.Misc.AbcCore.Mapping
{
    // required since ProductAbcBundles is loaded by John
    public partial class AbcBonusBundleNameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(ProductAbcBundle), "ProductAbcBundles" },
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string>
        {
            { (typeof(ProductAbcBundle), "EndDate"), "end_date" },
            { (typeof(ProductAbcBundle), "StartDate"), "start_date" },
            { (typeof(ProductAbcBundle), "MemoNumber"), "memo_no" },
            { (typeof(ProductAbcBundle), "ItemNumber"), "Item_Number" },
            // sketchy here, we're using this so we can have an ID populated, but we'll never use it
            { (typeof(ProductAbcBundle), "Id"), "Item_Number" },
        };
    }
}