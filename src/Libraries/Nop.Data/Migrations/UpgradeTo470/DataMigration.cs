using FluentMigrator;
using LinqToDB;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Security;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.UpgradeTo470;

[NopUpdateMigration("2023-01-01 00:00:00", "4.70", UpdateMigrationType.Data)]
public class DataMigration : Migration
{
    private readonly INopDataProvider _dataProvider;

    public DataMigration(INopDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        //#5312 new activity log type
        var activityLogTypeTable = _dataProvider.GetTable<ActivityLogType>();

        if (!activityLogTypeTable.Any(alt =>
                string.Compare(alt.SystemKeyword, "ImportCustomers", StringComparison.InvariantCultureIgnoreCase) ==
                0))
        {
            _dataProvider.InsertEntity(
                new ActivityLogType
                {
                    SystemKeyword = "ImportCustomers",
                    Enabled = true,
                    Name = "Customers were imported"
                }
            );
        }

        //6660 new activity log type for update plugin
        if (!activityLogTypeTable.Any(alt =>
                string.Compare(alt.SystemKeyword, "UpdatePlugin", StringComparison.InvariantCultureIgnoreCase) ==
                0))
        {
            _dataProvider.InsertEntity(
                new ActivityLogType { SystemKeyword = "UpdatePlugin", Enabled = true, Name = "Update a plugin" }
            );
        }

        // new permission
        if (_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "AccessProfiling", StringComparison.InvariantCultureIgnoreCase) == 0)) 
            _dataProvider.BulkDeleteEntitiesAsync<PermissionRecord>(pr => pr.SystemName == "AccessProfiling");

        //#6890
        //remove column
        this.DeleteColumnsIfExists<Product>(["IsTelecommunicationsOrBroadcastingOrElectronicServices"]);

        //New message template
        if (!_dataProvider.GetTable<MessageTemplate>().Any(st => string.Compare(st.Name, MessageTemplateSystemNames.DELETE_CUSTOMER_REQUEST_STORE_OWNER_NOTIFICATION, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            var eaGeneral = _dataProvider.GetTable<EmailAccount>().FirstOrDefault() ?? throw new Exception("Default email account cannot be loaded");
            _dataProvider.InsertEntity(new MessageTemplate()
            {
                Name = MessageTemplateSystemNames.DELETE_CUSTOMER_REQUEST_STORE_OWNER_NOTIFICATION,
                Subject = "%Store.Name%. New request to delete customer (GDPR)",
                Body = $"%Customer.Email% has requested account deletion. You can consider this in the admin area.",
                IsActive = true,
                EmailAccountId = eaGeneral.Id
            });
        }

        //#7031
        var emailAccountTableName = nameof(EmailAccount);
        var credentialsColumnName = "UseDefaultCredentials";

        if (Schema.Table(emailAccountTableName).Column(credentialsColumnName).Exists())
        {
            var emailAccounts = _dataProvider.GetTable<EmailAccount>().ToList();
            foreach (var item in emailAccounts)
            {
                if (!string.IsNullOrEmpty(item.Username))
                    item.EmailAuthenticationMethod = EmailAuthenticationMethod.Login;
            }

            _dataProvider.UpdateEntities(emailAccounts);

            //remove column
            Delete.Column(credentialsColumnName).FromTable(emailAccountTableName);
        }

        //#6978
        var newsLetterSubscriptionTableName = nameof(NewsLetterSubscription);
        var languageIdColumnName = nameof(NewsLetterSubscription.LanguageId);
        if (Schema.Table(newsLetterSubscriptionTableName).Column(languageIdColumnName).Exists())
        {
            var defaultLanguageId = _dataProvider.GetTable<Language>().FirstOrDefault()?.Id ?? 0;

            _dataProvider.GetTable<NewsLetterSubscription>()
                .Set(p => p.LanguageId, defaultLanguageId)
                .Update();
        }
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}