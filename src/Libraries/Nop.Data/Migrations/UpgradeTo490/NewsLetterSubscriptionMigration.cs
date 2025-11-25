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
        if (!Schema.TableExist<NewsLetterSubscriptionType>())
            Create.TableFor<NewsLetterSubscriptionType>();

        if (!Schema.ColumnExist<NewsLetterSubscription>(t => t.TypeId))
        {
            //add new column
            Alter.AddColumnFor<NewsLetterSubscription>(t => t.TypeId)
                .AsInt32()
                .ForeignKey<NewsLetterSubscriptionType>()
                .Nullable();
        }

        if (!Schema.ColumnExist<Campaign>(t => t.NewsLetterSubscriptionTypeId))
        {
            //add new column
            Alter.AddColumnFor<Campaign>(t => t.NewsLetterSubscriptionTypeId)
                .AsInt32()
                .NotNullable()
                .SetExistingRowsTo(0);
        }
    }
}