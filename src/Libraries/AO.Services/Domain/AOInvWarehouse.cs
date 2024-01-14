using Nop.Core;
using System;

namespace AO.Services.Domain
{
    public partial class AOInvWarehouse : BaseEntity
    {        
        public string Address { get; set; }
        
        public string Name { get; set; }
    }
}
