using Nop.Core;
using Nop.Core.Domain.Catalog;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nop.Plugin.Misc.AbcCore.Domain
{
    /// <summary>
    /// A collection of products that use home delivery
    /// </summary>
    public partial class ProductHomeDelivery : BaseEntity
    {
        public virtual int Product_Id { get; set; }
    }
}