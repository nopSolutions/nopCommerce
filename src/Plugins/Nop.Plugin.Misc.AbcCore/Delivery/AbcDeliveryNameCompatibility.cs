using System;
using System.Collections.Generic;
using Nop.Plugin.Misc.AbcCore.Delivery;
using Nop.Data.Mapping;

namespace Nop.Plugin.Misc.AbcCore.Delivery
{
    public partial class AbcDeliveryNameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string> { };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string>
        {
            { (typeof(AbcDeliveryItem), "Id"), "Item_Number" },
            { (typeof(AbcDeliveryMap), "Id"), "CategoryId" },
        };
    }
}