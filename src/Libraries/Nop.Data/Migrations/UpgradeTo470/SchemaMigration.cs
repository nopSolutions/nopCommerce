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
        this.AddOrAlterColumnFor<MessageTemplate>(t => t.AllowDirectReply)
            .AsBoolean()
            .NotNullable()
            .SetExistingRowsTo(false);

        //1934
        this.CreateTableIfNotExists<ProductAttributeCombinationPicture>();
        this.CreateTableIfNotExists<ProductAttributeValuePicture>();

        this.AddOrAlterColumnFor<Product>(t => t.DisplayAttributeCombinationImagesOnly)
            .AsBoolean()
            .NotNullable()
            .SetExistingRowsTo(false);

        //#6710
        var description = "The field is not used since 4.70 and is left only for the update process use the ProductAttributeCombinationPicture instead";

        this.AddOrAlterColumnFor<ProductAttributeCombination>(t => t.PictureId)
        .AsInt32()
        .Nullable()
        .WithColumnDescription(description);

        this.AddOrAlterColumnFor<ProductAttributeValue>(t => t.PictureId)
            .AsInt32()
            .Nullable()
            .WithColumnDescription(description);

        // 6771
        this.AddOrAlterColumnFor<Customer>(t => t.LastIpAddress)
            .AsString(100)
            .Nullable();

        this.AddOrAlterColumnFor<ForumPost>(t => t.IPAddress)
            .AsString(100)
            .Nullable();

        this.AddOrAlterColumnFor<ActivityLog>(t => t.IpAddress)
            .AsString(100)
            .Nullable();

        this.AddOrAlterColumnFor<Log>(t => t.IpAddress)
            .AsString(100)
            .Nullable();

        this.AddOrAlterColumnFor<Order>(t => t.CustomerIp)
            .AsString(100)
            .Nullable();

        //#6958
        //add column

        this.AddOrAlterColumnFor<EmailAccount>(t => t.MaxNumberOfEmails)
            .AsInt32()
            .NotNullable()
            .SetExistingRowsTo(50);

        //#7031
        this.AddOrAlterColumnFor<EmailAccount>(t => t.EmailAuthenticationMethodId)
            .AsInt32()
            .NotNullable()
            .SetExistingRowsTo(0);

        this.AddOrAlterColumnFor<EmailAccount>(t => t.ClientId)
            .AsString()
            .Nullable();

        this.AddOrAlterColumnFor<EmailAccount>(t => t.ClientSecret)
            .AsString()
            .Nullable();

        this.AddOrAlterColumnFor<EmailAccount>(t => t.TenantId)
            .AsString()
            .Nullable();

        //#6978
        this.AddOrAlterColumnFor<NewsLetterSubscription>(t => t.LanguageId)
            .AsInt32()
            .NotNullable()
            .SetExistingRowsTo(1);
    }
}