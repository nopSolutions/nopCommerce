using Nop.Core;
using Nop.Data;
using System;
using System.Linq;

namespace Nop.Plugin.Misc.AbcCore.Domain
{
    public partial class ProductEnergyGuide : BaseEntity
    {
        public virtual int ProductId { get; set; }
        public virtual string EnergyGuideUrl { get; set; }
    }
}