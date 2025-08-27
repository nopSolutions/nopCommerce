using System.Data;
using FluentMigrator;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Data.Extensions;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.News.Domain;

namespace Nop.Plugin.Misc.News.Data.Migrations;

[NopMigration("2025-03-06 00:00:00", "Misc.News schema", MigrationProcessType.Installation)]
public class SchemaMigration : Migration
{
    #region Fields

    private readonly INopDataProvider _dataProvider;

    #endregion

    #region Ctor

    public SchemaMigration(INopDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        var newsTableName = NameCompatibilityManager.GetTableName(typeof(NewsItem));
        var newsCommentTableName = NameCompatibilityManager.GetTableName(typeof(NewsComment));
        var newsCommentCustomerIdColunbName = NameCompatibilityManager.GetColumnName(typeof(NewsComment), nameof(NewsComment.CustomerId));

        if (!Schema.Table(newsTableName).Exists())
            Create.TableFor<NewsItem>();

        if (Schema.Table(newsCommentTableName).Column(newsCommentCustomerIdColunbName).Exists())
        {
            var customerTableName = NameCompatibilityManager.GetTableName(typeof(Customer));
            var customerIdColumnName = NameCompatibilityManager.GetColumnName(typeof(Customer), nameof(BaseEntity.Id));

            var constraintName = _dataProvider
                .CreateForeignKeyName(newsCommentTableName, newsCommentCustomerIdColunbName, customerTableName, customerIdColumnName);

            Delete.ForeignKey(constraintName).OnTable(newsCommentTableName);
            Alter.Column(newsCommentCustomerIdColunbName)
                .OnTable(newsCommentTableName)
                .AsInt32()
                .Nullable()
                .ForeignKey(customerTableName, customerIdColumnName)
                .OnDelete(Rule.SetNull);
        }
        else
        {
            Create.TableFor<NewsComment>();
        }
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down() 
    {
        Delete.Table(NameCompatibilityManager.GetTableName(typeof(NewsComment)));
        Delete.Table(NameCompatibilityManager.GetTableName(typeof(NewsItem)));
    }

    #endregion
}
