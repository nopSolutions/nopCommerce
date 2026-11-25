using FluentMigrator;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Messages;

namespace Nop.Data.Migrations.UpgradeTo510;

[NopUpdateMigration("2026-10-28 00:00:00", "5.10", UpdateMigrationType.Data)]
public class DataMigration : Migration
{
    private const int PAGE_SIZE = 500;
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
        //#8224
        while (true)
        {
            var comments = _dataProvider.GetTable<BlogComment>()
                .Join(_dataProvider.GetTable<Customer>(), bc => bc.CustomerId, c => c.Id, (bc, c) => new { bc, c })
                .Where(t => string.IsNullOrEmpty(t.c.Email))
                .Select(t => t.bc).Take(PAGE_SIZE).ToList();

            if (!comments.Any())
                break;

            foreach (var comment in comments)
                comment.CustomerId = null;

            _dataProvider.UpdateEntities(comments);
        }

        while (true)
        {
            var productReviews = _dataProvider.GetTable<ProductReview>()
                .Join(_dataProvider.GetTable<Customer>(), pr => pr.CustomerId, c => c.Id, (pr, c) => new { pr, c })
                .Where(t => string.IsNullOrEmpty(t.c.Email))
                .Select(t => t.pr).Take(PAGE_SIZE).ToList();

            if (!productReviews.Any())
                break;

            foreach (var pr in productReviews)
                pr.CustomerId = null;

            _dataProvider.UpdateEntities(productReviews);
        }

        while (true)
        {
            var searchTerms = _dataProvider.GetTable<SearchTerm>()
                .Join(_dataProvider.GetTable<Customer>(), term => term.CustomerId, c => c.Id,
                    (term, c) => new { term, c })
                .Where(t => string.IsNullOrEmpty(t.c.Email))
                .Select(t => t.term).Take(PAGE_SIZE).ToList();

            if (!searchTerms.Any())
                break;

            foreach (var term in searchTerms)
                term.CustomerId = null;

            _dataProvider.UpdateEntities(searchTerms);
        }

        //#8273
        if (!_dataProvider.GetTable<MessageTemplate>().Any(st => string.Compare(st.Name,
            MessageTemplateSystemNames.NEW_AFFILIATE_ACCOUNT_APPLY_STORE_OWNER_NOTIFICATION,
            StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            var eaGeneral = _dataProvider.GetTable<EmailAccount>().FirstOrDefault()
                ?? throw new Exception("Default email account cannot be loaded");

            _dataProvider.InsertEntity(new MessageTemplate
            {
                Name = MessageTemplateSystemNames.NEW_AFFILIATE_ACCOUNT_APPLY_STORE_OWNER_NOTIFICATION,
                Subject = "%Store.Name%. New affiliate account submitted.",
                Body =
                    $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Customer.FullName% (%Customer.Email%) has just submitted for an affiliate account. Details are below:{Environment.NewLine}<br />{Environment.NewLine}Affiliate friendly URL name: %Affiliate.FriendlyUrlName%{Environment.NewLine}<br />{Environment.NewLine}Affiliate email: %Affiliate.Email%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}You can activate it in admin area.{Environment.NewLine}</p>{Environment.NewLine}",
                IsActive = true,
                EmailAccountId = eaGeneral.Id
            });
        }

        if (!_dataProvider.GetTable<MessageTemplate>().Any(st => string.Compare(st.Name,
            MessageTemplateSystemNames.AFFILIATE_ACCOUNT_ACTIVE_CUSTOMER_NOTIFICATION,
            StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            var eaGeneral = _dataProvider.GetTable<EmailAccount>().FirstOrDefault()
                ?? throw new Exception("Default email account cannot be loaded");

            _dataProvider.InsertEntity(new MessageTemplate
            {
                Name = MessageTemplateSystemNames.AFFILIATE_ACCOUNT_ACTIVE_CUSTOMER_NOTIFICATION,
                Subject = "%Store.Name%. Affiliate account activated.",
                Body =
                    $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Your affiliate account has just been activated. Details are below:{Environment.NewLine}<br />{Environment.NewLine}Affiliate friendly URL name: %Affiliate.FriendlyUrlName%",
                IsActive = true,
                EmailAccountId = eaGeneral.Id
            });
        }

        var activityLogTypeTable = _dataProvider.GetTable<ActivityLogType>();

        if (!activityLogTypeTable.Any(alt =>
                string.Compare(alt.SystemKeyword, "AddNewAffiliateCommission",
                    StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _dataProvider.InsertEntity(new ActivityLogType
            {
                SystemKeyword = "AddNewAffiliateCommission",
                Enabled = true,
                Name = "Add a new affiliate commission"
            });
        }

        if (!activityLogTypeTable.Any(alt =>
                string.Compare(alt.SystemKeyword, "DeleteAffiliateCommission",
                    StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _dataProvider.InsertEntity(new ActivityLogType
            {
                SystemKeyword = "DeleteAffiliateCommission",
                Enabled = true,
                Name = "Delete an affiliate commission"
            });
        }

        if (!activityLogTypeTable.Any(alt =>
                string.Compare(alt.SystemKeyword, "EditAffiliateCommission",
                    StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _dataProvider.InsertEntity(new ActivityLogType
            {
                SystemKeyword = "EditAffiliateCommission",
                Enabled = true,
                Name = "Edit an affiliate commission"
            });
        }
    }

    /// <summary>Collects the DOWN migration expressions</summary>
    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}