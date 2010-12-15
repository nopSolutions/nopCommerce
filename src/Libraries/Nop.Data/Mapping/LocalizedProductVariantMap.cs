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
    public partial class LocalizedProductVariantMap : EntityTypeConfiguration<LocalizedProductVariant>
    {
        public LocalizedProductVariantMap()
        {
            this.ToTable("ProductVariantLocalized");
            this.HasKey(lpv => lpv.Id);
            this.Property(lpv => lpv.Name).IsRequired().HasMaxLength(400);
            this.Property(lpv => lpv.Description).IsRequired().HasMaxLength(int.MaxValue);

            this.HasRequired(lpv => lpv.ProductVariant)
                .WithMany(pv => pv.LocalizedProductVariants)
                .HasForeignKey(lpv => lpv.ProductVariantId);


            this.HasRequired(lpv => lpv.Language)
                .WithMany(l => l.LocalizedProductVariants)
                .HasForeignKey(lpv => lpv.LanguageId);
        }
    }
}