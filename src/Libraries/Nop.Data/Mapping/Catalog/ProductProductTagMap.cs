using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public class ProductProductTagMap : NopEntityTypeConfiguration<Product_ProductTag_Mapping>
    {
        public override void Configure(EntityTypeBuilder<Product_ProductTag_Mapping> builder)
        {
            base.Configure(builder);
            builder.HasKey(t => new { t.ProductId, t.ProductTagId });
            builder.HasOne(ca => ca.Product)
                .WithMany(ca => ca.ProductTags)
                .HasForeignKey(ca => ca.ProductId);

            builder.HasOne(ca => ca.ProductTag)
                .WithMany(ca => ca.Products)
                .HasForeignKey(ca => ca.ProductTagId);
        }
    }
}
