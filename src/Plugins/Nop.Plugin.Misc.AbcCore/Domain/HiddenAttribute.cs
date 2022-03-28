using Nop.Core;
using Nop.Core.Domain.Catalog;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nop.Plugin.Misc.AbcCore.Domain
{
    /// <summary>
    /// A custom attribute that will not appear but affect shopping cart price
    /// </summary>
    public partial class HiddenAttribute : BaseEntity
    {
        public virtual string Name { get; set; }
        public virtual ICollection<HiddenAttributeValue> HiddenAttributeValues { get; set; }
    }
}