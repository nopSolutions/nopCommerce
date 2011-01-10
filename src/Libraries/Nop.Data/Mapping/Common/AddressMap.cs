using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Common;

namespace Nop.Data.Mapping.Common
{
    public partial class AddressMap : EntityTypeConfiguration<Address>
    {
        public AddressMap()
        {
            this.ToTable("Addresses");
            this.HasKey(a => a.Id);
        }
    }
}
