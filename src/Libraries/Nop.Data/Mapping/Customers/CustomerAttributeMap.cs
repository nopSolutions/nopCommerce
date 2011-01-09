//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Customers;


namespace Nop.Data.Mapping.Customers
{
    public partial class CustomerAttributeMap : EntityTypeConfiguration<CustomerAttribute>
    {
        public CustomerAttributeMap()
        {
            this.ToTable("CustomerAttribute");

            this.HasKey(ca => ca.Id);
            this.Property(ca => ca.Key).IsRequired().HasMaxLength(200);
            this.Property(ca => ca.Value).IsRequired().HasMaxLength(1000);

            this.HasRequired(ca => ca.Customer)
                .WithMany(c => c.CustomerAttributes)
                .HasForeignKey(ca => ca.CustomerId);

        }
    }
}