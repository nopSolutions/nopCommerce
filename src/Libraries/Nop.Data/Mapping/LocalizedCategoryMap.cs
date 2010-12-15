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
    public partial class LocalizedCategoryMap : EntityTypeConfiguration<LocalizedCategory>
    {
        public LocalizedCategoryMap()
        {
            this.ToTable("CategoryLocalized");
            this.HasKey(lc => lc.Id);
            this.Property(lc => lc.Name).IsRequired().HasMaxLength(400);
            this.Property(lc => lc.Description).IsRequired().HasMaxLength(int.MaxValue);
            this.Property(lc => lc.MetaKeywords).IsRequired().HasMaxLength(400);
            this.Property(lc => lc.MetaDescription).IsRequired();
            this.Property(lc => lc.MetaTitle).IsRequired().HasMaxLength(400);
            this.Property(lc => lc.SeName).IsRequired().HasMaxLength(100);
            
            this.HasRequired(lc => lc.Category)
                .WithMany(c => c.LocalizedCategories)
                .HasForeignKey(lc => lc.CategoryId);

            this.HasRequired(lc => lc.Language)
                .WithMany(l => l.LocalizedCategories)
                .HasForeignKey(lc => lc.LanguageId);
        }
    }
}