using Nop.Core;
using Nop.Core.Domain.Catalog;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nop.Plugin.Misc.AbcCore.Domain
{
    /// <summary>
    /// A mapping from shopping cart items to hidden attributes
    /// </summary>
    public partial class HiddenAttributeValue : BaseEntity
    {
        public virtual int ShoppingCartItem_Id { get; set; }

        public virtual int HiddenAttribute_Id { get; set; }

        public virtual decimal PriceAdjustment { get; set; }

        public virtual HiddenAttribute HiddenAttribute { get; set; }

    }
}