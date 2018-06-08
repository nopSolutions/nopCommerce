using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class ProductMap
    {
        protected override void PostInitialize()
        {
            this.Property(p => p.Author).HasMaxLength(400);
        }
    }
}
