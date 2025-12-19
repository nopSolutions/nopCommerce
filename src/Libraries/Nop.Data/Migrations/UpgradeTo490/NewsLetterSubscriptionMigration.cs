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
        this.CreateTableIfNotExists<NewsLetterSubscriptionType>();

        //add new column
        this.AddOrAlterColumnFor<NewsLetterSubscription>(t => t.TypeId)
            .AsInt32()
            .ForeignKey(nameof(NewsLetterSubscriptionType), nameof(NewsLetterSubscriptionType.Id))
            .OnDelete(Rule.Cascade)
            .Nullable();

        //add new column
        this.AddOrAlterColumnFor<Campaign>(t => t.NewsLetterSubscriptionTypeId).AsInt32().NotNullable().SetExistingRowsTo(0);
    }
}