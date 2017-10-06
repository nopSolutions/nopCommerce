using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Directory;

namespace Nop.Core.Domain.Shipping
{
    public class ShippingMethodCountry
    {
        public int CountryId { get; set; }
        public virtual Country Country { get; set; }

        public int ShippingMethodId { get; set; }
        public virtual ShippingMethod ShippingMethod { get; set; }
    }
}
