using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using DataShop.DemoPlugin.Domain;

namespace DataShop.DemoPlugin.Data
{
    public class DemoItemMap : EntityTypeConfiguration<DemoItem>
    {
        public DemoItemMap()
        {
            ToTable("DemoItems");
            //Map the primary key
            HasKey(m => m.Id);
            //Map the additional properties
            Property(m => m.ProductId);
            //Avoiding truncation/failure 
            //so we set the same max length used in the product name
            Property(m => m.ProductName).HasMaxLength(400);
            Property(m => m.IpAddress);
            Property(m => m.CustomerId);
            Property(m => m.IsRegistered);
        }
    }
}
