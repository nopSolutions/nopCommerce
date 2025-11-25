using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.UpgradeTo470;

[NopSchemaMigration("2024-04-20 00:00:00", "SchemaMigration for 4.70.0")]
public class SchemaMigration : ForwardOnlyMigration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        //#6167

        //add column
        if (!Schema.ColumnExist<MessageTemplate>(t => t.AllowDirectReply))
        {
            Alter.AlterColumnFor<MessageTemplate>(t => t.AllowDirectReply)
                .AsBoolean()
                .NotNullable()
                .SetExistingRowsTo(false);
        }

        //1934
        if (!Schema.TableExist<ProductAttributeCombinationPicture>())
            Create.TableFor<ProductAttributeCombinationPicture>();

        if (!Schema.TableExist<ProductAttributeValuePicture>())
            Create.TableFor<ProductAttributeValuePicture>();

        if (!Schema.ColumnExist<Product>(t => t.DisplayAttributeCombinationImagesOnly))
        {
            Alter.AddColumnFor<Product>(t => t.DisplayAttributeCombinationImagesOnly)
                .AsBoolean()
                .NotNullable()
                .SetExistingRowsTo(false);
        }

        //#6710
        var description = "The field is not used since 4.70 and is left only for the update process use the ProductAttributeCombinationPicture instead";

        if (Schema.ColumnExist<ProductAttributeCombination>(t => t.PictureId))
        {
            Alter.AlterColumnFor<ProductAttributeCombination>(t => t.PictureId)
                .AsInt32()
                .Nullable()
                .WithColumnDescription(description);
        }
        else
        {
            Alter.AddColumnFor<ProductAttributeCombination>(t => t.PictureId)
                .AsInt32()
                .Nullable()
                .SetExistingRowsTo(null)
                .WithColumnDescription(description);
        }

        if (Schema.ColumnExist<ProductAttributeValue>(t => t.PictureId))
        {
            Alter.AlterColumnFor<ProductAttributeValue>(t => t.PictureId)
                .AsInt32()
                .Nullable()
                .WithColumnDescription(description);
        }
        else
        {
            Alter.AddColumnFor<ProductAttributeValue>(t => t.PictureId)
                .AsInt32()
                .Nullable()
                .SetExistingRowsTo(null)
                .WithColumnDescription(description);
        }

        // 6771
        Alter.AlterColumnFor<Customer>(t => t.LastIpAddress).AsString(100).Nullable();
        Alter.AlterColumnFor<ForumPost>(t => t.IPAddress).AsString(100).Nullable();
        Alter.AlterColumnFor<ActivityLog>(t => t.IpAddress).AsString(100).Nullable();
        Alter.AlterColumnFor<Log>(t => t.IpAddress).AsString(100).Nullable();
        Alter.AlterColumnFor<Order>(t => t.CustomerIp).AsString(100).Nullable();

        //#6958
        //add column

        if (!Schema.ColumnExist<EmailAccount>(t => t.MaxNumberOfEmails))
        {
            Alter.AddColumnFor<EmailAccount>(t => t.MaxNumberOfEmails)
                .AsInt32()
                .NotNullable()
                .SetExistingRowsTo(50);
        }

        //#7031
        if (!Schema.ColumnExist<EmailAccount>(t => t.EmailAuthenticationMethodId))
        {
            Alter.AddColumnFor<EmailAccount>(t => t.EmailAuthenticationMethodId)
                .AsInt32()
                .NotNullable()
                .SetExistingRowsTo(0);
        }

        if (!Schema.ColumnExist<EmailAccount>(t => t.ClientId))
        {
            Alter.AddColumnFor<EmailAccount>(t => t.ClientId)
                .AsString()
                .Nullable();
        }

        if (!Schema.ColumnExist<EmailAccount>(t => t.ClientSecret))
        {
            Alter.AddColumnFor<EmailAccount>(t => t.ClientSecret)
                .AsString()
                .Nullable();
        }

        if (!Schema.ColumnExist<EmailAccount>(t => t.TenantId))
        {
            Alter.AddColumnFor<EmailAccount>(t => t.TenantId)
                .AsString()
                .Nullable();
        }

        //#6978
        if (!Schema.ColumnExist<NewsLetterSubscription>(t => t.LanguageId))
        {
            Alter.AddColumnFor<NewsLetterSubscription>(t => t.LanguageId)
                .AsInt32()
                .NotNullable()
                .SetExistingRowsTo(1);
        }
    }
}