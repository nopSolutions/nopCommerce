using FluentMigrator;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.Forums.Domain;

namespace Nop.Plugin.Misc.Forums.Data.Migrations;

[NopMigration("2026-08-10 00:00:00", "Misc.Forums: Delete constraints migration", MigrationProcessType.Installation)]
public class DeleteConstraintsMigration : Migration
{
    #region Fields

    private readonly INopDataProvider _dataProvider;

    #endregion

    #region Ctor

    public DeleteConstraintsMigration(INopDataProvider dataProvider)
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
        var customerTableName = NameCompatibilityManager.GetTableName(typeof(Customer));
        var customerIdColumnName = NameCompatibilityManager.GetColumnName(typeof(Customer), nameof(BaseEntity.Id));

        var forumTopicTableName = NameCompatibilityManager.GetTableName(typeof(ForumTopic));
        var forumTopicCustomerIdColumnName = NameCompatibilityManager.GetColumnName(typeof(ForumTopic), nameof(ForumTopic.CustomerId));

        if (Schema.Table(forumTopicTableName).Column(forumTopicCustomerIdColumnName).Exists())
        {
            var constraintName = _dataProvider
                .CreateForeignKeyName(forumTopicTableName, forumTopicCustomerIdColumnName, customerTableName, customerIdColumnName);

            if (Schema.Table(forumTopicTableName).Constraint(constraintName).Exists())
                Delete.UniqueConstraint(constraintName).FromTable(forumTopicTableName);
        }


        var forumPostTableName = NameCompatibilityManager.GetTableName(typeof(ForumPost));
        var forumPostCustomerIdColumnName = NameCompatibilityManager.GetColumnName(typeof(ForumPost), nameof(ForumPost.CustomerId));

        if (Schema.Table(forumPostTableName).Column(forumPostCustomerIdColumnName).Exists())
        {
            var constraintName = _dataProvider
                .CreateForeignKeyName(forumPostTableName, forumPostCustomerIdColumnName, customerTableName, customerIdColumnName);

            if (Schema.Table(forumPostTableName).Constraint(constraintName).Exists())
                Delete.UniqueConstraint(constraintName).FromTable(forumPostTableName);
        }

    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {
    }

    #endregion
}