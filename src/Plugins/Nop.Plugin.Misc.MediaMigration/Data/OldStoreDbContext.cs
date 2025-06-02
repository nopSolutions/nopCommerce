using Microsoft.EntityFrameworkCore;
using Nop.Plugin.Misc.MediaMigration.Models;

namespace Nop.Plugin.Misc.MediaMigration.Data;
public class OldStoreDbContext : DbContext
{
    public OldStoreDbContext(DbContextOptions<OldStoreDbContext> options)
    : base(options)
    {
    }

    public DbSet<OldProductMedia> ProductMedias { get; set; }
    public DbSet<Persistent_Map_Product> Persistent_Map_Products { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OldProductMedia>().ToTable("Tbl_ProductMedia").HasKey(x=>x.PM_ID);
        modelBuilder.Entity<Persistent_Map_Product>().HasNoKey().ToTable("Persistent_Map_Product");

        // اگر جدول‌های دیگری هم دارید به همین روش اضافه کنید
        // modelBuilder.Entity<OldMediaType>().HasNoKey().ToTable("Tbl_MediaType");
    }

}
