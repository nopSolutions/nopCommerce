using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.Polls.Domain;

namespace Nop.Plugin.Misc.Polls.Data.Migrations;

[NopMigration("2025-11-11 00:00:00", "Misc.Polls schema", MigrationProcessType.Installation)]
public class SchemaMigration : Migration
{
    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(Poll))).Exists())
            this.CreateTableIfNotExists<Poll>();
        else
            this.AddOrAlterColumnFor<Poll>(t => t.ShowInLeftSideColumn).AsBoolean().WithDefaultValue(false);

        this.CreateTableIfNotExists<PollAnswer>();
        this.CreateTableIfNotExists<PollVotingRecord>();
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {
        Delete.Table(NameCompatibilityManager.GetTableName(typeof(PollVotingRecord)));
        Delete.Table(NameCompatibilityManager.GetTableName(typeof(PollAnswer)));
        Delete.Table(NameCompatibilityManager.GetTableName(typeof(Poll)));
    }

    #endregion
}