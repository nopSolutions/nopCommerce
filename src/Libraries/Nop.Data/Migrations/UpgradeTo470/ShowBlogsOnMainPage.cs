using FluentMigrator;
using Nop.Core.Domain.Blogs;
namespace Nop.Data.Migrations.UpgradeTo470
{

    [NopSchemaMigration("2023-05-14 15:15:00", "Add new columns to blogPost and blogSettings")]
    public class ShowBlogOnMainPage:AutoReversingMigration{
        public override void Up()
        {
            if (!Schema.Table(nameof(BlogPost)).Column(nameof(BlogPost.ShowBlogPostOnMainPage)).Exists()){
                Alter.Table(nameof(BlogPost))
                    .AddColumn(nameof(BlogPost.ShowBlogPostOnMainPage)).AsBoolean().WithDefaultValue(false);
            }
        }
    }
}