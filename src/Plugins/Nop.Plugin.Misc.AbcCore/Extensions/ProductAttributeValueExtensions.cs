using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Nop.Plugin.Misc.AbcCore.Extensions
{
    public static class ProductAttributeValueExtensions
    {
        public static bool EqualTo(this ProductAttributeValue pav, ProductAttributeValue pav2)
        {
            return pav.Name == pav2.Name
                && pav.PriceAdjustment == pav2.PriceAdjustment
                && pav.Cost == pav2.Cost
                && pav.IsPreSelected == pav2.IsPreSelected;
        }
    }
}