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
using Nop.Core.Domain.Catalog;


namespace Nop.Data.Mapping.Catalog
{
    public partial class ProductVariantMap : EntityTypeConfiguration<ProductVariant>
    {
        public ProductVariantMap()
        {
            this.ToTable("ProductVariant");
            this.HasKey(pv => pv.Id);
            this.Property(pv => pv.Name).IsRequired().HasMaxLength(400);
            this.Property(pv => pv.Sku).HasMaxLength(400);
            this.Property(pv => pv.Description).IsMaxLength();
            this.Property(pv => pv.AdminComment).IsMaxLength();
            this.Property(pv => pv.ManufacturerPartNumber).HasMaxLength(400);
            this.Property(pv => pv.UserAgreementText).IsMaxLength();

            this.Property(pv => pv.AdditionalShippingCharge).HasPrecision(18, 4);
            this.Property(pv => pv.Price).HasPrecision(18, 4);
            this.Property(pv => pv.OldPrice).HasPrecision(18, 4);
            this.Property(pv => pv.ProductCost).HasPrecision(18, 4);
            this.Property(pv => pv.MinimumCustomerEnteredPrice).HasPrecision(18, 4);
            this.Property(pv => pv.MaximumCustomerEnteredPrice).HasPrecision(18, 4);
            this.Property(pv => pv.Weight).HasPrecision(18, 4);
            this.Property(pv => pv.Length).HasPrecision(18, 4);
            this.Property(pv => pv.Width).HasPrecision(18, 4);
            this.Property(pv => pv.Height).HasPrecision(18, 4);

            this.Ignore(pv => pv.BackorderMode);
            this.Ignore(pv => pv.DownloadActivationType);
            this.Ignore(pv => pv.GiftCardType);
            this.Ignore(pv => pv.LowStockActivity);
            this.Ignore(pv => pv.ManageInventoryMethod);
            this.Ignore(pv => pv.RecurringCyclePeriod);


            
            this.HasRequired(pv => pv.Product)
                .WithMany(p => p.ProductVariants)
                .HasForeignKey(pv => pv.ProductId);
        }
    }
}