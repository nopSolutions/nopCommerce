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
using Nop.Core.Domain.Discounts;


namespace Nop.Data.Mapping.Discounts
{
    public partial class DiscountMap : EntityTypeConfiguration<Discount>
    {
        public DiscountMap()
        {
            this.ToTable("Discount");
            this.HasKey(d => d.Id);
            this.Property(d => d.Name).IsRequired().HasMaxLength(200);
            this.Property(d => d.CouponCode).IsRequired().HasMaxLength(100);
            this.Ignore(d => d.DiscountType);
            this.Ignore(d => d.DiscountLimitation);

            this.HasMany(dr => dr.DiscountRequirements)
                .WithRequired(d => d.Discount)
                .HasForeignKey(dr => dr.DiscountId);

            this.HasMany(dr => dr.AppliedToCategories)
                .WithMany(c => c.AppliedDiscounts)
                .Map(m => m.ToTable("Discount_AppliedToCategories"));
            
            this.HasMany(dr => dr.AppliedToProductVariants)
                .WithMany(pv => pv.AppliedDiscounts)
                .Map(m => m.ToTable("Discount_AppliedToProductVariants"));
        }
    }
}