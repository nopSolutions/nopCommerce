using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.Forums.Domain;

namespace Nop.Plugin.Misc.Forums.Data.Migrations;

[NopMigration("2026-02-02 00:00:00", "Misc.Forums schema", MigrationProcessType.Installation)]
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
        this.CreateTableIfNotExists<ForumGroup>();
        this.CreateTableIfNotExists<Forum>();
        this.CreateTableIfNotExists<ForumTopic>();
        this.CreateTableIfNotExists<ForumPost>();
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