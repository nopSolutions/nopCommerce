using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Messages;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.UpgradeTo490;

[NopSchemaMigration("2025-03-19 00:00:00", "Multiple newsletter lists")]
public class NewsLetterSubscriptionMigration : ForwardOnlyMigration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        if (!Schema.Table(nameof(NewsLetterSubscriptionType)).Exists())
            Create.TableFor<NewsLetterSubscriptionType>();

        if (!Schema.Table(nameof(NewsLetterSubscription)).Column(nameof(NewsLetterSubscription.TypeId)).Exists())
        {
            //add new column
            Alter.Table(nameof(NewsLetterSubscription))
                .AddColumn(nameof(NewsLetterSubscription.TypeId))
                .AsInt32()
                .ForeignKey(nameof(NewsLetterSubscriptionType), nameof(NewsLetterSubscriptionType.Id))
                .OnDelete(Rule.Cascade)
                .Nullable();
        }

        if (!Schema.Table(nameof(Campaign)).Column(nameof(Campaign.NewsLetterSubscriptionTypeId)).Exists())
        {
            //add new column
            Alter.Table(nameof(Campaign))
                .AddColumn(nameof(Campaign.NewsLetterSubscriptionTypeId)).AsInt32().NotNullable().SetExistingRowsTo(0);
        }
    }
}