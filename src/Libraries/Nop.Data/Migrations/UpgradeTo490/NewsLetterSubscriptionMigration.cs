using FluentMigrator;
using Nop.Core.Domain.Messages;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.UpgradeTo490;

[NopSchemaMigration("2025-03-19 00:00:00", "Multiple newsletter lists")]
public class NewsLetterSubscriptionMigration : ForwardOnlyMigration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(NewsLetterSubscriptionType))).Exists())
            Create.TableFor<NewsLetterSubscriptionType>();

        if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(NewsLetterSubscriptionTypeMapping))).Exists())
            Create.TableFor<NewsLetterSubscriptionTypeMapping>();
    }
}
