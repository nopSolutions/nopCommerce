using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

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
        var messageTemplateTableName = nameof(MessageTemplate);
        var allowDirectReplyColumnName = nameof(MessageTemplate.AllowDirectReply);

        //add column
        if (!Schema.Table(messageTemplateTableName).Column(allowDirectReplyColumnName).Exists())
            Alter.Table(messageTemplateTableName)
                .AddColumn(allowDirectReplyColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);

        //1934
        if (!Schema.Table(nameof(ProductAttributeCombinationPicture)).Exists())
            Create.TableFor<ProductAttributeCombinationPicture>();

        if (!Schema.Table(nameof(ProductAttributeValuePicture)).Exists())
            Create.TableFor<ProductAttributeValuePicture>();

        var productTableName = nameof(Product);
        var displayAttributeCombinationImagesOnlyColumnName = nameof(Product.DisplayAttributeCombinationImagesOnly);

        if (!Schema.Table(productTableName).Column(displayAttributeCombinationImagesOnlyColumnName).Exists())
            Alter.Table(productTableName)
                .AddColumn(displayAttributeCombinationImagesOnlyColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);

        //#6710
        var productAttributeCombinationTableName = nameof(ProductAttributeCombination);
        var pac = Schema.Table(productAttributeCombinationTableName);
        var columnName = "PictureId";
        var description = "The field is not used since 4.70 and is left only for the update process use the ProductAttributeCombinationPicture instead";

        if (pac.Column(columnName).Exists())
            Alter.Table(productAttributeCombinationTableName)
                .AlterColumn(columnName).AsInt32().Nullable().WithColumnDescription(description);
        else
            Alter.Table(productAttributeCombinationTableName)
                .AddColumn(columnName).AsInt32().Nullable().SetExistingRowsTo(null).WithColumnDescription(description);

        var productAttributeValueTableName = nameof(ProductAttributeValue);
        var pav = Schema.Table(productAttributeValueTableName);

        if (pav.Column(columnName).Exists())
            Alter.Table(productAttributeValueTableName)
                .AlterColumn(columnName).AsInt32().Nullable().WithColumnDescription(description);
        else
            Alter.Table(productAttributeValueTableName)
                .AddColumn(columnName).AsInt32().Nullable().SetExistingRowsTo(null).WithColumnDescription(description);

        // 6771
        Alter.Table(nameof(Customer)).AlterColumn(nameof(Customer.LastIpAddress)).AsString(100).Nullable();
        Alter.Table(NameCompatibilityManager.GetTableName(typeof(ForumPost))).AlterColumn(nameof(ForumPost.IPAddress)).AsString(100).Nullable();
        Alter.Table(nameof(ActivityLog)).AlterColumn(nameof(ActivityLog.IpAddress)).AsString(100).Nullable();
        Alter.Table(nameof(Log)).AlterColumn(nameof(Log.IpAddress)).AsString(100).Nullable();
        Alter.Table(nameof(Order)).AlterColumn(nameof(Order.CustomerIp)).AsString(100).Nullable();

        //#6958
        //add column
        var emailAccountTableName = nameof(EmailAccount);
        var maxNumberOfEmailsColumnName = nameof(EmailAccount.MaxNumberOfEmails);

        if (!Schema.Table(emailAccountTableName).Column(maxNumberOfEmailsColumnName).Exists())
            Alter.Table(emailAccountTableName)
                .AddColumn(maxNumberOfEmailsColumnName).AsInt32().NotNullable().SetExistingRowsTo(50);

        //#7031
        if (!Schema.Table(emailAccountTableName).Column(nameof(EmailAccount.EmailAuthenticationMethodId)).Exists())
            Alter.Table(emailAccountTableName)
                .AddColumn(nameof(EmailAccount.EmailAuthenticationMethodId)).AsInt32().NotNullable().SetExistingRowsTo(0);

        if (!Schema.Table(emailAccountTableName).Column(nameof(EmailAccount.ClientId)).Exists())
            Alter.Table(emailAccountTableName)
                .AddColumn(nameof(EmailAccount.ClientId)).AsString().Nullable();

        if (!Schema.Table(emailAccountTableName).Column(nameof(EmailAccount.ClientSecret)).Exists())
            Alter.Table(emailAccountTableName)
                .AddColumn(nameof(EmailAccount.ClientSecret)).AsString().Nullable();

        if (!Schema.Table(emailAccountTableName).Column(nameof(EmailAccount.TenantId)).Exists())
            Alter.Table(emailAccountTableName)
                .AddColumn(nameof(EmailAccount.TenantId)).AsString().Nullable();

        //#6978
        var newsLetterSubscriptionTableName = nameof(NewsLetterSubscription);
        if (!Schema.Table(newsLetterSubscriptionTableName).Column(nameof(NewsLetterSubscription.LanguageId)).Exists())
            Alter.Table(newsLetterSubscriptionTableName)
                .AddColumn(nameof(NewsLetterSubscription.LanguageId)).AsInt32().NotNullable().SetExistingRowsTo(1);
    }
}