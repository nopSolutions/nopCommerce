using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcCore.Domain
{
    public partial class CustomerShopMapping : BaseEntity
    {
        public virtual int CustomerId { get; set; }
        public virtual int ShopId { get; set; }
    }
}
