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
    public partial class ProductVariantMap : EntityTypeConfiguration<ProductVariant>
    {
        public ProductVariantMap()
        {
            this.ToTable("ProductVariant");
            this.HasKey(p => p.Id);
            this.Property(p => p.Name).IsRequired().HasMaxLength(400);
            this.Property(p => p.Sku).IsRequired().HasMaxLength(400);
            this.Property(p => p.Description).IsRequired().HasMaxLength(int.MaxValue);
            this.Property(p => p.AdminComment).IsRequired().HasMaxLength(int.MaxValue);
            this.Property(p => p.ManufacturerPartNumber).IsRequired().HasMaxLength(400);
            this.Property(p => p.UserAgreementText).IsRequired().HasMaxLength(int.MaxValue);
            this.Ignore(p => p.BackorderMode);
            this.Ignore(p => p.DownloadActivationType);
            this.Ignore(p => p.GiftCardType);
            this.Ignore(p => p.LowStockActivityId);
            this.Ignore(p => p.ManageInventoryMethod);
            this.Ignore(p => p.RecurringProductCyclePeriod);

            this.HasRequired(pv => pv.Product)
                .WithMany(p => p.ProductVariants)
                .HasForeignKey(pv => pv.ProductId);
        }
    }
}