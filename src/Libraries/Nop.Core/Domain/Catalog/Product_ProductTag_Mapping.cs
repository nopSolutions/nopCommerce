using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Catalog
{
    public class Product_ProductTag_Mapping
    {
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int ProductTagId { get; set; }
        public virtual ProductTag ProductTag { get; set; }
    }
}
