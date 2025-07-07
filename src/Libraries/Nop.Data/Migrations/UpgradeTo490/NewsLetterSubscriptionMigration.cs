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

        // Ensure at least one type exists
        Execute.Sql("INSERT INTO [NewsLetterSubscriptionType] ([Name],TickedByDefault,DisplayOrder,LimitedToStores) VALUES ('Default Type',1,1,0)");
                
        if (!Schema.Table(nameof(NewsLetterSubscription)).Column(nameof(NewsLetterSubscription.TypeId)).Exists())
        {
            // Add the column as nullable first
            Alter.Table(nameof(NewsLetterSubscription))
                .AddColumn(nameof(NewsLetterSubscription.TypeId))
                .AsInt32()
                .Nullable();

            // Set all existing rows to the default type (assuming Id=1 for the first record)
            Execute.Sql("UPDATE [NewsLetterSubscription] SET [TypeId] = (SELECT TOP 1 [Id] FROM [NewsLetterSubscriptionType] ORDER BY [Id])");

            // Alter the column to be not nullable and add the foreign key
            Alter.Table(nameof(NewsLetterSubscription))
                .AlterColumn(nameof(NewsLetterSubscription.TypeId))
                .AsInt32()
                .NotNullable()
                .ForeignKey<NewsLetterSubscriptionType>(onDelete: Rule.Cascade);
        }

        if (!Schema.Table(nameof(Campaign)).Column(nameof(Campaign.NewsLetterSubscriptionTypeId)).Exists())
        {
            //add new column
            Alter.Table(nameof(Campaign))
                .AddColumn(nameof(Campaign.NewsLetterSubscriptionTypeId)).AsInt32().NotNullable().SetExistingRowsTo(0);
        }
    }
}