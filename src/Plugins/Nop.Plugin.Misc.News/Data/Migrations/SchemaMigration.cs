using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.News.Domain;

namespace Nop.Plugin.Misc.News.Data.Migrations;

[NopMigration("2025-03-06 00:00:00", "Misc.News schema", MigrationProcessType.Installation)]
public class SchemaMigration : Migration
{
    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        this.CreateTableIfNotExists<NewsItem>();

        var newsCommentTableName = NameCompatibilityManager.GetTableName(typeof(NewsComment));
        var newsCommentCustomerIdColumnName = NameCompatibilityManager.GetColumnName(typeof(NewsComment), nameof(NewsComment.CustomerId));
        if (Schema.Table(newsCommentTableName).Column(newsCommentCustomerIdColumnName).Exists())
        {
            this.AddOrAlterForeignKeyColumnFor<NewsComment, Customer>(record => record.CustomerId, "NewsComment_Customer")
                .OnDelete(Rule.SetNull)
                .Nullable();
        }
        else
        {
            this.CreateTableIfNotExists<NewsComment>();
        }
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {
        this.DeleteTableIfExists<NewsComment>();
        this.DeleteTableIfExists<NewsItem>();
    }

    #endregion
}