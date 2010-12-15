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
    public partial class LocalizedProductMap : EntityTypeConfiguration<LocalizedProduct>
    {
        public LocalizedProductMap()
        {
            this.ToTable("ProductLocalized");
            this.HasKey(lp => lp.Id);
            this.Property(lp => lp.Name).IsRequired().HasMaxLength(400);
            this.Property(lp => lp.ShortDescription).IsRequired().HasMaxLength(int.MaxValue);
            this.Property(lp => lp.FullDescription).IsRequired().HasMaxLength(int.MaxValue);
            this.Property(lp => lp.MetaKeywords).IsRequired().HasMaxLength(400);
            this.Property(lp => lp.MetaDescription).IsRequired();
            this.Property(lp => lp.MetaTitle).IsRequired().HasMaxLength(400);
            this.Property(lp => lp.SeName).IsRequired().HasMaxLength(100);
            
            this.HasRequired(lp => lp.Product)
                .WithMany(p => p.LocalizedProducts)
                .HasForeignKey(lp => lp.ProductId);

            this.HasRequired(lp => lp.Language)
                .WithMany(l => l.LocalizedProducts)
                .HasForeignKey(lp => lp.LanguageId);
        }
    }
}