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
    public partial class LocalizedManufacturerMap : EntityTypeConfiguration<LocalizedManufacturer>
    {
        public LocalizedManufacturerMap()
        {
            this.ToTable("ManufacturerLocalized");
            this.HasKey(lm => lm.Id);
            this.Property(lm => lm.Name).IsRequired().HasMaxLength(400);
            this.Property(lm => lm.Description).IsRequired().HasMaxLength(int.MaxValue);
            this.Property(lm => lm.MetaKeywords).IsRequired().HasMaxLength(400);
            this.Property(lm => lm.MetaDescription).IsRequired();
            this.Property(lm => lm.MetaTitle).IsRequired().HasMaxLength(400);
            this.Property(lm => lm.SeName).IsRequired().HasMaxLength(100);

            this.HasRequired(lm => lm.Manufacturer)
                .WithMany(m => m.LocalizedManufacturers)
                .HasForeignKey(lm => lm.ManufacturerId);

            this.HasRequired(lm => lm.Language)
                .WithMany(l => l.LocalizedManufacturers)
                .HasForeignKey(lm => lm.LanguageId);
        }
    }
}