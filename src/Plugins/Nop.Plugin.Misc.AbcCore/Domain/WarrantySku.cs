using Nop.Core;
using Nop.Core.Domain.Catalog;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nop.Plugin.Misc.AbcCore.Domain
{
    /// <summary>
    /// A mapping from products to a cart price. these should be unique to one product
    /// </summary>
    public partial class WarrantySku : BaseEntity
    {
        public virtual string Name { get; set; }

        public virtual string Sku { get; set; }
    }
}