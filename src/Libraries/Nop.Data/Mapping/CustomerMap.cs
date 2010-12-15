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
using Nop.Core.Domain;


namespace Nop.Data.Mapping
{
    public partial class CustomerMap : EntityTypeConfiguration<Customer>
    {
        public CustomerMap()
        {
            this.ToTable("Customer");
            this.HasKey(l => l.Id);
            this.Property(l => l.Email).IsRequired().HasMaxLength(255);
            this.Property(l => l.Username).IsRequired().HasMaxLength(255);
            this.Property(l => l.PasswordHash).IsRequired().HasMaxLength(255);
            this.Property(l => l.SaltKey).IsRequired().HasMaxLength(255);
            this.Property(l => l.AdminComment).HasMaxLength(int.MaxValue);
        
        }
    }
}