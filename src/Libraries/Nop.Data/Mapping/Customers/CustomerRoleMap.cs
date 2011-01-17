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
    public partial class CustomerRoleMap : EntityTypeConfiguration<CustomerRole>
    {
        public CustomerRoleMap()
        {
            this.ToTable("CustomerRole");
            this.HasKey(cr => cr.Id);
            this.Property(cr => cr.Name).IsRequired().HasMaxLength(255);
            this.Property(cr => cr.SystemName).IsRequired().HasMaxLength(255);

            //this.HasMany(cr => cr.Customers)
            //    .WithMany(c => c.CustomerRoles)
            //    .Map(m =>
            //    {
            //        m.ToTable("Customer_CustomerRole_Mapping");
            //        m.MapLeftKey(cr => cr.Id, "FkToCustomerRoleId");
            //        m.MapRightKey(c => c.Id, "FkToCustomerId");
            //    });

            this.HasMany(cr => cr.Customers)
                .WithMany(c => c.CustomerRoles)
                .Map(m => m.ToTable("Customer_CustomerRole_Mapping"));
        }
    }
}