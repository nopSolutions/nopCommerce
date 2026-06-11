using System.Data;
using FluentMigrator;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.Forums.Domain;

namespace Nop.Plugin.Misc.Forums.Data.Migrations;

[NopMigration("2026-08-10 00:00:01", "Misc.Forums schema", MigrationProcessType.Installation)]
public class SchemaMigration : Migration
{
    #region Utilities

    private void CreateIndexes()
    {
        if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(ForumSubscription))).Index("IX_Forums_Subscription_TopicId").Exists())
        {
            Create.Index("IX_Forums_Subscription_TopicId").OnTable(NameCompatibilityManager.GetTableName(typeof(ForumSubscription)))
                .OnColumn(nameof(ForumSubscription.TopicId)).Ascending()
                .WithOptions().NonClustered();
        }

        if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(ForumSubscription))).Index("IX_Forums_Subscription_ForumId").Exists())
        {
            Create.Index("IX_Forums_Subscription_ForumId").OnTable(NameCompatibilityManager.GetTableName(typeof(ForumSubscription)))
                .OnColumn(nameof(ForumSubscription.ForumId)).Ascending()
                .WithOptions().NonClustered();
        }

        if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(ForumGroup))).Index("IX_Forums_Group_DisplayOrder").Exists())
        {
            Create.Index("IX_Forums_Group_DisplayOrder").OnTable(NameCompatibilityManager.GetTableName(typeof(ForumGroup)))
                .OnColumn(nameof(ForumGroup.DisplayOrder)).Ascending()
                .WithOptions().NonClustered();
        }

        if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(Forum))).Index("IX_Forums_Forum_DisplayOrder").Exists())
        {
            Create.Index("IX_Forums_Forum_DisplayOrder").OnTable(NameCompatibilityManager.GetTableName(typeof(Forum)))
                .OnColumn(nameof(Forum.DisplayOrder)).Ascending()
                .WithOptions().NonClustered();
        }

        if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(ForumTopic))).Index("IX_Forums_Topic_Subject").Exists())
        {
            Create.Index("IX_Forums_Topic_Subject").OnTable(NameCompatibilityManager.GetTableName(typeof(ForumTopic)))
                .OnColumn(nameof(ForumTopic.Subject)).Ascending()
                .WithOptions().NonClustered();
        }

        if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(ForumTopic))).Index("IX_ForumTopic_CustomerId").Exists())
        {
            IfDatabase("sqlserver").Create.Index("IX_ForumTopic_CustomerId")
                    .OnTable(NameCompatibilityManager.GetTableName(typeof(ForumTopic)))
                    .OnColumn(nameof(ForumTopic.CustomerId)).Ascending()
                    .WithOptions().NonClustered();
        }

        if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(ForumPost))).Index("IX_ForumPost_CustomerId").Exists())
        {
            IfDatabase("sqlserver").Create.Index("IX_ForumPost_CustomerId")
                .OnTable(NameCompatibilityManager.GetTableName(typeof(ForumPost)))
                .OnColumn(nameof(ForumPost.CustomerId)).Ascending()
                .WithOptions().NonClustered();
        }

        if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(ForumSubscription))).Index("IX_ForumSubscription_CustomerId").Exists())
        {
            IfDatabase("sqlserver").Create.Index("IX_ForumSubscription_CustomerId")
                .OnTable(NameCompatibilityManager.GetTableName(typeof(ForumSubscription)))
                .OnColumn(nameof(ForumSubscription.CustomerId)).Ascending()
                .WithOptions().NonClustered();
        }
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

        this.CreateTableIfNotExists<ForumGroup>();
        this.CreateTableIfNotExists<Forum>();

        var forumTopicTableName = NameCompatibilityManager.GetTableName(typeof(ForumTopic));
        var forumTopicCustomerIdColumnName = NameCompatibilityManager.GetColumnName(typeof(ForumTopic), nameof(ForumTopic.CustomerId));

        if (Schema.Table(forumTopicTableName).Column(forumTopicCustomerIdColumnName).Exists())
        {
            Alter.Table(forumTopicTableName)
                .AlterColumn(forumTopicCustomerIdColumnName)
                .AsInt32()
                .Nullable()
                .ForeignKey(customerTableName, customerIdColumnName).OnDelete(Rule.SetNull);
        }
        else
        {
            this.CreateTableIfNotExists<ForumTopic>();
        }

        var forumPostTableName = NameCompatibilityManager.GetTableName(typeof(ForumPost));
        var forumPostCustomerIdColumnName = NameCompatibilityManager.GetColumnName(typeof(ForumPost), nameof(ForumPost.CustomerId));

        if (Schema.Table(forumPostTableName).Column(forumPostCustomerIdColumnName).Exists())
        {
            Alter.Table(forumPostTableName)
                .AlterColumn(forumPostCustomerIdColumnName)
                .AsInt32()
                .Nullable()
                .ForeignKey(customerTableName, customerIdColumnName).OnDelete(Rule.SetNull);
        }
        else
        {
            this.CreateTableIfNotExists<ForumPost>();
        }

        this.CreateTableIfNotExists<ForumPostVote>();
        this.CreateTableIfNotExists<ForumSubscription>();

        CreateIndexes();
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {
        Delete.Table(NameCompatibilityManager.GetTableName(typeof(ForumSubscription)));
        Delete.Table(NameCompatibilityManager.GetTableName(typeof(ForumPostVote)));
        Delete.Table(NameCompatibilityManager.GetTableName(typeof(ForumPost)));
        Delete.Table(NameCompatibilityManager.GetTableName(typeof(ForumTopic)));
        Delete.Table(NameCompatibilityManager.GetTableName(typeof(Forum)));
        Delete.Table(NameCompatibilityManager.GetTableName(typeof(ForumGroup)));
    }

    #endregion
}